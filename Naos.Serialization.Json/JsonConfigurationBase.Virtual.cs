// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigurationBase.Virtual.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System.Collections.Generic;
    using Naos.Serialization.Domain;

    /// <summary>
    /// Base class to use for creating <see cref="NaosJsonSerializer" /> configuration.
    /// </summary>
    public abstract partial class JsonConfigurationBase
    {
        /// <summary>
        /// Gets the optional override to the contract resolver of the settings gotten from the provided kind for reading.
        /// </summary>
        protected virtual IReadOnlyDictionary<SerializationDirection, RegisteredContractResolver> OverrideContractResolver => null;

        /// <summary>
        /// Gets the optional converters to add.
        /// </summary>
        protected virtual IReadOnlyCollection<RegisteredJsonConverter> ConvertersToRegister => null;
    }
}