namespace Kongrevsky.Utilities.Json.JsonConverters
{
    using System;
    using System.Globalization;
    using Kongrevsky.Utilities.Math;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class DecimalConverter : JsonConverter
    {
        public CultureInfo CultureInfo { get; set; }

        public byte Precision { get; set; } = 4;

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(decimal?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            switch (token.Type) {
                case JTokenType.Float:
                case JTokenType.Integer:
                    return token.ToObject<decimal>();
                case JTokenType.String:
                    // customize this to suit your needs
                    return Decimal.Parse(token.ToString(), CultureInfo ?? CultureInfo.CurrentCulture);
                case JTokenType.Null when objectType == typeof(decimal?):
                    return null;
                default:
                    throw new JsonSerializationException("Unexpected token type: " +
                                                         token.Type);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            decimal? d;
            if (value != null)
                d = ((decimal)value).Truncate(Precision);
            else
                d = null;
            JToken.FromObject(d).WriteTo(writer);
        }
    }
}