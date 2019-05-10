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

    /// <summary>
    /// Base class to use for creating <see cref="NaosJsonSerializer" /> configuration.
    /// </summary>
    public abstract partial class JsonConfigurationBase : SerializationConfigurationBase
    {
        private static readonly IReadOnlyCollection<Type> InheritedTypeConverterBlackList =
            new[]
            {
                typeof(string),
                typeof(object),
            };

        /// <inheritdoc />
        public sealed override IReadOnlyCollection<Type> InternalDependentConfigurationTypes => new[] { typeof(InternalJsonConfiguration) };

        /// <summary>
        /// Gets the types that have been registered with a converter.
        /// </summary>
        protected HashSet<Type> TypesWithConverters { get; } = new HashSet<Type>();

        /// <summary>
        /// Gets the registered converter set to use.
        /// </summary>
        protected IList<RegisteredJsonConverter> RegisteredConverters { get; } = new List<RegisteredJsonConverter>();

        /// <summary>
        /// Gets the inherited types to handle.
        /// </summary>
        protected HashSet<Type> InheritedTypesToHandle { get; } = new HashSet<Type>();

        /// <summary>
        /// Gets the types with registered converters that yield a string as the output (this allows for standard use as a key in a dictionary).
        /// </summary>
        protected HashSet<Type> TypesWithStringConverters { get; } = new HashSet<Type>();
    }

    /// <summary>
    /// Internal implementation of <see cref="JsonConfigurationBase" /> that will auto register necessary internal types.
    /// </summary>
    public sealed class InternalJsonConfiguration : JsonConfigurationBase, IDoNotNeedInternalDependencies
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegisterWithDiscovery => InternallyRequiredTypes;
    }

    /// <summary>
    /// Generic implementation of <see cref="JsonConfigurationBase" /> that will auto register with discovery using type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Type to auto register with discovery.</typeparam>
    public sealed class GenericDiscoveryJsonConfiguration<T> : JsonConfigurationBase
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegisterWithDiscovery => new[] { typeof(T) };
    }

    /// <summary>
    /// Generic implementation of <see cref="JsonConfigurationBase" /> that will auto register with discovery using type <typeparamref name="T1" />, <typeparamref name="T2" />.
    /// </summary>
    /// <typeparam name="T1">Type one to auto register with discovery.</typeparam>
    /// <typeparam name="T2">Type two to auto register with discovery.</typeparam>
    public sealed class GenericDiscoveryJsonConfiguration<T1, T2> : JsonConfigurationBase
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegisterWithDiscovery => new[] { typeof(T1), typeof(T2) };
    }

    /// <summary>
    /// Null implementation of <see cref="JsonConfigurationBase"/>.
    /// </summary>
    public sealed class NullJsonConfiguration : JsonConfigurationBase, IImplementNullObjectPattern
    {
    }
}