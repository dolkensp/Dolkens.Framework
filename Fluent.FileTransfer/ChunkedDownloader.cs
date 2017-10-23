using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadUtilities
{
    public class ChunkedRequest
    {
        private const Int64 KB = 1024;
        private const Int64 MB = 1024 * KB;
        private const Int64 GB = 1024 * MB;

        #region Constructors

        static ChunkedRequest()
        {
            if (ServicePointManager.DefaultConnectionLimit < Environment.ProcessorCount) ServicePointManager.DefaultConnectionLimit = Environment.ProcessorCount; // Unlock additional threads for downloads
        }

        private ChunkedRequest() { }

        #endregion

        private String _sourceUrl;
        private String _targetPath;
        private CancellationTokenSource _cancellationSource;
        private Boolean _downloading;
        private String _tempPath;
        private String _eTag;

        #region Public Methods

        public static ChunkedRequest Create(String sourceUrl)
        {
            return new ChunkedRequest
            {
                _sourceUrl = sourceUrl,
                _downloading = false,
            };
        }

        public ChunkedRequest TempFile(String tempFile)
        {
            this._tempPath = new FileInfo(tempFile).FullName;

            return this;
        }

        public async Task PauseAsync()
        {
            this._cancellationSource.Cancel(false);

            await Task.Delay(0);
        }

        public async Task ResumeAsync()
        {
            await this.DownloadAsync(this._targetPath);
        }

        public async Task AbortAsync()
        {
            if (!this._cancellationSource.IsCancellationRequested) this._cancellationSource.Cancel(false);

            SpinWait.SpinUntil(() => !this._downloading, 5000); // Wait up to 5000ms for threads to finish

            var tempPattern = this.GetTempFile("*");
            foreach (var tempPath in Directory.GetFiles(Path.GetDirectoryName(tempPattern), Path.GetFileName(tempPattern)))
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }

            await Task.Delay(0);
        }

        public async Task DownloadAsync(String targetPath)
        {
            if (this._downloading) throw new Exception("Download already in progress.");

            this._cancellationSource = new CancellationTokenSource();

            var cancellationToken = this._cancellationSource.Token;

            var sourceUrl = this._sourceUrl;

            this._targetPath = new FileInfo(targetPath).FullName;

            // var maxRequests = 4;

            var contentLength = -1L;
            var chunks = Environment.ProcessorCount; // Default to 1 chunk per thread
            var minChunkSize = 10L * MB; // Don't use chunks smaller than 10MB
            var maxChunkSize = 512L * MB; // Don't use chunks larger than 512MB
            var chunkSize = minChunkSize;

            #region Calculate Chunks

            if (chunks > 1)
            {
                try
                {
                    var request = HttpWebRequest.CreateHttp(this._sourceUrl);
                    request.Method = "HEAD";

                    using (var response = await request.GetResponseAsync() as HttpWebResponse)
                    {
                        if (response.Headers[HttpResponseHeader.AcceptRanges] != "bytes") chunks = 1; // Can't chunk files that don't support Ranges
                        this._eTag = response.Headers[HttpResponseHeader.ETag];
                        contentLength = response.ContentLength;
                    }
                }
                catch (WebException)
                {
                    // WebException was thrown on HEAD request, so we can't support chunks

                    chunks = 1;
                    chunkSize = -1;
                    contentLength = -1;
                }
            }

            if (contentLength == -1)
            {
                // Can't chunk files with unknown file size
                chunks = 1;
                chunkSize = -1;
            }
            else
            {
                // Divide the file up so every thread has something to do
                chunkSize = PowerFloor(contentLength / chunks, 2);
                if (chunkSize < minChunkSize) chunkSize = minChunkSize;
                if (chunkSize > maxChunkSize) chunkSize = maxChunkSize;
                chunks = (Int32)(contentLength / chunkSize);
                if (contentLength > chunks * chunkSize) chunks += 1;
            }

            #endregion

            #region Download Chunks

            var chunkTasks = new Task<String>[chunks];

            this._downloading = true;

            // var throttler = new SemaphoreSlim(initialCount: maxRequests);

            for (Int32 i = 0, j = chunks; i < j; i++)
            {
                chunkTasks[i] = this.DownloadChunkAsync(this._targetPath, i, chunkSize, contentLength);

                // Task.Run(async () =>
                // {
                //     try { await ; }
                //     finally { throttler.Release(); }
                // });
            }

            await Task.WhenAll(chunkTasks);

            this._downloading = false;

            #endregion

            #region Merge Chunks

            if (this._cancellationSource.Token.IsCancellationRequested) return;

            var tempFile = this.GetTempFile($"file");
            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));

            using (var outFile = File.Open(tempFile, FileMode.Create, FileAccess.Write))
            {
                for (Int32 i = 0, j = chunks; i < j; i++)
                {
                    using (var inFile = File.Open(chunkTasks[i].Result, FileMode.Open, FileAccess.Read))
                    {
                        Int32 bytes;
                        var buffer = new Byte[1 * MB];
                        while ((bytes = await inFile.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            if (this._cancellationSource.Token.IsCancellationRequested) return;
                            await outFile.WriteAsync(buffer, 0, bytes);
                        }
                        // await inFile.CopyToAsync(outFile, 1 * MB, this._cancellationSource.Token);
                        outFile.Seek(-1, SeekOrigin.Current); // Move back 1 byte when merging to remove EOF bytes
                    }

                    File.Delete(chunkTasks[i].Result);
                }
            }

            #endregion

            #region Move Temporary File

            if (this._cancellationSource.Token.IsCancellationRequested) return;

            Directory.CreateDirectory(Path.GetDirectoryName(this._targetPath));
            if (File.Exists(this._targetPath)) File.Delete(this._targetPath);
            File.Move(tempFile, this._targetPath);

            #endregion
        }

        #endregion

        #region Private Methods

        private async Task<String> DownloadChunkAsync(String targetPath, Int32 chunkID, Int64 chunkSize, Int64 contentLength)
        {
            var maxDelay = 300000; // Max delay of 5 minutes
            var retryDelay = 5000; // Default delay of 5 seconds
            var maxRetries = 6;    // Max of 6 retry attempts
            var retries = 0;

            var tempFile = this.GetTempFile($"{chunkID:0000}");
            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));

            while (retries++ < maxRetries)
            {
                try
                {
                    if (this._cancellationSource.Token.IsCancellationRequested) return tempFile;

                    using (var outFile = File.Open(tempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        var offset = chunkID * chunkSize;
                        var request = HttpWebRequest.CreateHttp(this._sourceUrl);

                        if (contentLength != -1 && chunkSize != -1)
                        {
                            if (outFile.Length >= chunkSize) return tempFile; // Chunk is already complete
                            if (offset + outFile.Length >= contentLength) return tempFile; // Final chunk is already complete

                            request.AddRange(offset + outFile.Length, offset + chunkSize);
                        }

                        outFile.Position = outFile.Length;

                        using (var response = await request.GetResponseAsync() as HttpWebResponse)
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                Int32 bytes;
                                var buffer = new Byte[1 * MB];
                                while ((bytes = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    if (this._cancellationSource.Token.IsCancellationRequested) return tempFile;
                                    await outFile.WriteAsync(buffer, 0, bytes);
                                }
                                // await responseStream.CopyToAsync(outFile, 1 * MB, this._cancellationSource.Token);
                            }
                        }

                        return tempFile;
                    }
                }
                catch (Exception ex)
                {
                    if (this._cancellationSource.Token.IsCancellationRequested) return tempFile;

                    // If there's an error - delete the chunk and try again
                    if (File.Exists(tempFile)) File.Delete(tempFile);

                    // If there's an error - wait for an exponential amount of time
                    await Task.Delay(Math.Min(maxDelay, retryDelay * retries * retries));

                    if (retries == maxRetries) throw new Exception("Failed to download chunk", ex);
                }
            }

            return tempFile;
        }

        #endregion

        #region Private Helpers

        private String GetTempFile(String extension)
        {
            var eTag = String.Empty;
            if (!String.IsNullOrWhiteSpace(this._eTag)) eTag = $".{this._eTag.Replace("\"", "").Replace(":", "")}";

            if (String.IsNullOrWhiteSpace(this._tempPath))
            {
                return Path.Combine(Path.GetTempPath(), $"{typeof(ChunkedRequest).FullName}", $"{(UInt64)this._targetPath.GetHashCode()}{eTag}.{extension}");
            }
            else
            {
                return $"{this._tempPath}{eTag}.{extension}";
            }
        }

        private static Int32 PowerFloor(Int32 val, Int32 pow)
        {
            if (pow < 0) return 0;
            if (pow == 0) return val;

            if (val >> 1 > 0) return PowerFloor(val >> 1, 2) << 1;

            return val;
        }

        private static Int64 PowerFloor(Int64 val, Int32 pow)
        {
            if (pow < 0) return 0;
            if (pow == 0) return val;

            if (val >> 1 > 0) return PowerFloor(val >> 1, 2) << 1;

            return val;
        }

        #endregion
    }
}