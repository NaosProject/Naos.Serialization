// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Compression.Domain;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Factory;
    using Naos.Serialization.Factory.Extensions;
    using Naos.Serialization.Json;
    using OBeautifulCode.Representation;

    using Xunit;

    public static class ExtensionsTest
    {
        [Fact]
        public static void ToDescribedSerializationWithSpecificFactory___Null_serializer_description___Throws()
        {
            // Arrange
            Action action = () => DomainExtensions.ToDescribedSerializationUsingSpecificFactory(
                A.Dummy<string>(),
                null,
                SerializerFactory.Instance,
                CompressorFactory.Instance);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("Parameter 'serializationDescription' is null.");
        }

        [Fact]
        public static void ToDescribedSerializationUsingSpecificFactory___Null_serializer_factory___Throws()
        {
            // Arrange
            Action action = () => DomainExtensions.ToDescribedSerializationUsingSpecificFactory(
                A.Dummy<string>(),
                A.Dummy<SerializationDescription>(),
                null,
                CompressorFactory.Instance);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("Parameter 'serializerFactory' is null.");
        }

        [Fact]
        public static void ToDescribedSerializationUsingSpecificFactory___Null_compressor_factory___Throws()
        {
            // Arrange
            Action action = () => DomainExtensions.ToDescribedSerializationUsingSpecificFactory(
                A.Dummy<string>(),
                A.Dummy<SerializationDescription>(),
                SerializerFactory.Instance,
                null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("Parameter 'compressorFactory' is null.");
        }

        [Fact]
        public static void ToDescribedSerializationWithSpecificFactory___Null_object___Works()
        {
            // Arrange
            string objectToPackageIntoDescribedSerialization = null;
            var serializerDescription = new SerializationDescription(SerializationKind.Json, SerializationFormat.String, typeof(NullJsonConfiguration).ToRepresentation(), CompressionKind.None);

            // Act
            var describedSerialization = objectToPackageIntoDescribedSerialization.ToDescribedSerializationUsingSpecificFactory(
                serializerDescription,
                SerializerFactory.Instance,
                CompressorFactory.Instance);

            // Assert
            describedSerialization.Should().NotBeNull();
            describedSerialization.PayloadTypeRepresentation.Should().Be(typeof(string).ToRepresentation());
            describedSerialization.SerializedPayload.Should().Be("null");
            describedSerialization.SerializationDescription.Should().Be(serializerDescription);
        }

        [Fact]
        public static void ToDescribedSerializationWithSpecificSerializer___Valid_object_and_serializer___Works()
        {
            // Arrange
            string objectToPackageIntoDescribedSerialization = null;
            var serializerDescription = new SerializationDescription(SerializationKind.Json, SerializationFormat.String, typeof(NullJsonConfiguration).ToRepresentation(), CompressionKind.None);

            // Act
            var describedSerialization = objectToPackageIntoDescribedSerialization.ToDescribedSerializationUsingSpecificFactory(
                serializerDescription,
                SerializerFactory.Instance,
                CompressorFactory.Instance);

            // Assert
            describedSerialization.Should().NotBeNull();
            describedSerialization.PayloadTypeRepresentation.Should().Be(typeof(string).ToRepresentation());
            describedSerialization.SerializedPayload.Should().Be("null");
            describedSerialization.SerializationDescription.Should().Be(serializerDescription);
        }

        [Fact]
        public static void ToDescribedSerializationWithSpecificFactory___All_valid___Works()
        {
            // Arrange
            var objectToPackageIntoDescribedSerialization = A.Dummy<string>();
            var serializerDescription = new SerializationDescription(SerializationKind.Json, SerializationFormat.String, typeof(NullJsonConfiguration).ToRepresentation(), CompressionKind.None);

            // Act
            var describedSerialization = DomainExtensions.ToDescribedSerializationUsingSpecificFactory(
                objectToPackageIntoDescribedSerialization,
                serializerDescription,
                SerializerFactory.Instance,
                CompressorFactory.Instance,
                unregisteredTypeEncounteredStrategy: UnregisteredTypeEncounteredStrategy.Attempt);

            // Assert
            describedSerialization.Should().NotBeNull();
            describedSerialization.PayloadTypeRepresentation.Should().Be(objectToPackageIntoDescribedSerialization.GetType().ToRepresentation());
            describedSerialization.SerializedPayload.Should().Be("\"" + objectToPackageIntoDescribedSerialization + "\"");
            describedSerialization.SerializationDescription.Should().Be(serializerDescription);
        }

        [Fact]
        public static void FromDescribedSerializationWithSpecificFactory___Null_object___Works()
        {
            // Arrange
            string expected = null;
            var serializerDescription = new SerializationDescription(SerializationKind.Json, SerializationFormat.String, typeof(NullJsonConfiguration).ToRepresentation(), CompressionKind.None);
            var payload = "null";
            var describedSerialization = new DescribedSerialization(
                typeof(string).ToRepresentation(),
                payload,
                serializerDescription);

            // Act
            var actual = DomainExtensions.DeserializePayloadUsingSpecificFactory(
                describedSerialization,
                SerializerFactory.Instance,
                CompressorFactory.Instance,
                unregisteredTypeEncounteredStrategy: UnregisteredTypeEncounteredStrategy.Attempt);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public static void FromDescribedSerializationWithSpecificFactory___All_valid___Works()
        {
            // Arrange
            var expected = A.Dummy<string>();
            var serializerDescription = new SerializationDescription(SerializationKind.Json, SerializationFormat.String, typeof(NullJsonConfiguration).ToRepresentation(), CompressionKind.None);
            var payload = "\"" + expected + "\"";
            var describedSerialization = new DescribedSerialization(
                expected.GetType().ToRepresentation(),
                payload,
                serializerDescription);

            // Act
            var actual = DomainExtensions.DeserializePayloadUsingSpecificFactory(
                describedSerialization,
                SerializerFactory.Instance,
                CompressorFactory.Instance,
                unregisteredTypeEncounteredStrategy: UnregisteredTypeEncounteredStrategy.Attempt);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().Be(expected);
        }

        [Fact]
        public static void ToDescribedSerialization___Valid___Just_passes_to_specific_factory_version_with_default_factory()
        {
            // Arrange
            var objectToPackageIntoDescribedSerialization = A.Dummy<string>();
            var serializerDescription = new SerializationDescription(SerializationKind.Json, SerializationFormat.String, typeof(NullJsonConfiguration).ToRepresentation(), CompressionKind.None);

            // Act
            var describedSerialization = objectToPackageIntoDescribedSerialization.ToDescribedSerialization(serializerDescription, unregisteredTypeEncounteredStrategy: UnregisteredTypeEncounteredStrategy.Attempt);

            // Assert
            describedSerialization.Should().NotBeNull();
            describedSerialization.PayloadTypeRepresentation.Should().Be(objectToPackageIntoDescribedSerialization.GetType().ToRepresentation());
            describedSerialization.SerializedPayload.Should().Be("\"" + objectToPackageIntoDescribedSerialization + "\"");
            describedSerialization.SerializationDescription.Should().Be(serializerDescription);
        }

        [Fact]
        public static void FromDescribedSerialization___Valid___Just_passes_to_specific_factory_version_with_default_factory()
        {
            // Arrange
            var expected = A.Dummy<string>();
            var serializerDescription = new SerializationDescription(SerializationKind.Json, SerializationFormat.String, typeof(NullJsonConfiguration).ToRepresentation(), CompressionKind.None);
            var payload = "\"" + expected + "\"";
            var describedSerialization = new DescribedSerialization(
                expected.GetType().ToRepresentation(),
                payload,
                serializerDescription);

            // Act
            var actual = describedSerialization.DeserializePayload(unregisteredTypeEncounteredStrategy: UnregisteredTypeEncounteredStrategy.Attempt);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().Be(expected);
        }

        [Fact]
        public static void FromDescribedSerializationGeneric___Valid___Just_passes_to_specific_factory_version_with_default_factory()
        {
            // Arrange
            var expected = A.Dummy<string>();
            var serializerDescription = new SerializationDescription(SerializationKind.Json, SerializationFormat.String, typeof(NullJsonConfiguration).ToRepresentation(), CompressionKind.None);
            var payload = "\"" + expected + "\"";
            var describedSerialization = new DescribedSerialization(
                expected.GetType().ToRepresentation(),
                payload,
                serializerDescription);

            // Act
            var actual = describedSerialization.DeserializePayload<string>(unregisteredTypeEncounteredStrategy: UnregisteredTypeEncounteredStrategy.Attempt);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().Be(expected);
        }
    }
}
