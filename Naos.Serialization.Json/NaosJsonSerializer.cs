// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosJsonSerializer.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Text;

    using Naos.Serialization.Domain;

    using Newtonsoft.Json;

    using Spritely.Recipes;

    /// <summary>
    /// JSON serializer.
    /// </summary>
    public sealed class NaosJsonSerializer : ISerializeAndDeserialize
    {
        /// <summary>
        /// Encoding to use for conversion in and out of bytes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is not mutable.")]
        public static readonly Encoding SerializationEncoding = Encoding.UTF8;

        private readonly JsonSerializerSettings serializationSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="NaosJsonSerializer"/> class.
        /// </summary>
        /// <param name="serializationKind">Type of serialization to use.</param>
        /// <param name="configurationType">Type of configuration to use.</param>
        public NaosJsonSerializer(SerializationKind serializationKind = SerializationKind.Default, Type configurationType = null)
        {
            new { serializationKind }.Must().NotBeEqualTo(SerializationKind.Invalid).OrThrowFirstFailure();

            this.SerializationKind = serializationKind;
            this.ConfigurationType = configurationType;

            this.serializationSettings = NewtonsoftJsonSerializerSettingsFactory.BuildSettings(this.SerializationKind, this.ConfigurationType);
        }

        /// <inheritdoc cref="IHaveSerializationKind" />
        public SerializationKind SerializationKind { get; private set; }

        /// <inheritdoc cref="IHaveConfigurationType" />
        public Type ConfigurationType { get; private set; }

        /// <summary>
        /// Converts JSON string into a byte array.
        /// </summary>
        /// <param name="json">JSON string.</param>
        /// <returns>Byte array.</returns>
        public static byte[] ConvertJsonToByteArray(string json)
        {
            var ret = SerializationEncoding.GetBytes(json);
            return ret;
        }

        /// <summary>
        /// Converts JSON byte array into a string.
        /// </summary>
        /// <param name="jsonAsBytes">JSON string as bytes.</param>
        /// <returns>JSON string.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "I like this name...")]
        public static string ConvertByteArrayToJson(byte[] jsonAsBytes)
        {
            var ret = SerializationEncoding.GetString(jsonAsBytes);
            return ret;
        }

        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public byte[] SerializeToBytes(object objectToSerialize)
        {
            var jsonString = this.SerializeToString(objectToSerialize);
            var jsonBytes = ConvertJsonToByteArray(jsonString);
            return jsonBytes;
        }

        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public T Deserialize<T>(byte[] serializedBytes)
        {
            var ret = this.Deserialize(serializedBytes, typeof(T));
            return (T)ret;
        }

        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public object Deserialize(byte[] serializedBytes, Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            var jsonString = ConvertByteArrayToJson(serializedBytes);
            return this.Deserialize(jsonString, type);
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public string SerializeToString(object objectToSerialize)
        {
            var ret = JsonConvert.SerializeObject(objectToSerialize, this.serializationSettings);

            return ret;
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public T Deserialize<T>(string serializedString)
        {
            var ret = JsonConvert.DeserializeObject<T>(serializedString, this.serializationSettings);

            return ret;
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public object Deserialize(string serializedString, Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            var ret = JsonConvert.DeserializeObject(serializedString, type, this.serializationSettings);

            return ret;
        }
    }
}
