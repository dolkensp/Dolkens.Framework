using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        // public HttpRequestHeaders DefaultRequestHeaders { get; }
        // public long MaxResponseContentBufferSize { get; set; }
        // public TimeSpan Timeout { get; set; }

        #endregion

        #region CRUD Methods

        public virtual TResponse Create<TResponse>(IWebApiRequest<TResponse> data = null) { return this.Create<TResponse>(data.ResourceUrl, data); }

        public virtual TResponse Create<TResponse>(String resource, Object data = null)
        {
            HttpContent content = data == null ?
                new ObjectContent(typeof(Object), data, new WwwFormUrlEncodedMediaTypeFormatter { }) :
                new ObjectContent(data.GetType(), data, new WwwFormUrlEncodedMediaTypeFormatter { });

            return this._client.PostAsync(resource, content).Result.Content.ReadAsAsync<TResponse>().Result;
        }

        public virtual TResponse Retrieve<TResponse>(IWebApiRequest<TResponse> data = null) { return this.Retrieve<TResponse>(data.ResourceUrl, data); }

        public virtual TResponse Retrieve<TResponse>(String resource, Object data = null)
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

        public virtual TResponse Update<TResponse>(IWebApiRequest<TResponse> data = null) { return this.Update<TResponse>(data.ResourceUrl, data); }

        public virtual TResponse Update<TResponse>(String resource, Object data = null)
        {
            HttpContent content = data == null ?
                new ObjectContent(typeof(Object), data, new WwwFormUrlEncodedMediaTypeFormatter { }) :
                new ObjectContent(data.GetType(), data, new WwwFormUrlEncodedMediaTypeFormatter { });

            return this._client.PutAsync(resource, content).Result.Content.ReadAsAsync<TResponse>().Result;
        }

        public virtual TResponse Delete<TResponse>(IWebApiRequest<TResponse> data = null) { return this.Delete<TResponse>(data.ResourceUrl, data); }

        public virtual TResponse Delete<TResponse>(String resource, Object data = null)
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
    }
}
