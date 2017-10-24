// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonSerializerTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;

    using Xunit;

    using static System.FormattableString;

    public static class BsonSerializerTest
    {
        [Fact]
        public static void NaosBsonSerializer___Not_Custom_SerializationKind___Throws()
        {
            // Arrange
            Action action1 = () => new NaosBsonSerializer(SerializationKind.Invalid);
            Action action2 = () => new NaosBsonSerializer(SerializationKind.Default);
            Action action3 = () => new NaosBsonSerializer(SerializationKind.Compact);
            Action action4 = () => new NaosBsonSerializer(SerializationKind.Minimal);

            // Act
            var exception1 = Record.Exception(action1);
            var exception2 = Record.Exception(action2);
            var exception3 = Record.Exception(action3);
            var exception4 = Record.Exception(action4);

            // Assert
            new Dictionary<SerializationKind, Exception>
                {
                    { SerializationKind.Invalid, exception1 },
                    { SerializationKind.Default, exception2 },
                    { SerializationKind.Compact, exception3 },
                    { SerializationKind.Minimal, exception4 },
                }.ToList().ForEach(
                _ =>
                    {
                        _.Value.Should().NotBeNull();
                        _.Value.Should().BeOfType<ArgumentException>();
                        _.Value.Message.Should().Be(Invariant($"Value must be equal to Custom.\r\nParameter name: serializationKind"));
                    });
        }

        [Fact]
        public static void NaosBsonSerializer___Invalid_configuration_type___Throws()
        {
            // Arrange
            Action action = () => new NaosBsonSerializer(SerializationKind.Custom, typeof(string));

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
            Action action = () => new NaosBsonSerializer(SerializationKind.Custom, typeof(CustomNoPublicConstructor));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: configurationType must contain a default constructor to use in NaosBsonSerializer.");
        }
    }
}
