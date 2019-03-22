// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeJsonConverter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Linq;
    using Naos.Serialization.Domain;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Custom <see cref="DateTime"/> converter to do the right thing.
    /// </summary>
    internal class DateTimeJsonConverter : JsonConverter
    {
        private static readonly Type[] SupportedDateTimeTypes = new[] { typeof(DateTime), typeof(DateTime?) };

        private static readonly IStringSerializeAndDeserialize UnderlyingSerializer = new NaosDateTimeStringSerializer();

        /// <inheritdoc />
        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            var payload = value == null ? null : UnderlyingSerializer.SerializeToString(value);

            var payloadObject = new JValue(payload);
            payloadObject.WriteTo(writer);
        }

        /// <inheritdoc />
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var payload = reader.Value;
            var result = payload == null ? null : UnderlyingSerializer.Deserialize(payload.ToString(), objectType);

            return result;
        }

        /// <inheritdoc />
        public override bool CanConvert(
            Type objectType)
        {
            bool result;

            if (objectType == null)
            {
                result = false;
            }
            else
            {
                result = SupportedDateTimeTypes.Contains(objectType);
            }

            return result;
        }
    }
}
