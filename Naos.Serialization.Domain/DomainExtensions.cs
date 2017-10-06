// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainExtensions.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain.Extensions
{
    using System;

    using Naos.Compression.Domain;

    using OBeautifulCode.TypeRepresentation;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class DomainExtensions
    {
        /// <summary>
        /// Converts an object to a self described serialization to persist or share.
        /// </summary>
        /// <param name="objectToPackageIntoDescribedSerialization">Object to serialize.</param>
        /// <param name="serializationDescription">Description of the serializer to use.</param>
        /// <param name="serializerFactory">Implementation of <see cref="ISerializerFactory" /> that can resolve the serializer.</param>
        /// <param name="compressorFactory">Implementation of <see cref="ICompressorFactory" /> that can resolve the compressor.</param>
        /// <param name="typeMatchStrategy">Optional type match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="TypeMatchStrategy.NamespaceAndName" />.</param>
        /// <param name="multipleMatchStrategy">Optional multiple match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="MultipleMatchStrategy.ThrowOnMultiple" />.</param>
        /// <returns>Self decribed serialization.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Checked with Must and tested.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Checked with Must and tested.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "Checked with Must and tested.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "Spelling/name is correct.")]
        public static DescribedSerialization ToDescribedSerializationUsingSpecificFactory(
            this object objectToPackageIntoDescribedSerialization,
            SerializationDescription serializationDescription,
            ISerializerFactory serializerFactory,
            ICompressorFactory compressorFactory,
            TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName,
            MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple)
        {
            new { objectToPackageIntoDescribedSerialization, serializationDescription, serializerFactory, compressorFactory }.Must().NotBeNull().OrThrowFirstFailure();

            var serializer = serializerFactory.BuildSerializer(serializationDescription, typeMatchStrategy, multipleMatchStrategy);
            var compressor = compressorFactory.BuildCompressor(serializationDescription.CompressionKind);

            string payload;
            switch (serializationDescription.SerializationRepresentation)
            {
                case SerializationRepresentation.Binary:
                    var rawBytes = serializer.SerializeToBytes(objectToPackageIntoDescribedSerialization);
                    var compressedBytes = compressor.CompressBytes(rawBytes);
                    payload = DescribedSerialization.BinaryPayloadEncoding.GetString(compressedBytes);
                    break;
                case SerializationRepresentation.String:
                    payload = serializer.SerializeToString(objectToPackageIntoDescribedSerialization);
                    break;
                default: throw new NotSupportedException(Invariant($"{nameof(SerializationRepresentation)} - {serializationDescription.SerializationRepresentation} is not supported."));
            }

            var ret = new DescribedSerialization(
                objectToPackageIntoDescribedSerialization.GetType().ToTypeDescription(),
                payload,
                serializationDescription);

            return ret;
        }

        /// <summary>
        /// Converts a self described serialization back into it's object.
        /// </summary>
        /// <param name="describedSerialization">Self described serialized object.</param>
        /// <param name="serializerFactory">Implementation of <see cref="ISerializerFactory" /> that can resolve the serializer.</param>
        /// <param name="compressorFactory">Implementation of <see cref="ICompressorFactory" /> that can resolve the compressor.</param>
        /// <param name="typeMatchStrategy">Optional type match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="TypeMatchStrategy.NamespaceAndName" />.</param>
        /// <param name="multipleMatchStrategy">Optional multiple match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="MultipleMatchStrategy.ThrowOnMultiple" />.</param>
        /// <returns>Orginally serialized object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Checked with Must and tested.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Checked with Must and tested.")]
        public static object DeserializePayloadUsingSpecificFactory(
            this DescribedSerialization describedSerialization,
            ISerializerFactory serializerFactory,
            ICompressorFactory compressorFactory,
            TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName,
            MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple)
        {
            new { describedSerialization, serializerFactory, compressorFactory }.Must().NotBeNull().OrThrowFirstFailure();

            var serializer = serializerFactory.BuildSerializer(describedSerialization.SerializationDescription, typeMatchStrategy, multipleMatchStrategy);
            var compressor = compressorFactory.BuildCompressor(describedSerialization.SerializationDescription.CompressionKind);

            var targetType = describedSerialization.PayloadTypeDescription.ResolveFromLoadedTypes(typeMatchStrategy, multipleMatchStrategy);

            object ret;
            switch (describedSerialization.SerializationDescription.SerializationRepresentation)
            {
                case SerializationRepresentation.Binary:
                    var rawBytes = DescribedSerialization.BinaryPayloadEncoding.GetBytes(describedSerialization.SerializedPayload);
                    var decompressedBytes = compressor.DecompressBytes(rawBytes);
                    ret = serializer.Deserialize(decompressedBytes, targetType);
                    break;
                case SerializationRepresentation.String:
                    ret = serializer.Deserialize(describedSerialization.SerializedPayload, targetType);
                    break;
                default: throw new NotSupportedException(Invariant($"{nameof(SerializationRepresentation)} - {describedSerialization.SerializationDescription.SerializationRepresentation} is not supported."));
            }

            return ret;
        }

        /// <summary>
        /// Converts a self described serialization back into it's object.
        /// </summary>
        /// <param name="describedSerialization">Self described serialized object.</param>
        /// <param name="serializerFactory">Implementation of <see cref="ISerializerFactory" /> that can resolve the serializer.</param>
        /// <param name="compressorFactory">Implementation of <see cref="ICompressorFactory" /> that can resolve the compressor.</param>
        /// <param name="typeMatchStrategy">Optional type match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="TypeMatchStrategy.NamespaceAndName" />.</param>
        /// <param name="multipleMatchStrategy">Optional multiple match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="MultipleMatchStrategy.ThrowOnMultiple" />.</param>
        /// <typeparam name="T">Expected return type.</typeparam>
        /// <returns>Orginally serialized object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Checked with Must and tested.")]
        public static T DeserializePayloadUsingSpecificFactory<T>(
            this DescribedSerialization describedSerialization,
            ISerializerFactory serializerFactory,
            ICompressorFactory compressorFactory,
            TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName,
            MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple)
        {
            return (T)DeserializePayloadUsingSpecificFactory(describedSerialization, serializerFactory, compressorFactory, typeMatchStrategy, multipleMatchStrategy);
        }
    }
}