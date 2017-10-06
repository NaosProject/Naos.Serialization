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
    using Naos.Serialization.Domain;
    using Naos.Serialization.Factory;

    using Xunit;

    using static System.FormattableString;

    public static class FactoryTest
    {
        [Fact]
        public static void BuildCompressor___Null_description___Throws()
        {
            // Arrange
            Action action = () => SerializerFactory.Instance.BuildCompressor(null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: serializationDescription");
        }

        [Fact]
        public static void BuildSerializer___Null_description___Throws()
        {
            // Arrange
            Action action = () => SerializerFactory.Instance.BuildSerializer(null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: serializationDescription");
        }

        [Fact]
        public static void BuildCompressor___None_kind___Gets_NullCompressor()
        {
            // Arrange
            var serializerDescription = new SerializationDescription(
                SerializationFormat.Json,
                SerializationRepresentation.String,
                SerializationKind.Default,
                null,
                CompressionKind.None);

            // Act
            var compressor = SerializerFactory.Instance.BuildCompressor(serializerDescription);

            // Assert
            compressor.Should().NotBeNull();
            compressor.Should().BeOfType<NullCompressor>();
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
            var compressor = SerializerFactory.Instance.BuildCompressor(serializerDescription);

            // Assert
            compressor.Should().NotBeNull();
            compressor.Should().BeOfType<NullCompressor>();
        }

        [Fact]
        public static void BuildSerializer___Bson___Gets_Bson_serializer()
        {
            // Arrange
            var serializerDescription = new SerializationDescription(
                SerializationFormat.Json,
                SerializationRepresentation.String);

            // Act
            var compressor = SerializerFactory.Instance.BuildCompressor(serializerDescription);

            // Assert
            compressor.Should().NotBeNull();
            compressor.Should().BeOfType<NullCompressor>();
        }
    }
}
