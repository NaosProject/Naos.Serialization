// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryExtensions.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Factory.Extensions
{
    using Naos.Compression.Domain;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Domain.Extensions;

    using OBeautifulCode.TypeRepresentation;

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class FactoryExtensions
    {
        /// <summary>
        /// Converts an object to a self described serialization to persist or share.
        /// </summary>
        /// <param name="objectToPackageIntoDescribedSerialization">Object to serialize.</param>
        /// <param name="serializationDescription">Description of the serializer to use.</param>
        /// <param name="typeMatchStrategy">Optional type match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="TypeMatchStrategy.NamespaceAndName" />.</param>
        /// <param name="multipleMatchStrategy">Optional multiple match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="MultipleMatchStrategy.ThrowOnMultiple" />.</param>
        /// <returns>Self decribed serialization.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "Spelling/name is correct.")]
        public static DescribedSerialization ToDescribedSerialization(
            this object objectToPackageIntoDescribedSerialization,
            SerializationDescription serializationDescription,
            TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName,
            MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple)
        {
            return DomainExtensions.ToDescribedSerializationUsingSpecificFactory(
                objectToPackageIntoDescribedSerialization,
                serializationDescription,
                SerializerFactory.Instance,
                CompressorFactory.Instance,
                typeMatchStrategy,
                multipleMatchStrategy);
        }

        /// <summary>
        /// Converts a self described serialization back into it's object.
        /// </summary>
        /// <param name="describedSerialization">Self described serialized object.</param>
        /// <param name="typeMatchStrategy">Optional type match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="TypeMatchStrategy.NamespaceAndName" />.</param>
        /// <param name="multipleMatchStrategy">Optional multiple match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="MultipleMatchStrategy.ThrowOnMultiple" />.</param>
        /// <returns>Orginally serialized object.</returns>
        public static object DeserializePayload(this DescribedSerialization describedSerialization, TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName, MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple)
        {
            return DomainExtensions.DeserializePayloadUsingSpecificFactory(
                describedSerialization,
                SerializerFactory.Instance,
                CompressorFactory.Instance,
                typeMatchStrategy,
                multipleMatchStrategy);
        }

        /// <summary>
        /// Converts a self described serialization back into it's object.
        /// </summary>
        /// <param name="describedSerialization">Self described serialized object.</param>
        /// <param name="typeMatchStrategy">Optional type match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="TypeMatchStrategy.NamespaceAndName" />.</param>
        /// <param name="multipleMatchStrategy">Optional multiple match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="MultipleMatchStrategy.ThrowOnMultiple" />.</param>
        /// <typeparam name="T">Expected return type.</typeparam>
        /// <returns>Orginally serialized object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Checked with Must and tested.")]
        public static T DeserializePayload<T>(this DescribedSerialization describedSerialization, TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName, MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple)
        {
            return DomainExtensions.DeserializePayloadUsingSpecificFactory<T>(
                describedSerialization,
                SerializerFactory.Instance,
                CompressorFactory.Instance,
                typeMatchStrategy,
                multipleMatchStrategy);
        }
    }
}