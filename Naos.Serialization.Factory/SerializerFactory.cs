// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerFactory.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Factory
{
    using System;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using Naos.Serialization.PropertyBag;

    using OBeautifulCode.Type;
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Default implementation of <see cref="ISerializerFactory" />.
    /// </summary>
    public sealed class SerializerFactory : ISerializerFactory
    {
        private static readonly SerializerFactory InternalInstance = new SerializerFactory();

        /// <summary>
        /// Gets the singleton entry point to the code.
        /// </summary>
        public static ISerializerFactory Instance => InternalInstance;

        private readonly object sync = new object();

        private SerializerFactory()
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
                    case SerializationKind.Bson: return new NaosBsonSerializer(configurationType, unregisteredTypeEncounteredStrategy);
                    case SerializationKind.Json: return new NaosJsonSerializer(configurationType, unregisteredTypeEncounteredStrategy);
                    case SerializationKind.PropertyBag: return new NaosPropertyBagSerializer(configurationType, unregisteredTypeEncounteredStrategy);
                    default: throw new NotSupportedException(Invariant($"{nameof(serializationDescription)} from enumeration {nameof(SerializationKind)} of {serializationDescription.SerializationKind} is not supported."));
                }
            }
        }
    }
}
