// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializerFactory.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using OBeautifulCode.TypeRepresentation;

    /// <summary>
    /// Abstract factory interface for building serializers.
    /// </summary>
    public interface ISerializerFactory
    {
        /// <summary>
        /// Builds the correct implementation of <see cref="ISerializeAndDeserialize" /> based on the description.
        /// </summary>
        /// <param name="serializationDescription">Description of the serializer.</param>
        /// <param name="typeMatchStrategy">Optional type match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="TypeMatchStrategy.NamespaceAndName" />.</param>
        /// <param name="multipleMatchStrategy">Optional multiple match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="MultipleMatchStrategy.ThrowOnMultiple" />.</param>
        /// <returns>Correct implementation of <see cref="ISerializeAndDeserialize" /> based on the description.</returns>
        ISerializeAndDeserialize BuildSerializer(SerializationDescription serializationDescription, TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName, MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple);
    }
}