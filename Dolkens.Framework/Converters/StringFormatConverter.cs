using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.Converters
{
    public class StringFormatConverter : JsonConverter
    {
        public String Format { get; set; }

        public StringFormatConverter(String format) { this.Format = format; }

        private static Dictionary<Type, MethodInfo> _toStringCache = new Dictionary<Type, MethodInfo> { };

        public override Boolean CanConvert(Type objectType)
        {
            if (!StringFormatConverter._toStringCache.ContainsKey(objectType)) StringFormatConverter._toStringCache[objectType] = objectType.GetMethod("ToString", new[] { typeof(String) });

            return StringFormatConverter._toStringCache[objectType] != null;
        }

        public override Object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Default parser");
        }

        public override Boolean CanRead
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (this.CanConvert(value.GetType()))
            {
                writer.WriteValue(_toStringCache[value.GetType()].Invoke(value, new[] { this.Format }) as String);
            }
            else
            {
                var jo = JObject.FromObject(value);
                jo.WriteTo(writer);
            }
        }
    }
}
