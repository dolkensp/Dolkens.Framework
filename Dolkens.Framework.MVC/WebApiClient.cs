using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.MVC.Async
{
    using MediaTypeFormatterEnum = Dolkens.Framework.MVC.WebApiClient.MediaTypeFormatterEnum;

    public class WebApiClient : IDisposable
    {
        #region Private Properties

        protected HttpClient _client;

        #endregion

        #region Constructor/s

        public WebApiClient()
        {
            this._client = this._client ?? new HttpClient { };
        }

        public WebApiClient(String baseAddress)
            : base()
        {
            if (!String.IsNullOrWhiteSpace(baseAddress))
            {
                this.BaseAddress = new Uri(baseAddress);
            }
        }

        public WebApiClient(Uri baseAddress)
            : base()
        {
            if (baseAddress != null)
            {
                this.BaseAddress = baseAddress;
            }
        }

        #endregion

        #region Public Properties

        public Uri BaseAddress
        {
            get
            {
                this._client = this._client ?? new HttpClient { };
                return this._client.BaseAddress;
            }
            set
            {
                this._client = this._client ?? new HttpClient { };
                this._client.BaseAddress = value;
            }
        }

        private MediaTypeFormatterEnum _mediaTypeFormatter = MediaTypeFormatterEnum.Json;
        public virtual MediaTypeFormatterEnum MediaTypeFormatter
        {
            get { return this._mediaTypeFormatter; }
        }

        // public HttpRequestHeaders DefaultRequestHeaders { get; }
        // public long MaxResponseContentBufferSize { get; set; }
        // public TimeSpan Timeout { get; set; }

        #endregion

        public void Dispose()
        {
            this._client = this._client ?? new HttpClient { };
            this._client.Dispose();
        }
    }
}

namespace Dolkens.Framework.MVC
{
    using Utilities = Dolkens.Framework.MVC.WebApiClient;

    #region Interfaces

    public interface IWebApiRequest<TResponse>
    {
        String ResourceUrl { get; }
    }

    public interface IWebApiRequest<TResponse, TError> : IWebApiRequest<TResponse> { }

    #endregion

    public class ApiResult<TResponse>
    {
        public TResponse Response { get; set; }
    }

    public class ApiResult<TResponse, TError> : ApiResult<TResponse>
    {
        public TError Error { get; set; }
    }

    public class WebApiClient : IDisposable
    {
        private class NullResult { }

        public enum MediaTypeFormatterEnum
        {
            Bson,
            Json,
            FormUrlEncoded,
            WwwFormUrlEncoded,
            Xml
        }

        #region Private Properties

        protected HttpClient _client;

        #endregion

        #region Constructor/s

        public WebApiClient()
        {
            this._client = this._client ?? new HttpClient { };
        }

        public WebApiClient(String baseAddress)
            : base()
        {
            if (!String.IsNullOrWhiteSpace(baseAddress))
            {
                this.BaseAddress = new Uri(baseAddress);
            }
        }

        public WebApiClient(Uri baseAddress)
            : base()
        {
            if (baseAddress != null)
            {
                this.BaseAddress = baseAddress;
            }
        }

        #endregion

        #region Public Properties

        public Uri BaseAddress
        {
            get
            {
                this._client = this._client ?? new HttpClient { };
                return this._client.BaseAddress;
            }
            set
            {
                this._client = this._client ?? new HttpClient { };
                this._client.BaseAddress = value;
            }
        }

        private MediaTypeFormatterEnum _mediaTypeFormatter = MediaTypeFormatterEnum.Json;
        public virtual MediaTypeFormatterEnum MediaTypeFormatter
        {
            get { return this._mediaTypeFormatter; }
        }

        // public HttpRequestHeaders DefaultRequestHeaders { get; }
        // public long MaxResponseContentBufferSize { get; set; }
        // public TimeSpan Timeout { get; set; }

        #endregion

        #region CRUD Methods

        public virtual TResponse Create<TResponse>(IWebApiRequest<TResponse> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) { return this.Create<TResponse>(data.ResourceUrl, data, mediaTypeFormatter); }

        public virtual TResponse Create<TResponse>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null)
        {
            HttpContent content = data == null ?
                new ObjectContent(typeof(Object), data, WebApiClient.GetMediaTypeFormatter(mediaTypeFormatter ?? this.MediaTypeFormatter)) :
                new ObjectContent(data.GetType(), data, WebApiClient.GetMediaTypeFormatter(mediaTypeFormatter ?? this.MediaTypeFormatter));

            return this._client.PostAsync(resource, content).Result.Content.ReadAsAsync<TResponse>().Result;
        }

        public virtual TResponse Retrieve<TResponse>(IWebApiRequest<TResponse> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) { return this.Retrieve<TResponse>(data.ResourceUrl, data, mediaTypeFormatter); }

        public virtual TResponse Retrieve<TResponse>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null)
        {
            var query = WwwFormUrlEncodedMediaTypeFormatter.Serialize(data);

            if (!String.IsNullOrWhiteSpace(query))
            {
                if (resource.Contains("?"))
                {
                    resource = $"{resource}&{query}";
                }
                else
                {
                    resource = $"{resource}?{query}";
                }
            }

            return this._client.GetAsync(resource).Result.Content.ReadAsAsync<TResponse>().Result;
        }

        public virtual TResponse Update<TResponse>(IWebApiRequest<TResponse> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) { return this.Update<TResponse>(data.ResourceUrl, data, mediaTypeFormatter); }

        public virtual TResponse Update<TResponse>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null)
        {
            HttpContent content = data == null ?
                new ObjectContent(typeof(Object), data, WebApiClient.GetMediaTypeFormatter(mediaTypeFormatter ?? this.MediaTypeFormatter)) :
                new ObjectContent(data.GetType(), data, WebApiClient.GetMediaTypeFormatter(mediaTypeFormatter ?? this.MediaTypeFormatter));

            return this._client.PutAsync(resource, content).Result.Content.ReadAsAsync<TResponse>().Result;
        }

        public virtual TResponse Delete<TResponse>(IWebApiRequest<TResponse> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) { return this.Delete<TResponse>(data.ResourceUrl, data, mediaTypeFormatter); }

        public virtual TResponse Delete<TResponse>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null)
        {
            var query = WwwFormUrlEncodedMediaTypeFormatter.Serialize(data);

            if (!String.IsNullOrWhiteSpace(query))
            {
                if (resource.Contains("?"))
                {
                    resource = $"{resource}&{query}";
                }
                else
                {
                    resource = $"{resource}?{query}";
                }
            }

            return this._client.DeleteAsync(resource).Result.Content.ReadAsAsync<TResponse>().Result;
        }

        #endregion

        #region async CRUD Methods

        public virtual async Task<ApiResult<TResponse>> CreateAsync<TResponse>(IWebApiRequest<TResponse> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) => await this.CreateAsync<TResponse, NullResult>(data.ResourceUrl, data, mediaTypeFormatter);
        public virtual async Task<ApiResult<TResponse>> CreateAsync<TResponse>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) => await this.CreateAsync<TResponse, NullResult>(resource, data, mediaTypeFormatter);

        public virtual async Task<ApiResult<TResponse, TError>> CreateAsync<TResponse, TError>(IWebApiRequest<TResponse, TError> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) => await this.CreateAsync<TResponse, TError>(data.ResourceUrl, data, mediaTypeFormatter);
        public virtual async Task<ApiResult<TResponse, TError>> CreateAsync<TResponse, TError>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null)
        {
            HttpContent content = data == null ?
                new ObjectContent(typeof(Object), data, Utilities.GetMediaTypeFormatter(mediaTypeFormatter ?? this.MediaTypeFormatter)) :
                new ObjectContent(data.GetType(), data, Utilities.GetMediaTypeFormatter(mediaTypeFormatter ?? this.MediaTypeFormatter));

            var result = await this._client.PostAsync(resource, content);

            if (result.StatusCode == HttpStatusCode.OK) return new ApiResult<TResponse, TError> { Response = await result.Content.ReadAsAsync<TResponse>() };

            return new ApiResult<TResponse, TError> { Error = await result.Content.ReadAsAsync<TError>() };
        }

        public virtual async Task<ApiResult<TResponse>> RetrieveAsync<TResponse>(IWebApiRequest<TResponse> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) => await this.RetrieveAsync<TResponse, NullResult>(data.ResourceUrl, data, mediaTypeFormatter);
        public virtual async Task<ApiResult<TResponse>> RetrieveAsync<TResponse>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) => await this.RetrieveAsync<TResponse, NullResult>(resource, data, mediaTypeFormatter);

        public virtual async Task<ApiResult<TResponse, TError>> RetrieveAsync<TResponse, TError>(IWebApiRequest<TResponse, TError> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) => await this.RetrieveAsync<TResponse, TError>(data.ResourceUrl, data, mediaTypeFormatter);
        public virtual async Task<ApiResult<TResponse, TError>> RetrieveAsync<TResponse, TError>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null)
        {
            var query = WwwFormUrlEncodedMediaTypeFormatter.Serialize(data);

            if (!String.IsNullOrWhiteSpace(query))
            {
                if (resource.Contains("?")) resource = $"{resource}&{query}";
                else resource = $"{resource}?{query}";
            }

            var result = await this._client.GetAsync(resource);

            if (result.StatusCode == HttpStatusCode.OK) return new ApiResult<TResponse, TError> { Response = await result.Content.ReadAsAsync<TResponse>() };

            return new ApiResult<TResponse, TError> { Error = await result.Content.ReadAsAsync<TError>() };
        }

        public virtual async Task<ApiResult<TResponse>> UpdateAsync<TResponse>(IWebApiRequest<TResponse> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) => await this.UpdateAsync<TResponse, NullResult>(data.ResourceUrl, data, mediaTypeFormatter);
        public virtual async Task<ApiResult<TResponse>> UpdateAsync<TResponse>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) => await this.UpdateAsync<TResponse, NullResult>(resource, data, mediaTypeFormatter);

        public virtual async Task<ApiResult<TResponse, TError>> UpdateAsync<TResponse, TError>(IWebApiRequest<TResponse, TError> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) => await this.UpdateAsync<TResponse, TError>(data.ResourceUrl, data, mediaTypeFormatter);
        public virtual async Task<ApiResult<TResponse, TError>> UpdateAsync<TResponse, TError>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null)
        {
            HttpContent content = data == null ?
                new ObjectContent(typeof(Object), data, Utilities.GetMediaTypeFormatter(mediaTypeFormatter ?? this.MediaTypeFormatter)) :
                new ObjectContent(data.GetType(), data, Utilities.GetMediaTypeFormatter(mediaTypeFormatter ?? this.MediaTypeFormatter));

            var result = await this._client.PutAsync(resource, content);

            if (result.StatusCode == HttpStatusCode.OK) return new ApiResult<TResponse, TError> { Response = await result.Content.ReadAsAsync<TResponse>() };

            return new ApiResult<TResponse, TError> { Error = await result.Content.ReadAsAsync<TError>() };
        }

        public virtual async Task<ApiResult<TResponse>> DeleteAsync<TResponse>(IWebApiRequest<TResponse> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) => await this.DeleteAsync<TResponse, NullResult>(data.ResourceUrl, data, mediaTypeFormatter);
        public virtual async Task<ApiResult<TResponse>> DeleteAsync<TResponse>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) => await this.DeleteAsync<TResponse, NullResult>(resource, data, mediaTypeFormatter);

        public virtual async Task<ApiResult<TResponse, TError>> DeleteAsync<TResponse, TError>(IWebApiRequest<TResponse, TError> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) => await this.DeleteAsync<TResponse, TError>(data.ResourceUrl, data, mediaTypeFormatter);
        public virtual async Task<ApiResult<TResponse, TError>> DeleteAsync<TResponse, TError>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null)
        {
            var query = WwwFormUrlEncodedMediaTypeFormatter.Serialize(data);

            if (!String.IsNullOrWhiteSpace(query))
            {
                if (resource.Contains("?")) resource = $"{resource}&{query}";
                else resource = $"{resource}?{query}";
            }

            var result = await this._client.DeleteAsync(resource);

            if (result.StatusCode == HttpStatusCode.OK) return new ApiResult<TResponse, TError> { Response = await result.Content.ReadAsAsync<TResponse>() };

            return new ApiResult<TResponse, TError> { Error = await result.Content.ReadAsAsync<TError>() };
        }

        #endregion

        public void Dispose()
        {
            this._client = this._client ?? new HttpClient { };
            this._client.Dispose();
        }

        internal static MediaTypeFormatter GetMediaTypeFormatter(MediaTypeFormatterEnum mediaTypeFormatter)
        {
            switch (mediaTypeFormatter)
            {
                // case MediaTypeFormatterEnum.Bson:
                //     return new BsonMediaTypeFormatter { };
                case MediaTypeFormatterEnum.FormUrlEncoded:
                    return new FormUrlEncodedMediaTypeFormatter { };
                case MediaTypeFormatterEnum.Json:
                    return new JsonMediaTypeFormatter { };
                case MediaTypeFormatterEnum.WwwFormUrlEncoded:
                    return new WwwFormUrlEncodedMediaTypeFormatter { };
                case MediaTypeFormatterEnum.Xml:
                    return new XmlMediaTypeFormatter { };
            }

            throw new ArgumentException("Invalid MediaTypeFormatter specified", "mediaTypeFormatter");
        }
    }
}