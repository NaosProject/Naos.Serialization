// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerFactory.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Factory
{
    using System;

    using Naos.Compression.Domain;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;

    using OBeautifulCode.TypeRepresentation;

    using Spritely.Recipes;

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

        /// <inheritdoc cref="ISerializerFactory" />
        public ISerializeAndDeserialize BuildSerializer(SerializationDescription serializationDescription, TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName, MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple)
        {
            new { serializationDescription }.Must().NotBeNull().OrThrowFirstFailure();

            lock (this.sync)
            {
                var configurationType = serializationDescription.ConfigurationTypeDescription?.ResolveFromLoadedTypes(typeMatchStrategy, multipleMatchStrategy);

                switch (serializationDescription.SerializationFormat)
                {
                    case SerializationFormat.Bson: return new NaosBsonSerializer(serializationDescription.SerializationKind, configurationType);
                    case SerializationFormat.Json: return new NaosJsonSerializer(serializationDescription.SerializationKind, configurationType);
                    default: throw new NotSupportedException(Invariant($"{nameof(serializationDescription)} from enumeration {nameof(SerializationFormat)} of {serializationDescription.SerializationFormat} is not supported."));
                }
            }
        }
    }
}
