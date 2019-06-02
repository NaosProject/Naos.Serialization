// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializingAndDeserializingBehaviorOfNull.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using FluentAssertions;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using Naos.Serialization.PropertyBag;
    using Xunit;

    public static class SerializingAndDeserializingBehaviorOfNull
    {
        [Fact]
        public static void PropertyBagCanSerializeNull()
        {
            // Arrange
            var serializer = new NaosPropertyBagSerializer();

            // Act
            var actualNullString = serializer.SerializeToString(null);
            var actualNullBytes = serializer.SerializeToBytes(null);
            var actualStringException = Record.Exception(() => serializer.SerializeToString(SerializationConfigurationBase.NullSerializedStringValue));
            var actualNullFromString = serializer.Deserialize<NullableObject>(actualNullString);
            var actualNullFromBytes = serializer.Deserialize<NullableObject>(actualNullBytes);

            // Assert
            actualNullString.Should().Be(SerializationConfigurationBase.NullSerializedStringValue);
            actualNullFromString.Should().BeNull();
            actualNullFromBytes.Should().BeNull();
            actualStringException.Should().NotBeNull();
            actualStringException.Should().BeOfType<NotSupportedException>();
            actualStringException.Message.Should().Be("String is not supported as a type for this serializer.");
        }

        [Fact]
        public static void JsonCanSerializeNull()
        {
            // Arrange
            var serializer = new NaosJsonSerializer();

            // Act
            var actualNullString = serializer.SerializeToString(null);
            var actualNullBytes = serializer.SerializeToBytes(null);
            var actualString = serializer.SerializeToString(SerializationConfigurationBase.NullSerializedStringValue);
            var actualNullFromString = serializer.Deserialize<NullableObject>(actualNullString);
            var actualNullFromBytes = serializer.Deserialize<NullableObject>(actualNullBytes);
            var actual = serializer.Deserialize<string>(actualString);

            // Assert
            actualNullString.Should().Be(SerializationConfigurationBase.NullSerializedStringValue);
            actualNullFromString.Should().BeNull();
            actualNullFromBytes.Should().BeNull();
            actualString.Should().NotBe(SerializationConfigurationBase.NullSerializedStringValue);
            actual.Should().Be(SerializationConfigurationBase.NullSerializedStringValue);
        }

        [Fact]
        public static void BsonCanSerializeNull()
        {
            // Arrange
            var serializer = new NaosBsonSerializer();

            // Act
            var actualNullString = serializer.SerializeToString(null);
            var actualNullBytes = serializer.SerializeToBytes(null);
            var actualStringException = Record.Exception(() => serializer.SerializeToString(SerializationConfigurationBase.NullSerializedStringValue));
            var actualNullFromString = serializer.Deserialize<NullableObject>(actualNullString);
            var actualNullFromBytes = serializer.Deserialize<NullableObject>(actualNullBytes);

            // Assert
            actualNullString.Should().Be(SerializationConfigurationBase.NullSerializedStringValue);
            actualNullFromString.Should().BeNull();
            actualNullFromBytes.Should().BeNull();
            actualStringException.Should().NotBeNull();
            actualStringException.Should().BeOfType<NotSupportedException>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Not important.")]
        public class NullableObject
        {
        }
    }
}
