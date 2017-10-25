// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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
            exception.Message.Should().Be("\r\nParameter name: serializationDescription");
            jsonException.Should().NotBeNull();
            jsonException.Should().BeOfType<ArgumentNullException>();
            jsonException.Message.Should().Be("\r\nParameter name: serializationDescription");
        }

        [Fact]
        public static void BuildSerializer___Json___Gets_Json_serializer()
        {
            // Arrange
            var serializerDescription = new SerializationDescription(
                SerializationFormat.Json,
                SerializationRepresentation.String,
                SerializationKind.Default,
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
            exception.Message.Should().Be("\r\nParameter name: serializationDescription");
            bsonException.Should().NotBeNull();
            bsonException.Should().BeOfType<ArgumentNullException>();
            bsonException.Message.Should().Be("\r\nParameter name: serializationDescription");
        }

        [Fact]
        public static void BuildSerializer___Bson___Gets_Bson_serializer()
        {
            // Arrange
            var expectedConfigType = typeof(CustomThrowsConfig);
            var serializerDescription = new SerializationDescription(
                SerializationFormat.Bson,
                SerializationRepresentation.String,
                SerializationKind.Custom,
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
