using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.MVC
{
    #region Interfaces

    public interface IWebApiRequest<TResponse>
    {
        String ResourceUrl { get; }
    }

    #endregion

    public class WebApiClient : IDisposable
    {
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
                new ObjectContent(typeof(Object), data, this.GetMediaTypeFormatter(mediaTypeFormatter)) :
                new ObjectContent(data.GetType(), data, this.GetMediaTypeFormatter(mediaTypeFormatter));

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
                    resource = String.Format("{0}&{1}", resource, query);
                }
                else
                {
                    resource = String.Format("{0}?{1}", resource, query);
                }
            }

            return this._client.GetAsync(resource).Result.Content.ReadAsAsync<TResponse>().Result;
        }

        public virtual TResponse Update<TResponse>(IWebApiRequest<TResponse> data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null) { return this.Update<TResponse>(data.ResourceUrl, data, mediaTypeFormatter); }

        public virtual TResponse Update<TResponse>(String resource, Object data = null, MediaTypeFormatterEnum? mediaTypeFormatter = null)
        {
            HttpContent content = data == null ?
                new ObjectContent(typeof(Object), data, this.GetMediaTypeFormatter(mediaTypeFormatter)) :
                new ObjectContent(data.GetType(), data, this.GetMediaTypeFormatter(mediaTypeFormatter));

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
                    resource = String.Format("{0}&{1}", resource, query);
                }
                else
                {
                    resource = String.Format("{0}?{1}", resource, query);
                }
            }

            return this._client.DeleteAsync(resource).Result.Content.ReadAsAsync<TResponse>().Result;
        }

        #endregion

        public void Dispose()
        {
            this._client = this._client ?? new HttpClient { };
            this._client.Dispose();
        }

        private MediaTypeFormatter GetMediaTypeFormatter(MediaTypeFormatterEnum? mediaTypeFormatter = null)
        {
            mediaTypeFormatter = mediaTypeFormatter ?? this.MediaTypeFormatter;
            
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
