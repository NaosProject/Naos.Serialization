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

    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

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
        /// Gets the serialization settings to use for writing dynamic or anonymous objects.
        /// </summary>
        private readonly JsonSerializerSettings anonymousWriteSerializationSettings;

        private readonly JsonConfigurationBase configuration;

        private readonly JsonFormattingKind formattingKind;

        /// <summary>
        /// Initializes a new instance of the <see cref="NaosJsonSerializer"/> class.
        /// </summary>
        /// <param name="configurationType">Type of configuration to use.</param>
        /// <param name="formattingKind">Type of serialization to use.</param>
        public NaosJsonSerializer(
            Type configurationType = null,
            JsonFormattingKind formattingKind = JsonFormattingKind.Default)
        {
            new { formattingKind }.Must().NotBeEqualTo(JsonFormattingKind.Invalid);
            this.formattingKind = formattingKind;

            if (configurationType != null)
            {
                configurationType.IsSubclassOf(typeof(JsonConfigurationBase)).Named(
                    Invariant($"Configuration type - {configurationType.FullName} - must derive from {nameof(JsonConfigurationBase)}.")).Must().BeTrue();

                configurationType.HasParameterlessConstructor().Named(
                    Invariant($"{nameof(configurationType)} must contain a default constructor to use in {nameof(NaosJsonSerializer)}.")).Must().BeTrue();
            }

            this.ConfigurationType = configurationType ?? typeof(NullJsonConfiguration);
            this.configuration = SerializationConfigurationManager.ConfigureWithReturn<JsonConfigurationBase>(this.ConfigurationType);
            this.anonymousWriteSerializationSettings = this.configuration.BuildAnonymousJsonSerializerSettings(SerializationDirection.Serialize, this.formattingKind);
        }

        /// <inheritdoc />
        public Type ConfigurationType { get; private set; }

        /// <inheritdoc />
        public SerializationKind Kind => SerializationKind.Json;

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
            var jsonSerializerSettings = objectToSerialize != null && objectToSerialize.GetType().IsAnonymous()
                ? this.anonymousWriteSerializationSettings
                : this.configuration.BuildJsonSerializerSettings(SerializationDirection.Serialize, this.formattingKind);

            var ret = JsonConvert.SerializeObject(objectToSerialize, jsonSerializerSettings);

            if (this.formattingKind == JsonFormattingKind.Compact)
            {
                ret = ret.Replace(Environment.NewLine, string.Empty);
            }

            return ret;
        }

        /// <inheritdoc />
        public T Deserialize<T>(string serializedString)
        {
            var jsonSerializerSettings = this.configuration.BuildJsonSerializerSettings(SerializationDirection.Deserialize, this.formattingKind);
            var ret = JsonConvert.DeserializeObject<T>(serializedString, jsonSerializerSettings);

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
                var jsonSerializerSettings = this.configuration.BuildJsonSerializerSettings(SerializationDirection.Deserialize, this.formattingKind);
                ret = JsonConvert.DeserializeObject(serializedString, type, jsonSerializerSettings);
            }

            return ret;
        }
    }
}
