// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;

    using FluentAssertions;

    using Naos.Compression.Domain;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Factory;
    using Naos.Serialization.Json;

    using OBeautifulCode.TypeRepresentation;

    using Xunit;

    public static class FactoryTest
    {
        [Fact]
        public static void BuildSerializer___Null_description___Throws()
        {
            // Arrange
            Action action = () => SerializerFactory.Instance.BuildSerializer(null);
            Action jsonAction = () => JsonSerializerFactory.Instance.BuildSerializer(null);

            // Act
            var exception = Record.Exception(action);
            var jsonException = Record.Exception(jsonAction);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("Parameter 'serializationDescription' is null.");
            jsonException.Should().NotBeNull();
            jsonException.Should().BeOfType<ArgumentNullException>();
            jsonException.Message.Should().Be("Parameter 'serializationDescription' is null.");
        }

        [Fact]
        public static void BuildSerializer___Json___Gets_Json_serializer()
        {
            // Arrange
            var serializerDescription = new SerializationDescription(
                SerializationKind.Json,
                SerializationFormat.String,
                null,
                CompressionKind.None);

            // Act
            var serializer = SerializerFactory.Instance.BuildSerializer(serializerDescription);
            var jsonSerializer = JsonSerializerFactory.Instance.BuildSerializer(serializerDescription);

            // Assert
            serializer.Should().NotBeNull();
            serializer.Should().BeOfType<NaosJsonSerializer>();
            jsonSerializer.Should().NotBeNull();
            jsonSerializer.Should().BeOfType<NaosJsonSerializer>();
        }

        [Fact]
        public static void BuildSerializer___Bson___Null_description___Throws()
        {
            // Arrange
            Action action = () => SerializerFactory.Instance.BuildSerializer(null);
            Action bsonAction = () => BsonSerializerFactory.Instance.BuildSerializer(null);

            // Act
            var exception = Record.Exception(action);
            var bsonException = Record.Exception(bsonAction);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("Parameter 'serializationDescription' is null.");
            bsonException.Should().NotBeNull();
            bsonException.Should().BeOfType<ArgumentNullException>();
            bsonException.Message.Should().Be("Parameter 'serializationDescription' is null.");
        }

        [Fact]
        public static void BuildSerializer___Bson___Gets_Bson_serializer()
        {
            // Arrange
            var expectedConfigType = typeof(NullBsonConfiguration);
            var serializerDescription = new SerializationDescription(
                SerializationKind.Bson,
                SerializationFormat.String,
                expectedConfigType.ToTypeDescription());

            // Act
            var serializer = SerializerFactory.Instance.BuildSerializer(serializerDescription);
            var bsonSerializer = BsonSerializerFactory.Instance.BuildSerializer(serializerDescription);

            // Assert
            serializer.Should().NotBeNull();
            serializer.Should().BeOfType<NaosBsonSerializer>();
            serializer.ConfigurationType.Should().NotBeNull();
            serializer.ConfigurationType.Should().Be(expectedConfigType);

            bsonSerializer.Should().NotBeNull();
            bsonSerializer.Should().BeOfType<NaosBsonSerializer>();
            bsonSerializer.ConfigurationType.Should().NotBeNull();
            bsonSerializer.ConfigurationType.Should().Be(expectedConfigType);
        }
    }
}
