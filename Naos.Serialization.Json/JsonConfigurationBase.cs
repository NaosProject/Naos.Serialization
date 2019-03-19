// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigurationBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using Naos.Serialization.Domain;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Base class to assist creating the correct <see cref="JsonSerializerSettings" /> to use.
    /// </summary>
    public abstract class JsonConfigurationBase
    {
        private readonly object syncConfigure = new object();

        private bool configured;

        /// <summary>
        /// Run configuration logic.
        /// </summary>
        public void Configure()
        {
            if (!this.configured)
            {
                lock (this.syncConfigure)
                {
                    if (!this.configured)
                    {
                        new { this.InheritSettingsFromKind }.Must().NotBeEqualTo(SerializationKind.Invalid);

                        var baseWriteSerializationSettings = SerializationKindToSettingsSelectorByDirection[this.InheritSettingsFromKind](SerializationDirection.Serialize);
                        var baseReadSerializationSettings = SerializationKindToSettingsSelectorByDirection[this.InheritSettingsFromKind](SerializationDirection.Deserialize);

                        if (this.OverrideWriteContractResolver != null)
                        {
                            baseWriteSerializationSettings.ContractResolver = this.OverrideWriteContractResolver;
                        }

                        if (this.OverrideReadContractResolver != null)
                        {
                            baseReadSerializationSettings.ContractResolver = this.OverrideReadContractResolver;
                        }

                        this.WriteSerializationSettings = baseWriteSerializationSettings;
                        this.ReadSerializationSettings = baseReadSerializationSettings;

                        this.configured = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the serialization settings to use for writing (must be configured first).
        /// </summary>
        public JsonSerializerSettings WriteSerializationSettings { get; private set; }

        /// <summary>
        /// Gets the serialization settings to use for reading (must be configured first).
        /// </summary>
        public JsonSerializerSettings ReadSerializationSettings { get; private set; }

        /// <summary>
        /// Gets the optional override to the contract resolver of the settings gotten from the provided kind for reading.
        /// </summary>
        protected virtual IContractResolver OverrideReadContractResolver => null;

        /// <summary>
        /// Gets the optional override to the contract resolver of the settings gotten from the provided kind for writing.
        /// </summary>
        protected virtual IContractResolver OverrideWriteContractResolver => null;

        /// <summary>
        /// Gets the kind to use as the base settings before applying overrides.
        /// </summary>
        protected abstract SerializationKind InheritSettingsFromKind { get; }

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
                                    return DefaultSerializerWriterSettings;
                                case SerializationDirection.Deserialize:
                                    return DefaultSerializerReaderSettings;
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
                                    return CompactSerializerWriterSettings;
                                case SerializationDirection.Deserialize:
                                    return CompactSerializerReaderSettings;
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
                                    return MinimalSerializerWriterSettings;
                                case SerializationDirection.Deserialize:
                                    return MinimalSerializerReaderSettings;
                                default:
                                    throw new NotSupportedException(Invariant($"Value of {nameof(direction)} - {direction} is not currently supported."));
                            }
                        }
                    },
                };

        private static JsonSerializerSettings DefaultSerializerReaderSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter { CamelCaseText = true },
                    new SecureStringJsonConverter(),
                    new DictionaryJsonConverter(),
                    new InheritedTypeReaderJsonConverter(),
                },
            };

        private static JsonSerializerSettings DefaultSerializerWriterSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter { CamelCaseText = true },
                    new SecureStringJsonConverter(),
                    new DictionaryJsonConverter(),
                    new InheritedTypeWriterJsonConverter(),
                },
            };

        private static JsonSerializerSettings CompactSerializerReaderSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter { CamelCaseText = true },
                    new SecureStringJsonConverter(),
                    new DictionaryJsonConverter(),
                    new InheritedTypeReaderJsonConverter(),
                },
            };

        private static JsonSerializerSettings CompactSerializerWriterSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter { CamelCaseText = true },
                    new SecureStringJsonConverter(),
                    new DictionaryJsonConverter(),
                    new InheritedTypeWriterJsonConverter(),
                },
            };

        private static JsonSerializerSettings MinimalSerializerReaderSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter { CamelCaseText = true },
                    new SecureStringJsonConverter(),
                    new DictionaryJsonConverter(),
                    new InheritedTypeReaderJsonConverter(),
                },
            };

        private static JsonSerializerSettings MinimalSerializerWriterSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = CamelStrictConstructorContractResolver.Instance,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter { CamelCaseText = true },
                    new SecureStringJsonConverter(),
                    new DictionaryJsonConverter(),
                    new InheritedTypeWriterJsonConverter(),
                },
            };
    }

    /// <summary>
    /// Null implementation of <see cref="JsonConfigurationBase"/>.
    /// </summary>
    public sealed class NullJsonConfiguration : JsonConfigurationBase
    {
        /// <inheritdoc cref="JsonConfigurationBase" />
        protected override SerializationKind InheritSettingsFromKind => SerializationKind.Default;
    }
}