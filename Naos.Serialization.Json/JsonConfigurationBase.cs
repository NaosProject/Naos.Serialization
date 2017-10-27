// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigurationBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;

    using Naos.Serialization.Domain;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Spritely.Recipes;

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
                        new { this.InheritSettingsFromKind }.Must().NotBeEqualTo(SerializationKind.Invalid).OrThrowFirstFailure();

                        var baseSettings = GetSettingsBySerializationKind(this.InheritSettingsFromKind);

                        if (this.OverrideContractResolver != null)
                        {
                            baseSettings.ContractResolver = this.OverrideContractResolver;
                        }

                        this.SerializationSettings = baseSettings;

                        this.configured = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the serialization settings (must be configurated first).
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets the optional override to the contract resolver of the settings gotten from the provided kind.
        /// </summary>
        protected virtual IContractResolver OverrideContractResolver => null;

        /// <summary>
        /// Gets the kind to use as the base settings before applying overrides.
        /// </summary>
        protected abstract SerializationKind InheritSettingsFromKind { get; }

        /// <summary>
        /// Gets the settings to use from the <see cref="SerializationKind" /> provided.
        /// </summary>
        /// <param name="serializationKind">Kind to determine the settings.</param>
        /// <returns><see cref="JsonSerializerSettings" /> to use with <see cref="Newtonsoft" /> when serializing.</returns>
        public static JsonSerializerSettings GetSettingsBySerializationKind(SerializationKind serializationKind)
        {
            switch (serializationKind)
            {
                case SerializationKind.Default: return JsonConfiguration.DefaultSerializerSettings;
                case SerializationKind.Compact: return JsonConfiguration.CompactSerializerSettings;
                case SerializationKind.Minimal: return JsonConfiguration.MinimalSerializerSettings;
                default: throw new NotSupportedException(Invariant($"Value of {nameof(SerializationKind)} - {serializationKind} is not currently supported."));
            }
        }
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