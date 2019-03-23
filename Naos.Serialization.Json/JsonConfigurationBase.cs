// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigurationBase.cs" company="Naos Project">
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
    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.Validation.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Base class to use for creating <see cref="NaosJsonSerializer" /> configuration.
    /// </summary>
    public abstract class JsonConfigurationBase : SerializationConfigurationBase
    {
        private static readonly IReadOnlyCollection<Type> InheritedTypeConverterBlackList =
            new[]
            {
                typeof(string),
                typeof(object),
            };

        /// <summary>
        /// Gets the optional override to the contract resolver of the settings gotten from the provided kind for reading.
        /// </summary>
        protected virtual IReadOnlyDictionary<SerializationDirection, IContractResolver> OverrideContractResolver => null;

        /// <summary>
        /// Gets the optional override to the contract resolver of the settings gotten from the provided kind for reading.
        /// </summary>
        protected virtual IReadOnlyDictionary<SerializationDirection, IReadOnlyCollection<JsonConverter>> ConvertersToPushOnStack => null;

        private static readonly IReadOnlyDictionary<SerializationDirection,
                Func<IReadOnlyCollection<Type>, JsonFormattingKind, IList<JsonConverter>>>
            GetDefaultConverters =
                new Dictionary<SerializationDirection,
                    Func<IReadOnlyCollection<Type>, JsonFormattingKind, IList<JsonConverter>>>
                {
                    {
                        SerializationDirection.Serialize,
                        (inheritedTypeConverterTypes, serializationKind) =>

                            (serializationKind == JsonFormattingKind.Minimal
                                ? new JsonConverter[0]
                                : new[] { new InheritedTypeWriterJsonConverter(inheritedTypeConverterTypes), }).Concat(
                                new JsonConverter[]
                                {
                                    new StringEnumConverter { CamelCaseText = true },
                                    new SecureStringJsonConverter(),
                                    new DictionaryJsonConverter(),
                                    new DateTimeJsonConverter(),
                                }).ToList()
                    },
                    {
                        SerializationDirection.Deserialize,
                        (inheritedTypeConverterTypes, serializationKind) => new JsonConverter[]
                        {
                            new InheritedTypeReaderJsonConverter(inheritedTypeConverterTypes),
                            new StringEnumConverter { CamelCaseText = true },
                            new SecureStringJsonConverter(),
                            new DictionaryJsonConverter(),
                            new DateTimeJsonConverter(),
                        }
                    },
                };

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

        private IReadOnlyCollection<Type> inheritedTypeConverterTypes;

        private static JsonSerializerSettings DefaultDeserializingSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
            };

        private static JsonSerializerSettings DefaultSerializingSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
            };

        private static JsonSerializerSettings CompactDeserializingSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
            };

        private static JsonSerializerSettings CompactSerializingSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
            };

        private static JsonSerializerSettings MinimalDeserializingSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
            };

        private static JsonSerializerSettings MinimalSerializingSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
            };

        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull();

            this.inheritedTypeConverterTypes = types.Where(t => !InheritedTypeConverterBlackList.Contains(t) && (t.IsAbstract || t.IsInterface || types.Any(a => a.IsAssignableTo(t)))).Distinct().ToList();
        }

        /// <summary>
        /// Build <see cref="JsonSerializerSettings" /> to use for serialization using Newtonsoft.
        /// </summary>
        /// <param name="serializationDirection">Direction of serialization.</param>
        /// <param name="formattingKind">Kind of formatting to use.</param>
        /// <returns>Prepared settings to use with Newtonsoft.</returns>
        public JsonSerializerSettings BuildJsonSerializerSettings(SerializationDirection serializationDirection, JsonFormattingKind formattingKind = JsonFormattingKind.Default)
        {
            var result = SerializationKindToSettingsSelectorByDirection[formattingKind](SerializationDirection.Serialize);

            var specifiedConverters =
                this.ConvertersToPushOnStack != null && this.ConvertersToPushOnStack.ContainsKey(serializationDirection)
                    ? this.ConvertersToPushOnStack[serializationDirection] ?? new JsonConverter[0]
                    : new JsonConverter[0];

            var defaultConverters = GetDefaultConverters[serializationDirection](this.inheritedTypeConverterTypes, formattingKind);

            var converters = new JsonConverter[0]
                .Concat(specifiedConverters)
                .Concat(defaultConverters)
                .ToList();

            result.Converters = converters;

            if (this.OverrideContractResolver != null && this.OverrideContractResolver.ContainsKey(serializationDirection))
            {
                var overrideResolver = this.OverrideContractResolver[serializationDirection];
                new { overrideResolver }.Must().NotBeNull();
                result.ContractResolver = overrideResolver;
            }

            return result;
        }

        /// <summary>
        /// Build <see cref="JsonSerializerSettings" /> to use for serialization of anonymous types using Newtonsoft.
        /// </summary>
        /// <param name="serializationDirection">Direction of serialization.</param>
        /// <param name="formattingKind">Kind of formatting to use.</param>
        /// <returns>Prepared settings to use with Newtonsoft.</returns>
        public JsonSerializerSettings BuildAnonymousJsonSerializerSettings(SerializationDirection serializationDirection, JsonFormattingKind formattingKind = JsonFormattingKind.Default)
        {
            // this is a hack to not mess with casing since the case must match for dynamic deserialization...
            var result = SerializationKindToSettingsSelectorByDirection[formattingKind](serializationDirection);
            result.ContractResolver = new DefaultContractResolver();
            result.Converters = GetDefaultConverters[serializationDirection](this.inheritedTypeConverterTypes, formattingKind);
            return result;
        }
    }

    /// <summary>
    /// Generic implementation of <see cref="JsonConfigurationBase" /> that will auto register with discovery using type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Type to auto register with discovery.</typeparam>
    public sealed class GenericJsonConfiguration<T> : JsonConfigurationBase
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegisterWithDiscovery => new[] { typeof(T) };
    }

    /// <summary>
    /// Generic implementation of <see cref="JsonConfigurationBase" /> that will auto register with discovery using type <typeparamref name="T1" />, <typeparamref name="T2" />.
    /// </summary>
    /// <typeparam name="T1">Type one to auto register with discovery.</typeparam>
    /// <typeparam name="T2">Type two to auto register with discovery.</typeparam>
    public sealed class GenericJsonConfiguration<T1, T2> : JsonConfigurationBase
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegisterWithDiscovery => new[] { typeof(T1), typeof(T2) };
    }

    /// <summary>
    /// Null implementation of <see cref="JsonConfigurationBase"/>.
    /// </summary>
    public sealed class NullJsonConfiguration : JsonConfigurationBase
    {
        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            /* no-op */
        }
    }
}