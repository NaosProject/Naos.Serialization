// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerFactory.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;

    using Naos.Serialization.Domain;

    using OBeautifulCode.Type;
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Default implementation of <see cref="ISerializerFactory" />.
    /// </summary>
    public sealed class JsonSerializerFactory : ISerializerFactory
    {
        private static readonly JsonSerializerFactory InternalInstance = new JsonSerializerFactory();

        /// <summary>
        /// Gets the singleton entry point to the code.
        /// </summary>
        public static ISerializerFactory Instance => InternalInstance;

        private readonly object sync = new object();

        private JsonSerializerFactory()
        {
            /* no-op to make sure this can only be accessed via instance property */
        }

        /// <inheritdoc />
        public ISerializeAndDeserialize BuildSerializer(SerializationDescription serializationDescription, TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName, MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple, UnregisteredTypeEncounteredStrategy unregisteredTypeEncounteredStrategy = UnregisteredTypeEncounteredStrategy.Default)
        {
            new { serializationDescription }.Must().NotBeNull();

            lock (this.sync)
            {
                var configurationType = serializationDescription.ConfigurationTypeDescription?.ResolveFromLoadedTypes(typeMatchStrategy, multipleMatchStrategy);

                switch (serializationDescription.SerializationKind)
                {
                    case SerializationKind.Json: return new NaosJsonSerializer(configurationType, unregisteredTypeEncounteredStrategy);
                    default: throw new NotSupportedException(Invariant($"{nameof(serializationDescription)} from enumeration {nameof(SerializationKind)} of {serializationDescription.SerializationKind} is not supported."));
                }
            }
        }
    }
}
