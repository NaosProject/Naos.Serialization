﻿// --------------------------------------------------------------------------------------------------------------------
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
    public class NaosJsonSerializer : SerializerBase
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

        private readonly JsonConfigurationBase jsonConfiguration;

        private readonly JsonFormattingKind formattingKind;

        /// <summary>
        /// Initializes a new instance of the <see cref="NaosJsonSerializer"/> class.
        /// </summary>
        /// <param name="configurationType">Optional type of configuration to use; DEFAULT is none.</param>
        /// <param name="unregisteredTypeEncounteredStrategy">Optional strategy of what to do when encountering a type that has never been registered; DEFAULT is <see cref="UnregisteredTypeEncounteredStrategy.Throw" />.</param>
        /// <param name="formattingKind">Optional type of formatting to use; DEFAULT is <see cref="JsonFormattingKind.Default" />.</param>
        public NaosJsonSerializer(
            Type configurationType = null,
            UnregisteredTypeEncounteredStrategy unregisteredTypeEncounteredStrategy = UnregisteredTypeEncounteredStrategy.Default,
            JsonFormattingKind formattingKind = JsonFormattingKind.Default)
            : base(configurationType ?? typeof(NullJsonConfiguration), unregisteredTypeEncounteredStrategy)
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

            this.jsonConfiguration = (JsonConfigurationBase)this.configuration;
            this.anonymousWriteSerializationSettings = this.jsonConfiguration.BuildAnonymousJsonSerializerSettings(SerializationDirection.Serialize, this.formattingKind);
        }

        /// <inheritdoc />
        public override SerializationKind Kind => SerializationKind.Json;

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
        public override byte[] SerializeToBytes(object objectToSerialize)
        {
            var jsonString = this.SerializeToString(objectToSerialize);
            var jsonBytes = ConvertJsonToByteArray(jsonString);
            return jsonBytes;
        }

        /// <inheritdoc />
        public override T Deserialize<T>(byte[] serializedBytes)
        {
            var ret = this.Deserialize(serializedBytes, typeof(T));
            return (T)ret;
        }

        /// <inheritdoc />
        public override object Deserialize(byte[] serializedBytes, Type type)
        {
            new { type }.Must().NotBeNull();

            var jsonString = ConvertByteArrayToJson(serializedBytes);
            return this.Deserialize(jsonString, type);
        }

        /// <inheritdoc />
        public override string SerializeToString(object objectToSerialize)
        {
            var objectType = objectToSerialize?.GetType();

            this.InternalJsonThrowOnUnregisteredTypeIfAppropriate(objectType);

            var jsonSerializerSettings = objectToSerialize != null && objectType.IsAnonymous()
                ? this.anonymousWriteSerializationSettings
                : this.jsonConfiguration.BuildJsonSerializerSettings(SerializationDirection.Serialize, this.formattingKind);

            var ret = JsonConvert.SerializeObject(objectToSerialize, jsonSerializerSettings);

            if (this.formattingKind == JsonFormattingKind.Compact)
            {
                ret = ret.Replace(Environment.NewLine, string.Empty);
            }

            return ret;
        }

        /// <inheritdoc />
        public override T Deserialize<T>(string serializedString)
        {
            var objectType = typeof(T);

            this.InternalJsonThrowOnUnregisteredTypeIfAppropriate(objectType);

            var jsonSerializerSettings = this.jsonConfiguration.BuildJsonSerializerSettings(SerializationDirection.Deserialize, this.formattingKind);
            var ret = JsonConvert.DeserializeObject<T>(serializedString, jsonSerializerSettings);

            return ret;
        }

        /// <inheritdoc />
        public override object Deserialize(string serializedString, Type type)
        {
            new { type }.Must().NotBeNull();

            this.InternalJsonThrowOnUnregisteredTypeIfAppropriate(type);

            object ret;
            if (type == typeof(DynamicTypePlaceholder))
            {
                dynamic dyn = JObject.Parse(serializedString);
                ret = dyn;
            }
            else
            {
                var jsonSerializerSettings = this.jsonConfiguration.BuildJsonSerializerSettings(SerializationDirection.Deserialize, this.formattingKind);
                ret = JsonConvert.DeserializeObject(serializedString, type, jsonSerializerSettings);
            }

            return ret;
        }

        private void InternalJsonThrowOnUnregisteredTypeIfAppropriate(Type objectType)
        {
            // this type needs no registration and has value in consistent escaping/encoding...
            if (objectType != typeof(string))
            {
                this.ThrowOnUnregisteredTypeIfAppropriate(objectType);
            }
        }
    }

    /// <inheritdoc />
    public sealed class NaosJsonSerializer<TJsonConfiguration> : NaosJsonSerializer
        where TJsonConfiguration : JsonConfigurationBase, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NaosJsonSerializer{TJsonConfiguration}"/> class.
        /// </summary>
        public NaosJsonSerializer()
            : base(typeof(TJsonConfiguration))
        {
        }
    }
}
