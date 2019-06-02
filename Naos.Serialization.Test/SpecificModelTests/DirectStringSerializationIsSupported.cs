// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectStringSerializationIsSupported.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using FakeItEasy;
    using FluentAssertions;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Json;
    using Xunit;

    public static class DirectStringSerializationIsSupported
    {
        [Fact]
        public static void BsonSerializeStringToString()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<GenericDiscoveryBsonConfiguration<string>>();
            var input = A.Dummy<string>();

            // Act
            var bsonStringException = Record.Exception(() => bsonSerializer.SerializeToString(input));

            // Assert
            bsonStringException.Should().BeOfType<NotSupportedException>();
        }

        [Fact]
        public static void BsonSerializeStringToBytes()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<GenericDiscoveryBsonConfiguration<string>>();
            var input = A.Dummy<string>();

            // Act
            var bsonStringException = Record.Exception(() => bsonSerializer.SerializeToBytes(input));

            // Assert
            bsonStringException.Should().BeOfType<NotSupportedException>();
        }

        [Fact]
        public static void JsonSerializeStringToString()
        {
            // Arrange
            var jsonSerializer = new NaosJsonSerializer<GenericDiscoveryJsonConfiguration<string>>();
            var input = A.Dummy<string>();

            // Act
            var actualJsonString = jsonSerializer.SerializeToString(input);
            var actualJsonFromString = jsonSerializer.Deserialize<string>(actualJsonString);

            // Assert
            actualJsonFromString.Should().Be(input);
        }

        [Fact]
        public static void JsonSerializeStringToBytes()
        {
            // Arrange
            var jsonSerializer = new NaosJsonSerializer<GenericDiscoveryJsonConfiguration<string>>();
            var input = A.Dummy<string>();

            // Act
            var actualJsonBytes = jsonSerializer.SerializeToBytes(input);
            var actualJsonFromBytes = jsonSerializer.Deserialize<string>(actualJsonBytes);

            // Assert
            actualJsonFromBytes.Should().Be(input);
        }
    }
}
