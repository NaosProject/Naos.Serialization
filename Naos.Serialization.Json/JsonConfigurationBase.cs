// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigurationBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Concurrent;
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
        /// Inherited types to handle (static and used by all configurations to accomodate the fact that you have dependent configs that are only run once).
        /// </summary>
        private static readonly Tracker<Type> InheritedTypesToHandle = new Tracker<Type>((a, b) => a == b);

        /// <summary>
        /// Registered converters to use when serializing (static and used by all configurations to accomodate the fact that you have dependent configs that are only run once).
        /// </summary>
        private static readonly Tracker<RegisteredJsonConverter> RegisteredSerializingConverters = new Tracker<RegisteredJsonConverter>((a, b) => a == b);

        /// <summary>
        /// Registered converters to use when deserializing (static and used by all configurations to accomodate the fact that you have dependent configs that are only run once).
        /// </summary>
        private static readonly Tracker<RegisteredJsonConverter> RegisteredDeserializingConverters = new Tracker<RegisteredJsonConverter>((a, b) => a == b);

        /// <summary>
        /// Gets the optional override to the contract resolver of the settings gotten from the provided kind for reading.
        /// </summary>
        protected virtual IReadOnlyDictionary<SerializationDirection, IContractResolver> OverrideContractResolver => null;

        /// <summary>
        /// Gets the optional override to the contract resolver of the settings gotten from the provided kind for reading.
        /// </summary>
        protected virtual IReadOnlyDictionary<SerializationDirection, IReadOnlyCollection<RegisteredJsonConverter>> ConvertersToPushOnStack => null;

        private static readonly IReadOnlyDictionary<SerializationDirection,
                Func<JsonFormattingKind, IList<JsonConverter>>>
            GetDefaultConverters =
                new Dictionary<SerializationDirection,
                    Func<JsonFormattingKind, IList<JsonConverter>>>
                {
                    {
                        SerializationDirection.Serialize,
                        formattingKind =>
                        {
                            var inheritedTypesToHandle = InheritedTypesToHandle.GetAllTrackedObjects();
                            var typesThatConvertToString = RegisteredSerializingConverters
                                .GetAllTrackedObjects()
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
                                    : new[] { new InheritedTypeWriterJsonConverter(inheritedTypesToHandle) })
                                .Concat(
                                    new JsonConverter[]
                                    {
                                        new DictionaryJsonConverter(typesThatConvertToString),
                                        new KeyValueArrayDictionaryJsonConverter(typesThatConvertToString),
                                    }).ToList();
                        }
                    },
                    {
                        SerializationDirection.Deserialize,
                        serializationKind =>
                        {
                            var inheritedTypesToHandle = InheritedTypesToHandle.GetAllTrackedObjects();
                            var typesThatConvertToString = RegisteredSerializingConverters
                                .GetAllTrackedObjects()
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

        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull();

            var inheritedTypeConverterTypes = types.Where(t =>
                !InheritedTypeConverterBlackList.Contains(t) &&
                (t.IsAbstract || t.IsInterface || types.Any(a => a.IsAssignableTo(t)))).Distinct().ToList();

            inheritedTypeConverterTypes.ForEach(
                _ => InheritedTypesToHandle.RunTrackedOperation(
                    _,
                    InheritedTypesToHandle.NullTrackedOperation,
                    this.TypeTrackerCollisionStrategy,
                    this.GetType()));
        }

        /// <inheritdoc />
        protected override void InternalConfigure()
        {
            var nullRegisteredConverterMap =
                new Dictionary<SerializationDirection, IReadOnlyCollection<RegisteredJsonConverter>>
                {
                    { SerializationDirection.Serialize, new RegisteredJsonConverter[0] },
                    { SerializationDirection.Deserialize, new RegisteredJsonConverter[0] },
                };

            ((this.ConvertersToPushOnStack ??
              nullRegisteredConverterMap)
             [SerializationDirection.Serialize] ?? new RegisteredJsonConverter[0]).ToList()
                .ForEach(
                    _ => RegisteredSerializingConverters.RunTrackedOperation(
                        _,
                        RegisteredSerializingConverters.NullTrackedOperation,
                        this.TypeTrackerCollisionStrategy,
                        this.GetType()));

            ((this.ConvertersToPushOnStack ??
              nullRegisteredConverterMap)
             [SerializationDirection.Deserialize] ?? new RegisteredJsonConverter[0]).ToList()
                .ForEach(
                    _ => RegisteredDeserializingConverters.RunTrackedOperation(
                        _,
                        RegisteredDeserializingConverters.NullTrackedOperation,
                        this.TypeTrackerCollisionStrategy,
                        this.GetType()));
        }

        /// <summary>
        /// Build <see cref="JsonSerializerSettings" /> to use for serialization using Newtonsoft.
        /// </summary>
        /// <param name="serializationDirection">Direction of serialization.</param>
        /// <param name="formattingKind">Kind of formatting to use.</param>
        /// <returns>Prepared settings to use with Newtonsoft.</returns>
        public JsonSerializerSettings BuildJsonSerializerSettings(SerializationDirection serializationDirection, JsonFormattingKind formattingKind = JsonFormattingKind.Default)
        {
            (serializationDirection == SerializationDirection.Serialize || serializationDirection == SerializationDirection.Deserialize)
                .Named(Invariant($"{nameof(serializationDirection)}-must-be-{nameof(SerializationDirection.Serialize)}-or{nameof(SerializationDirection.Serialize)}"))
                .Must().BeTrue();

            var result = SerializationKindToSettingsSelectorByDirection[formattingKind](SerializationDirection.Serialize);

            var specifiedConverters = serializationDirection == SerializationDirection.Serialize
                ? RegisteredSerializingConverters.GetAllTrackedObjects().Select(_ => _.ConverterBuilderFunction()).ToList()
                : RegisteredDeserializingConverters.GetAllTrackedObjects().Select(_ => _.ConverterBuilderFunction()).ToList();

            var defaultConverters = GetDefaultConverters[serializationDirection](formattingKind);

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
            result.Converters = GetDefaultConverters[serializationDirection](formattingKind);
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