// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosJsonSerializer.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Text;

    using Naos.Serialization.Domain;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.Validation.Recipes;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="NaosJsonSerializer"/> class.
        /// </summary>
        /// <param name="serializationKind">Type of serialization to use.</param>
        /// <param name="configurationType">Type of configuration to use.</param>
        public NaosJsonSerializer(SerializationKind serializationKind = SerializationKind.Default, Type configurationType = null)
        {
            new { serializationKind }.Must().NotBeEqualTo(SerializationKind.Invalid);

            this.SerializationKind = serializationKind;
            this.ConfigurationType = configurationType;

            this.SerializationSettings = NewtonsoftJsonSerializerSettingsFactory.BuildSettings(this.SerializationKind, this.ConfigurationType);
        }

        /// <inheritdoc />
        public SerializationKind SerializationKind { get; private set; }

        /// <inheritdoc />
        public Type ConfigurationType { get; private set; }

        /// <summary>
        /// Gets the serialization settings to use.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; private set; }

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

        /// <inheritdoc />
        public byte[] SerializeToBytes(object objectToSerialize)
        {
            var jsonString = this.SerializeToString(objectToSerialize);
            var jsonBytes = ConvertJsonToByteArray(jsonString);
            return jsonBytes;
        }

        /// <inheritdoc />
        public T Deserialize<T>(byte[] serializedBytes)
        {
            var ret = this.Deserialize(serializedBytes, typeof(T));
            return (T)ret;
        }

        /// <inheritdoc />
        public object Deserialize(byte[] serializedBytes, Type type)
        {
            new { type }.Must().NotBeNull();

            var jsonString = ConvertByteArrayToJson(serializedBytes);
            return this.Deserialize(jsonString, type);
        }

        /// <inheritdoc />
        public string SerializeToString(object objectToSerialize)
        {
            var localSerializationSettings = this.SerializationSettings;
            if (objectToSerialize != null && objectToSerialize.GetType().IsAnonymous())
            {
                // this is a hack to not mess with casing since the case must match for dynamic deserialization...
                localSerializationSettings = NewtonsoftJsonSerializerSettingsFactory.BuildSettings(this.SerializationKind, this.ConfigurationType);
                localSerializationSettings.ContractResolver = new DefaultContractResolver();
            }

            var ret = JsonConvert.SerializeObject(objectToSerialize, localSerializationSettings);

            return ret;
        }

        /// <inheritdoc />
        public T Deserialize<T>(string serializedString)
        {
            var ret = JsonConvert.DeserializeObject<T>(serializedString, this.SerializationSettings);

            return ret;
        }

        /// <inheritdoc />
        public object Deserialize(string serializedString, Type type)
        {
            new { type }.Must().NotBeNull();

            object ret;
            if (type == typeof(DynamicTypePlaceholder))
            {
                dynamic dyn = JObject.Parse(serializedString);
                ret = dyn;
            }
            else
            {
                ret = JsonConvert.DeserializeObject(serializedString, type, this.SerializationSettings);
            }

            return ret;
        }
    }
}
