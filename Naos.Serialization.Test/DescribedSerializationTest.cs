// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DescribedSerializationTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Serialization.Domain;

    using OBeautifulCode.TypeRepresentation;

    using Xunit;

    public static class DescribedSerializationTest
    {
        [Fact]
        public static void Constructor__Should_throw_ArgumentNullException___When_parameter_typeDescription_is_null()
        {
            // Arrange
            Action action = () => new DescribedSerialization(null, A.Dummy<string>(), A.Dummy<SerializationDescription>());

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: payloadTypeDescription");
        }

        [Fact]
        public static void Constructor__Should_throw_ArgumentException___When_parameter_SerializerDescription_is_null()
        {
            // Arrange
            Action action = () => new DescribedSerialization(A.Dummy<TypeDescription>(), string.Empty, null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: serializationDescription");
        }

        [Fact]
        public static void TypeDescription__Should_return_same_typeDescription_passed_to_constructor___When_getting()
        {
            // Arrange
            var typeDescription = A.Dummy<TypeDescription>();
            var payload = A.Dummy<string>();
            var serializer = A.Dummy<SerializationDescription>();
            var systemUnderTest = new DescribedSerialization(typeDescription, payload, serializer);

            // Act
            var actual = systemUnderTest.PayloadTypeDescription;

            // Assert
            actual.Should().BeSameAs(typeDescription);
        }

        [Fact]
        public static void Payload__Should_return_same_payload_passed_to_constructor___When_getting()
        {
            // Arrange
            var typeDescription = A.Dummy<TypeDescription>();
            var payload = A.Dummy<string>();
            var serializer = A.Dummy<SerializationDescription>();
            var systemUnderTest = new DescribedSerialization(typeDescription, payload, serializer);

            // Act
            var actual = systemUnderTest.SerializedPayload;

            // Assert
            actual.Should().Be(payload);
        }

        [Fact]
        public static void Serializer__Should_return_same_serializer_passed_to_constructor___When_getting()
        {
            // Arrange
            var typeDescription = A.Dummy<TypeDescription>();
            var payload = A.Dummy<string>();
            var serializer = A.Dummy<SerializationDescription>();
            var systemUnderTest = new DescribedSerialization(typeDescription, payload, serializer);

            // Act
            var actual = systemUnderTest.SerializationDescription;

            // Assert
            actual.Should().Be(serializer);
        }
    }
}