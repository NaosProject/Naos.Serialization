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
        /// <summary>
        /// Gets the kind to use as the base settings before applying overrides.
        /// </summary>
        protected virtual SerializationKind InheritSettingsFromKind => SerializationKind.Default;

        /// <summary>
        /// Gets the optional override to the contract resolver of the settings gotten from the provided kind for reading.
        /// </summary>
        protected virtual IReadOnlyDictionary<SerializationDirection, IContractResolver> OverrideContractResolver => null;

        /// <summary>
        /// Gets the optional override to the contract resolver of the settings gotten from the provided kind for reading.
        /// </summary>
        protected virtual IReadOnlyDictionary<SerializationDirection, IReadOnlyCollection<JsonConverter>> ConvertersToPushOnStack => null;

        private static readonly IReadOnlyDictionary<SerializationDirection, Func<IReadOnlyCollection<Type>, IList<JsonConverter>>>
            GetDefaultConverters =
                new Dictionary<SerializationDirection, Func<IReadOnlyCollection<Type>, IList<JsonConverter>>>
                {
                    {
                        SerializationDirection.Serialize,
                        inheritedTypeConverterTypes => new JsonConverter[]
                        {
                            new InheritedTypeWriterJsonConverter(inheritedTypeConverterTypes),
                            new StringEnumConverter { CamelCaseText = true },
                            new SecureStringJsonConverter(),
                            new DictionaryJsonConverter(),
                            new DateTimeJsonConverter(),
                        }
                    },
                    {
                        SerializationDirection.Deserialize,
                        inheritedTypeConverterTypes => new JsonConverter[]
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
        /// Map of <see cref="SerializationKind" /> to a <see cref="Func{T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult}" /> that will take a <see cref="SerializationDirection" /> and return the correct <see cref="JsonSerializerSettings" />.
        /// </summary>
        internal static readonly Dictionary<SerializationKind, Func<SerializationDirection, JsonSerializerSettings>>
            SerializationKindToSettingsSelectorByDirection =
                new Dictionary<SerializationKind, Func<SerializationDirection, JsonSerializerSettings>>
                {
                    {
                        SerializationKind.Default, direction =>
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
                        SerializationKind.Compact, direction =>
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
                        SerializationKind.Minimal, direction =>
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
        protected override void InternalConfigure()
        {
            new { this.InheritSettingsFromKind }.Must().NotBeEqualTo(SerializationKind.Invalid);
        }

        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull();

            this.inheritedTypeConverterTypes = types.Where(t => types.Any(a => a.IsAssignableTo(t))).ToList();
        }

        /// <summary>
        /// Build <see cref="JsonSerializerSettings" /> to use for serialization using Newtonsoft.
        /// </summary>
        /// <param name="serializationDirection">Direction of serialization.</param>
        /// <returns>Prepared settings to use with Newtonsoft.</returns>
        public JsonSerializerSettings BuildJsonSerializerSettings(SerializationDirection serializationDirection)
        {
            var result = SerializationKindToSettingsSelectorByDirection[this.InheritSettingsFromKind](SerializationDirection.Serialize);

            var specifiedConverters =
                this.ConvertersToPushOnStack != null && this.ConvertersToPushOnStack.ContainsKey(serializationDirection)
                    ? this.ConvertersToPushOnStack[serializationDirection] ?? new JsonConverter[0]
                    : new JsonConverter[0];

            var defaultConverters = GetDefaultConverters[serializationDirection](this.inheritedTypeConverterTypes);

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
        /// <param name="serializationKind">Kind of serialization.</param>
        /// <param name="serializationDirection">Direction of serialization.</param>
        /// <returns>Prepared settings to use with Newtonsoft.</returns>
        public JsonSerializerSettings BuildAnonymousJsonSerializerSettings(SerializationKind serializationKind, SerializationDirection serializationDirection)
        {
            // this is a hack to not mess with casing since the case must match for dynamic deserialization...
            var result = SerializationKindToSettingsSelectorByDirection[serializationKind](serializationDirection);
            result.ContractResolver = new DefaultContractResolver();
            result.Converters = GetDefaultConverters[serializationDirection](this.inheritedTypeConverterTypes);
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