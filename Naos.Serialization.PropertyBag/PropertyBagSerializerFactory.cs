// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBagSerializerFactory.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.PropertyBag
{
    using System;
    using Naos.Serialization.Domain;
    using OBeautifulCode.Representation;
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Default implementation of <see cref="ISerializerFactory" />.
    /// </summary>
    public sealed class PropertyBagSerializerFactory : ISerializerFactory
    {
        private static readonly PropertyBagSerializerFactory InternalInstance = new PropertyBagSerializerFactory();

        /// <summary>
        /// Gets the singleton entry point to the code.
        /// </summary>
        public static ISerializerFactory Instance => InternalInstance;

        private readonly object sync = new object();

        private PropertyBagSerializerFactory()
        {
            /* no-op to make sure this can only be accessed via instance property */
        }

        /// <inheritdoc />
        public ISerializeAndDeserialize BuildSerializer(SerializationDescription serializationDescription, TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName, MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple, UnregisteredTypeEncounteredStrategy unregisteredTypeEncounteredStrategy = UnregisteredTypeEncounteredStrategy.Default)
        {
            new { serializationDescription }.Must().NotBeNull();

            lock (this.sync)
            {
                var configurationType = serializationDescription.ConfigurationTypeRepresentation?.ResolveFromLoadedTypes(typeMatchStrategy, multipleMatchStrategy);

                switch (serializationDescription.SerializationKind)
                {
                    case SerializationKind.PropertyBag: return new NaosPropertyBagSerializer(configurationType, unregisteredTypeEncounteredStrategy);
                    default: throw new NotSupportedException(Invariant($"{nameof(serializationDescription)} from enumeration {nameof(SerializationKind)} of {serializationDescription.SerializationKind} is not supported."));
                }
            }
        }
    }
}
