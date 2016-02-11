using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dolkens.Framework.MVC
{
    public class JsonNetResult : ActionResult
    {
        const String JsonRequest_GetNotAllowed = "This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.";

        public JsonNetResult() : base()
        {
            this.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public Encoding ContentEncoding { get; set; }

        public String ContentType { get; set; }

        public Object Data { get; set; }

        public JsonRequestBehavior JsonRequestBehavior { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(JsonNetResult.JsonRequest_GetNotAllowed);

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
                response.ContentType = ContentType;
            else
                response.ContentType = "application/json";
            
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            if (Data != null)
                response.Write(this.Data.ToJSON());
        }
    }
}
