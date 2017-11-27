// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashAlgorithmNameJsonConverter.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Spritely.Recipes
{
    using System;
    using System.Security.Cryptography;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class that enables the Json serializer to construct <see cref="HashAlgorithmName"/> instances.
    /// </summary>
    internal class HashAlgorithmNameJsonConverter : JsonConverter
    {
        private const string DefaultHashAlgorithmNameStringValue = "default";

        /// <summary>
        ///     Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        ///     <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HashAlgorithmName);
        }

        /// <summary>
        ///     Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        ///     The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var stringVersion = reader.Value.ToString();
            if (stringVersion == DefaultHashAlgorithmNameStringValue)
            {
                return default(HashAlgorithmName);
            }
            else
            {
                return new HashAlgorithmName(stringVersion);
            }
        }

        /// <summary>
        ///     Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (value != null)
            {
                var valueAsHashAlgorithmName = (HashAlgorithmName)value;
                var stringValueToWrite = valueAsHashAlgorithmName == default(HashAlgorithmName) ? DefaultHashAlgorithmNameStringValue : value.ToString();
                writer.WriteValue(stringValueToWrite);
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}
