// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainExtensions.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

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
        /// <typeparam name="T">Type of object to serialize.</typeparam>
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
        public static DescribedSerialization ToDescribedSerializationUsingSpecificFactory<T>(
            this T objectToPackageIntoDescribedSerialization,
            SerializationDescription serializationDescription,
            ISerializerFactory serializerFactory,
            ICompressorFactory compressorFactory,
            TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName,
            MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple)
        {
            new { serializationDescription, serializerFactory, compressorFactory }.Must().NotBeNull().OrThrowFirstFailure();

            var serializer = serializerFactory.BuildSerializer(serializationDescription, typeMatchStrategy, multipleMatchStrategy);
            var compressor = compressorFactory.BuildCompressor(serializationDescription.CompressionKind);

            var ret = objectToPackageIntoDescribedSerialization.ToDescribedSerializationUsingSpecificSerializer(
                serializationDescription,
                serializer,
                compressor);

            return ret;
        }

        /// <summary>
        /// Converts an object to a self described serialization to persist or share.
        /// </summary>
        /// <typeparam name="T">Type of object to serialize.</typeparam>
        /// <param name="objectToPackageIntoDescribedSerialization">Object to serialize.</param>
        /// <param name="serializationDescription">Description of the serializer to use.</param>
        /// <param name="serializer">Serializer to use.</param>
        /// <param name="compressor">Optional compressor to use; DEFAULT is null.</param>
        /// <returns>Self decribed serialization.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Checked with Must and tested.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Checked with Must and tested.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "Checked with Must and tested.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "Spelling/name is correct.")]
        public static DescribedSerialization ToDescribedSerializationUsingSpecificSerializer<T>(
            this T objectToPackageIntoDescribedSerialization,
            SerializationDescription serializationDescription,
            ISerialize serializer,
            ICompress compressor = null)
        {
            new { serializationDescription, serializer }.Must().NotBeNull().OrThrowFirstFailure();

            var localCompressor = compressor ?? new NullCompressor();

            localCompressor.CompressionKind.Named(Invariant($"{nameof(serializationDescription)}.{nameof(serializationDescription.CompressionKind)}-Must-match-{nameof(compressor)}.{nameof(compressor.CompressionKind)}")).Must().BeEqualTo(serializationDescription.CompressionKind).OrThrowFirstFailure();

            string payload;
            switch (serializationDescription.SerializationRepresentation)
            {
                case SerializationRepresentation.Binary:
                    var rawBytes = serializer.SerializeToBytes(objectToPackageIntoDescribedSerialization);
                    var compressedBytes = localCompressor.CompressBytes(rawBytes);
                    payload = DescribedSerialization.BinaryPayloadEncoding.GetString(compressedBytes);
                    break;
                case SerializationRepresentation.String:
                    payload = serializer.SerializeToString(objectToPackageIntoDescribedSerialization);
                    break;
                default: throw new NotSupportedException(Invariant($"{nameof(SerializationRepresentation)} - {serializationDescription.SerializationRepresentation} is not supported."));
            }

            var ret = new DescribedSerialization(
                (objectToPackageIntoDescribedSerialization?.GetType() ?? typeof(T)).ToTypeDescription(),
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

            var ret = describedSerialization.DeserializePayloadUsingSpecificSerializer(serializer, compressor, typeMatchStrategy, multipleMatchStrategy);
            return ret;
        }

        /// <summary>
        /// Converts a self described serialization back into it's object.
        /// </summary>
        /// <param name="describedSerialization">Self described serialized object.</param>
        /// <param name="deserializer">Deserializer to use.</param>
        /// <param name="decompressor">Optional compressor to use; DEFAULT is null.</param>
        /// <param name="typeMatchStrategy">Optional type match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="TypeMatchStrategy.NamespaceAndName" />.</param>
        /// <param name="multipleMatchStrategy">Optional multiple match strategy for resolving the type of object as well as the configuration type if any; DEFAULT is <see cref="MultipleMatchStrategy.ThrowOnMultiple" />.</param>
        /// <returns>Orginally serialized object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "deserializer", Justification = "It's a better name than serializer.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "decompressor", Justification = "It's a better name than compressor.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Checked with Must and tested.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Checked with Must and tested.")]
        public static object DeserializePayloadUsingSpecificSerializer(
            this DescribedSerialization describedSerialization,
            IDeserialize deserializer,
            IDecompress decompressor = null,
            TypeMatchStrategy typeMatchStrategy = TypeMatchStrategy.NamespaceAndName,
            MultipleMatchStrategy multipleMatchStrategy = MultipleMatchStrategy.ThrowOnMultiple)
        {
            new { describedSerialization, deserializer }.Must().NotBeNull().OrThrowFirstFailure();

            var localDecompressor = decompressor ?? new NullCompressor();

            var targetType = describedSerialization.PayloadTypeDescription.ResolveFromLoadedTypes(typeMatchStrategy, multipleMatchStrategy);

            object ret;
            switch (describedSerialization.SerializationDescription.SerializationRepresentation)
            {
                case SerializationRepresentation.Binary:
                    var rawBytes = DescribedSerialization.BinaryPayloadEncoding.GetBytes(describedSerialization.SerializedPayload);
                    var decompressedBytes = localDecompressor.DecompressBytes(rawBytes);
                    ret = deserializer.Deserialize(decompressedBytes, targetType);
                    break;
                case SerializationRepresentation.String:
                    ret = deserializer.Deserialize(describedSerialization.SerializedPayload, targetType);
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

        /// <summary>
        /// Interrogates the type for a parameterless constructor.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>A value indicating whether or not the type has a parameterless constructor.</returns>
        public static bool HasParameterlessConstructor(this Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();
            var paramterlessConstructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).SingleOrDefault(_ => _.GetParameters().Length == 0);
            return paramterlessConstructor != null;
        }

        /// <summary>
        /// Interrogates the type to see if it implements a specified interface.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <typeparam name="T">Type of interface to check for.</typeparam>
        /// <returns>A value indicating whether or not the type implements the interface.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Like this usage.")]
        public static bool ImplementsInterface<T>(this Type type)
            where T : class
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            var interfaceType = typeof(T);
            return ImplementsInterface(type, interfaceType);
        }

        /// <summary>
        /// Interrogates the type to see if it implements a specified interface.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <param name="interfaceType">Type to check.Type of interface to check for.</param>
        /// <returns>A value indicating whether or not the type implements the interface.</returns>
        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();
            new { interfaceType }.Must().NotBeNull().OrThrowFirstFailure();

            var iteratingType = type;
            while (iteratingType != null)
            {
                if (type.GetInterfaces().Any(_ => _ == interfaceType))
                {
                    return true;
                }

                iteratingType = iteratingType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Get the serializer by inspecting attribute on the property and optionally the specific type if known.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        /// <param name="specificType">Specific type if known.</param>
        /// <returns>Type of serializer if found, otherwise null.</returns>
        public static Type GetSerializerTypeFromAttribute(this PropertyInfo propertyInfo, Type specificType = null)
        {
            new { propertyInfo }.Must().NotBeNull().OrThrowFirstFailure();

            var propertyType = specificType ?? propertyInfo.PropertyType;
            var ret = ((NaosStringSerializerAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(NaosStringSerializerAttribute)))?.SerializerType
                      ?? ((NaosStringSerializerAttribute)Attribute.GetCustomAttribute(propertyType, typeof(NaosStringSerializerAttribute)))?.SerializerType;

            return ret;
        }

        /// <summary>
        /// Get the serializer by inspecting attribute on the type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>Type of serializer if found, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Prefer this for clarity of purpose.")]
        public static Type GetSerializerTypeFromAttribute(this Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            var ret = ((NaosStringSerializerAttribute)Attribute.GetCustomAttribute(type, typeof(NaosStringSerializerAttribute)))?.SerializerType;
            return ret;
        }

        /// <summary>
        /// Get the element serializer by inspecting attribute on the property.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        /// <returns>Type of element serializer if found, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Prefer this for clarity of purpose.")]
        public static Type GetElementSerializerTypeFromAttribute(this PropertyInfo propertyInfo)
        {
            new { propertyInfo }.Must().NotBeNull().OrThrowFirstFailure();

            var ret = ((NaosElementStringSerializerAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(NaosElementStringSerializerAttribute)))
                ?.ElementSerializerType;
            return ret;
        }
    }
}