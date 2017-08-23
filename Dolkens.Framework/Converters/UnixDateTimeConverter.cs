using Dolkens.Framework.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.Converters
{
    public class UnixDateTimeConverter : DateTimeConverterBase
    {
        public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            var nullable = objectType.GetType().IsNullable();

            if (reader.TokenType == JsonToken.Null)
            {
                if (!nullable) throw new JsonSerializationException($"Cannot convert null value to {objectType}.");

                return null;
            }

            objectType = nullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            if (reader.TokenType == JsonToken.Integer)
            {
                if (reader.ValueType == typeof(UInt64))
                {
                    return DateTimeUtilities.EPOCH.AddMilliseconds((UInt64)reader.Value).ToLocalTime();
                }
                else
                {
                    return DateTimeUtilities.EPOCH.AddMilliseconds((Int64)reader.Value).ToLocalTime();
                }
            }

            if (reader.TokenType == JsonToken.Float)
            {
                return DateTimeUtilities.EPOCH.AddMilliseconds((Double)reader.Value).ToLocalTime();
            }

            if (reader.TokenType == JsonToken.String)
            {
                Int64 int64 = 0;
                UInt64 uint64 = 0;
                Double value = 0;

                if (Int64.TryParse((String)reader.Value, out int64))
                {
                    return DateTimeUtilities.EPOCH.AddMilliseconds(int64).ToLocalTime();
                }
                else if (UInt64.TryParse((String)reader.Value, out uint64))
                {
                    return DateTimeUtilities.EPOCH.AddMilliseconds(uint64).ToLocalTime();
                }
                else if (Double.TryParse((String)reader.Value, out value))
                {
                    return DateTimeUtilities.EPOCH.AddMilliseconds(value).ToLocalTime();
                }
                else
                {
                    throw new JsonSerializationException($"Cannot convert value to {objectType}.");
                }
            }

            throw new JsonSerializationException($"Unexpected token parsing date. Expected Integer, got {reader.TokenType}.");
        }

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            String text;

            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;

                text = $"{(dateTime - DateTimeUtilities.EPOCH).TotalMilliseconds}";
            }

            else if (value is DateTimeOffset)
            {
                DateTimeOffset dateTime = (DateTimeOffset)value;

                text = $"{(dateTime - DateTimeUtilities.EPOCH).TotalMilliseconds}";
            }

            else
            {
                throw new JsonSerializationException($"Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {value.GetType()}.");
            }

            writer.WriteValue(text);
        }
    }
}
