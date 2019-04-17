// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigurationBase.Newtonsoft.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Naos.Serialization.Domain;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Base class to use for creating <see cref="NaosJsonSerializer" /> configuration.
    /// </summary>
    public abstract partial class JsonConfigurationBase
    {
        /// <summary>
        /// Build <see cref="JsonSerializerSettings" /> to use for serialization using Newtonsoft.
        /// </summary>
        /// <param name="serializationDirection">Direction of serialization.</param>
        /// <param name="formattingKind">Kind of formatting to use.</param>
        /// <returns>Prepared settings to use with Newtonsoft.</returns>
        public JsonSerializerSettings BuildJsonSerializerSettings(
            SerializationDirection serializationDirection,
            JsonFormattingKind formattingKind = JsonFormattingKind.Default)
        {
            (serializationDirection == SerializationDirection.Serialize || serializationDirection == SerializationDirection.Deserialize)
                .Named(Invariant($"{nameof(serializationDirection)}-must-be-{nameof(SerializationDirection.Serialize)}-or{nameof(SerializationDirection.Serialize)}"))
                .Must().BeTrue();

            var result = SerializationKindToSettingsSelectorByDirection[formattingKind](SerializationDirection.Serialize);

            var specifiedConverters = serializationDirection == SerializationDirection.Serialize
                ? this.RegisteredSerializingConverters.Select(_ => _.ConverterBuilderFunction()).ToList()
                : this.RegisteredDeserializingConverters.Select(_ => _.ConverterBuilderFunction()).ToList();

            var defaultConverters = this.GetDefaultConverters(serializationDirection, formattingKind);

            var converters = new JsonConverter[0]
                .Concat(specifiedConverters)
                .Concat(defaultConverters)
                .ToList();

            result.Converters = converters;

            if (this.OverrideContractResolver != null && this.OverrideContractResolver.ContainsKey(serializationDirection))
            {
                var overrideResolver = this.OverrideContractResolver[serializationDirection];
                new { overrideResolver }.Must().NotBeNull();
                result.ContractResolver = overrideResolver.ContractResolverBuilderFunction();
            }

            return result;
        }

        /// <summary>
        /// Build <see cref="JsonSerializerSettings" /> to use for serialization of anonymous types using Newtonsoft.
        /// </summary>
        /// <param name="serializationDirection">Direction of serialization.</param>
        /// <param name="formattingKind">Kind of formatting to use.</param>
        /// <returns>Prepared settings to use with Newtonsoft.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Keeping like other to offer option in the future of access to this.")]
        public JsonSerializerSettings BuildAnonymousJsonSerializerSettings(
            SerializationDirection serializationDirection,
            JsonFormattingKind formattingKind = JsonFormattingKind.Default)
        {
            // this is a hack to not mess with casing since the case must match for dynamic deserialization...
            var result = SerializationKindToSettingsSelectorByDirection[formattingKind](serializationDirection);
            result.ContractResolver = new DefaultContractResolver();
            result.Converters = this.GetDefaultConverters(serializationDirection, formattingKind);
            return result;
        }

        private IList<JsonConverter> GetDefaultConverters(SerializationDirection serializationDirection, JsonFormattingKind formattingKind)
        {
            switch (serializationDirection)
            {
                case SerializationDirection.Serialize:
                    return this.GetDefaultSerializingConverters(formattingKind);
                case SerializationDirection.Deserialize:
                    return this.GetDefaultDeserializingConverters();
                default:
                    throw new NotSupportedException(Invariant($"{nameof(SerializationDirection)} value {serializationDirection} is not supported."));
            }
        }

        private IList<JsonConverter> GetDefaultDeserializingConverters()
        {
            var inheritedTypesToHandle = this.InheritedTypesToHandle
                .Except(this.RegisteredDeserializingConverters.SelectMany(_ => _.HandledTypes)).ToList();

            var typesThatConvertToString = this.RegisteredSerializingConverters
                .Where(_ => _.OutputKind == RegisteredJsonConverterOutputKind.String)
                .SelectMany(_ => _.HandledTypes).Distinct().ToList();

            return new JsonConverter[0].Concat(
                new JsonConverter[]
                {
                    new DateTimeJsonConverter(),
                    new StringEnumConverter { CamelCaseText = true },
                    new SecureStringJsonConverter(),
                    new InheritedTypeReaderJsonConverter(inheritedTypesToHandle),
                    new DictionaryJsonConverter(typesThatConvertToString),
                    new KeyValueArrayDictionaryJsonConverter(typesThatConvertToString),
                }).ToList();
        }

        private IList<JsonConverter> GetDefaultSerializingConverters(JsonFormattingKind formattingKind)
        {
            var typesThatConvertToString = this.RegisteredSerializingConverters
                .Where(_ => _.OutputKind == RegisteredJsonConverterOutputKind.String)
                .SelectMany(_ => _.HandledTypes).Distinct().ToList();

            return new JsonConverter[0].Concat(
                    new JsonConverter[]
                    {
                        new DateTimeJsonConverter(),
                        new StringEnumConverter { CamelCaseText = true },
                        new SecureStringJsonConverter(),
                    }).Concat(formattingKind == JsonFormattingKind.Minimal
                    ? new JsonConverter[0]
                    : new[] { new InheritedTypeWriterJsonConverter(this.InheritedTypesToHandle) })
                .Concat(
                    new JsonConverter[]
                    {
                        new DictionaryJsonConverter(typesThatConvertToString),
                        new KeyValueArrayDictionaryJsonConverter(typesThatConvertToString),
                    }).ToList();
        }

        /// <summary>
        /// Map of <see cref="JsonFormattingKind" /> to a <see cref="Func{T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult}" /> that will take a <see cref="SerializationDirection" /> and return the correct <see cref="JsonSerializerSettings" />.
        /// </summary>
        internal static readonly Dictionary<JsonFormattingKind, Func<SerializationDirection, JsonSerializerSettings>>
            SerializationKindToSettingsSelectorByDirection =
                new Dictionary<JsonFormattingKind, Func<SerializationDirection, JsonSerializerSettings>>
                {
                    {
                        JsonFormattingKind.Default, direction =>
                        {
                            switch (direction)
                            {
                                case SerializationDirection.Serialize:
                                    return DefaultSerializingSettings;
                                case SerializationDirection.Deserialize:
                                    return DefaultDeserializingSettings;
                                default:
                                    throw new NotSupportedException(Invariant($"Value of {nameof(direction)} - {direction} is not currently supported."));
                            }
                        }
                    },
                    {
                        JsonFormattingKind.Compact, direction =>
                        {
                            switch (direction)
                            {
                                case SerializationDirection.Serialize:
                                    return CompactSerializingSettings;
                                case SerializationDirection.Deserialize:
                                    return CompactDeserializingSettings;
                                default:
                                    throw new NotSupportedException(Invariant($"Value of {nameof(direction)} - {direction} is not currently supported."));
                            }
                        }
                    },
                    {
                        JsonFormattingKind.Minimal, direction =>
                        {
                            switch (direction)
                            {
                                case SerializationDirection.Serialize:
                                    return MinimalSerializingSettings;
                                case SerializationDirection.Deserialize:
                                    return MinimalDeserializingSettings;
                                default:
                                    throw new NotSupportedException(Invariant($"Value of {nameof(direction)} - {direction} is not currently supported."));
                            }
                        }
                    },
                };

        private static JsonSerializerSettings DefaultDeserializingSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
                DateParseHandling = DateParseHandling.None,
                FloatParseHandling = FloatParseHandling.Decimal,
            };

        private static JsonSerializerSettings DefaultSerializingSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
                DateParseHandling = DateParseHandling.None,
                FloatParseHandling = FloatParseHandling.Decimal,
            };

        private static JsonSerializerSettings CompactDeserializingSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
                DateParseHandling = DateParseHandling.None,
                FloatParseHandling = FloatParseHandling.Decimal,
            };

        private static JsonSerializerSettings CompactSerializingSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
                DateParseHandling = DateParseHandling.None,
                FloatParseHandling = FloatParseHandling.Decimal,
            };

        private static JsonSerializerSettings MinimalDeserializingSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
                DateParseHandling = DateParseHandling.None,
                FloatParseHandling = FloatParseHandling.Decimal,
            };

        private static JsonSerializerSettings MinimalSerializingSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
                DateParseHandling = DateParseHandling.None,
                FloatParseHandling = FloatParseHandling.Decimal,
            };
    }
}