// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Compression.Domain;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Domain.Extensions;
    using Naos.Serialization.Factory;
    using Naos.Serialization.Factory.Extensions;

    using OBeautifulCode.TypeRepresentation;

    using Xunit;

    public static class ExtensionsTest
    {
        [Fact]
        public static void ToDescribedSerializationWithSpecificFactory___Null_object___Throws()
        {
            // Arrange
            Action action = () => DomainExtensions.ToDescribedSerializationWithSpecificFactory(
                null,
                A.Dummy<SerializationDescription>(),
                SerializerFactory.Instance);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: objectToPackageIntoDescribedSerialization");
        }

        [Fact]
        public static void ToDescribedSerializationWithSpecificFactory___Null_serializer_description___Throws()
        {
            // Arrange
            Action action = () => DomainExtensions.ToDescribedSerializationWithSpecificFactory(
                A.Dummy<string>(),
                null,
                SerializerFactory.Instance);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: serializationDescription");
        }

        [Fact]
        public static void ToDescribedSerializationWithSpecificFactory___Null_serializer_factory___Throws()
        {
            // Arrange
            Action action = () => DomainExtensions.ToDescribedSerializationWithSpecificFactory(
                A.Dummy<string>(),
                A.Dummy<SerializationDescription>(),
                null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: serializerFactory");
        }

        [Fact]
        public static void ToDescribedSerializationWithSpecificFactory___All_valid___Works()
        {
            // Arrange
            var objectToPackageIntoDescribedSerialization = A.Dummy<string>();
            var serializerDescription = new SerializationDescription(SerializationFormat.Json, SerializationRepresentation.String, SerializationKind.Minimal, typeof(BsonConfigurationTestAutoConstrainedType).ToTypeDescription(), CompressionKind.None);

            // Act
            var describedSerialization = DomainExtensions.ToDescribedSerializationWithSpecificFactory(objectToPackageIntoDescribedSerialization, serializerDescription, SerializerFactory.Instance);

            // Assert
            describedSerialization.Should().NotBeNull();
            describedSerialization.PayloadTypeDescription.Should().Be(objectToPackageIntoDescribedSerialization.GetType().ToTypeDescription());
            describedSerialization.SerializedPayload.Should().Be("\"" + objectToPackageIntoDescribedSerialization + "\"");
            describedSerialization.SerializationDescription.Should().Be(serializerDescription);
        }

        [Fact]
        public static void FromDescribedSerializationWithSpecificFactory___All_valid___Works()
        {
            // Arrange
            var expected = A.Dummy<string>();
            var serializerDescription = new SerializationDescription(SerializationFormat.Json, SerializationRepresentation.String, SerializationKind.Minimal, typeof(BsonConfigurationTestAutoConstrainedType).ToTypeDescription(), CompressionKind.None);
            var payload = "\"" + expected + "\"";
            var describedSerialization = new DescribedSerialization(
                expected.GetType().ToTypeDescription(),
                payload,
                serializerDescription);

            // Act
            var actual = DomainExtensions.FromDescribedSerializationWithSpecificFactory(describedSerialization, SerializerFactory.Instance);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().Be(expected);
        }

        [Fact]
        public static void ToDescribedSerialization___Valid___Just_passes_to_specific_factory_version_with_default_factory()
        {
            // Arrange
            var objectToPackageIntoDescribedSerialization = A.Dummy<string>();
            var serializerDescription = new SerializationDescription(SerializationFormat.Json, SerializationRepresentation.String, SerializationKind.Minimal, typeof(BsonConfigurationTestAutoConstrainedType).ToTypeDescription(), CompressionKind.None);

            // Act
            var describedSerialization = FactoryExtensions.ToDescribedSerialization(objectToPackageIntoDescribedSerialization, serializerDescription);

            // Assert
            describedSerialization.Should().NotBeNull();
            describedSerialization.PayloadTypeDescription.Should().Be(objectToPackageIntoDescribedSerialization.GetType().ToTypeDescription());
            describedSerialization.SerializedPayload.Should().Be("\"" + objectToPackageIntoDescribedSerialization + "\"");
            describedSerialization.SerializationDescription.Should().Be(serializerDescription);
        }

        [Fact]
        public static void FromDescribedSerialization___Valid___Just_passes_to_specific_factory_version_with_default_factory()
        {
            // Arrange
            var expected = A.Dummy<string>();
            var serializerDescription = new SerializationDescription(SerializationFormat.Json, SerializationRepresentation.String, SerializationKind.Minimal, typeof(BsonConfigurationTestAutoConstrainedType).ToTypeDescription(), CompressionKind.None);
            var payload = "\"" + expected + "\"";
            var describedSerialization = new DescribedSerialization(
                expected.GetType().ToTypeDescription(),
                payload,
                serializerDescription);

            // Act
            var actual = FactoryExtensions.FromDescribedSerialization(describedSerialization);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().Be(expected);
        }

        [Fact]
        public static void FromDescribedSerializationGeneric___Valid___Just_passes_to_specific_factory_version_with_default_factory()
        {
            // Arrange
            var expected = A.Dummy<string>();
            var serializerDescription = new SerializationDescription(SerializationFormat.Json, SerializationRepresentation.String, SerializationKind.Minimal, typeof(BsonConfigurationTestAutoConstrainedType).ToTypeDescription(), CompressionKind.None);
            var payload = "\"" + expected + "\"";
            var describedSerialization = new DescribedSerialization(
                expected.GetType().ToTypeDescription(),
                payload,
                serializerDescription);

            // Act
            var actual = FactoryExtensions.FromDescribedSerialization<string>(describedSerialization);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().Be(expected);
        }
    }
}
