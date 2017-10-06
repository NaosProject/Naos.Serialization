// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBsonSerializer.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;

    using FluentAssertions;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;

    using Xunit;

    public static class TestBsonSerializer
    {
        [Fact]
        public static void NaosBsonSerializer___Invalid_SerializationKind___Throws()
        {
            // Arrange
            Action action = () => new NaosBsonSerializer(SerializationKind.Invalid);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must not be equal to Invalid.\r\nParameter name: serializationKind");
        }

        [Fact]
        public static void NaosBsonSerializer___Invalid_configuration_type___Throws()
        {
            // Arrange
            Action action = () => new NaosBsonSerializer(SerializationKind.Default, typeof(string));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be true.\r\nParameter name: Configuration type - System.String - must derive from BsonConfigurationBase");
        }

        [Fact]
        public static void Constructor___Type_without_default_constructor___Throws()
        {
            // Arrange
            Action action = () => new NaosBsonSerializer(SerializationKind.Default, typeof(CustomNoPublicConstructor));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: configurationType must contain a default constructor to use in NaosBsonSerializer.");
        }
    }
}
