using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.MVC
{
    public class WwwFormUrlEncodedMediaTypeFormatter : FormUrlEncodedMediaTypeFormatter
    {
        private readonly static Encoding encoding = new UTF8Encoding(false);

        public override Boolean CanWriteType(Type type)
        {
            return base.CanWriteType(type) || true; // type == typeof(JObject);
        }

        public override Task WriteToStreamAsync(Type type, Object value, Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext, System.Threading.CancellationToken cancellationToken)
        {
            if (value == null)
            {
                return Task.FromResult(0); // Task.CompletedTask
            }
            else if (base.CanWriteType(type))
            {
                return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
            }
            else
            {
                return Task.Factory.StartNew(() =>
                {
                    Byte[] bytes = encoding.GetBytes(WwwFormUrlEncodedMediaTypeFormatter.Serialize(value));
                    writeStream.Write(bytes, 0, bytes.Length);
                }, cancellationToken);
            }
        }

        public static String Serialize(Object value)
        {
            if (value == null) return null;

            JToken token = JToken.FromObject(value);
            List<String> pairs = new List<String>();
            WwwFormUrlEncodedMediaTypeFormatter.Flatten(pairs, token, new List<Object> { });
            return String.Join("&", pairs);
        }

        private static void Flatten(List<String> pairs, JToken input, List<Object> indices)
        {
            if (input == null) return;

            switch (input.Type)
            {
                case JTokenType.Array:
                case JTokenType.Object:
                case JTokenType.Property:
                    foreach (var token in input)
                    {
                        indices.Add(token);
                        WwwFormUrlEncodedMediaTypeFormatter.Flatten(pairs, token, indices);
                        indices.RemoveAt(indices.Count - 1);
                    }

                    break;
                default:
                    var value = input.ToObject<String>();
                    if (value == null) return;
                    pairs.Add(String.Format("{0}={1}", Uri.EscapeDataString(input.Path), Uri.EscapeDataString(value)));
                    break;
            }
        }
    }
}