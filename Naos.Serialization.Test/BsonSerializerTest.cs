// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonSerializerTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
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
            Action action1 = () => new NaosBsonSerializer(serializationKind: SerializationKind.Invalid);
            Action action2 = () => new NaosBsonSerializer(serializationKind: SerializationKind.Custom);
            Action action3 = () => new NaosBsonSerializer(serializationKind: SerializationKind.Compact);
            Action action4 = () => new NaosBsonSerializer(serializationKind: SerializationKind.Minimal);

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
                        _.Value.Should().BeOfType<ArgumentOutOfRangeException>();
                        _.Value.Message.Should().StartWith("Parameter 'serializationKind' is not equal to the comparison value using EqualityComparer<T>.Default, where T: SerializationKind.");
                    });
        }

        [Fact]
        public static void NaosBsonSerializer___Invalid_configuration_type___Throws()
        {
            // Arrange
            Action action = () => new NaosBsonSerializer(typeof(string), SerializationKind.Default);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Parameter 'Configuration type - System.String - must derive from BsonConfigurationBase.' is not true.  Parameter value is 'False'.");
        }

        [Fact]
        public static void Constructor___Type_without_default_constructor___Throws()
        {
            // Arrange
            Action action = () => new NaosBsonSerializer(typeof(CustomNoPublicConstructor), SerializationKind.Default);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Parameter 'configurationType must contain a default constructor to use in NaosBsonSerializer.' is not true.  Parameter value is 'False'.");
        }
    }
}
