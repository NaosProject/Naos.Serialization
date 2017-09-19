// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;

    using Naos.Serialization.Bson;

    using Xunit;

    public static class TestExtensionMethods
    {
        [Fact]
        public static void SetEnumStringSerializer___Map_is_null___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ExtensionMethods.SetEnumStringSerializer(null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: map");
        }

        [Fact]
        public static void SetEnumStringSerializer___Map_member_not_enumeration___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ClassMap.MapMember(typeof(TestMapping).GetProperty(nameof(TestMapping.GuidProperty))).SetEnumStringSerializer();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be true.\r\nParameter name: memberTypeIsEnumeration");
        }

        [Fact]
        public static void SetEnumStringSerializer___Correct___Works()
        {
            // Arrange
            var memberToMap = ClassMap.MapMember(typeof(TestMapping).GetProperty(nameof(TestMapping.EnumProperty)));

            // Act
            var ret = memberToMap.SetEnumStringSerializer();

            // Assert
            ret.Should().NotBeNull();
            var bsonSerializer = ret.GetSerializer();
            bsonSerializer.Should().NotBeNull();
            bsonSerializer.Should().BeOfType<EnumSerializer<TestEnumeration>>();
            var enumSerializer = (EnumSerializer<TestEnumeration>)bsonSerializer;
            enumSerializer.Representation.Should().Be(BsonType.String);
        }

        [Fact]
        public static void SetEnumArraySerializer___Map_is_null___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ExtensionMethods.SetEnumArraySerializer(null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: map");
        }

        [Fact]
        public static void SetEnumArraySerializer___Member_not_array___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ClassMap.MapMember(typeof(TestMapping).GetProperty(nameof(TestMapping.DateTimePropertyUtc))).SetEnumArraySerializer();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be true.\r\nParameter name: memberTypeIsArray");
        }

        [Fact]
        public static void SetEnumArraySerializer___Member_not_enum_array___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ClassMap.MapMember(typeof(TestMapping).GetProperty(nameof(TestMapping.NonEnumArray))).SetEnumArraySerializer();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be true.\r\nParameter name: itemTypeIsEnum");
        }

        [Fact]
        public static void SetEnumArraySerializer___Correct___Works()
        {
            // Arrange
            var memberToMap = ClassMap.MapMember(typeof(TestMapping).GetProperty(nameof(TestMapping.EnumArray)));

            // Act
            var ret = memberToMap.SetEnumArraySerializer();

            // Assert
            ret.Should().NotBeNull();
            var bsonSerializer = ret.GetSerializer();
            bsonSerializer.Should().NotBeNull();

            var expectedSerializerType = typeof(ArraySerializer<TestEnumeration>);
            var actualSerializerType = ret.GetSerializer().GetType();
            actualSerializerType.GetGenericTypeDefinition().Should().Be(expectedSerializerType.GetGenericTypeDefinition());
            actualSerializerType.GetGenericArguments().Single().Should().Be(expectedSerializerType.GetGenericArguments().Single());

            var arraySerializer = (ArraySerializer<TestEnumeration>)bsonSerializer;
            var enumSerializer = (EnumSerializer<TestEnumeration>)arraySerializer.ItemSerializer;
            enumSerializer.Representation.Should().Be(BsonType.String);
        }

        [Fact]
        public static void SetDictionarySerializer___Map_is_null___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ExtensionMethods.SetDictionarySerializer(null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: map");
        }

        [Fact]
        public static void SetDictionarySerializer___Map_member_not_generic___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ClassMap.MapMember(typeof(TestMapping).GetProperty(nameof(TestMapping.StringProperty))).SetDictionarySerializer();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be equal to 2.\r\nParameter name: memberGenericArgumentsCount");
        }

        [Fact]
        public static void SetDictionarySerializer___Map_member_not_dictionary___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ClassMap.MapMember(typeof(TestMapping).GetProperty(nameof(TestMapping.IntIntTuple))).SetDictionarySerializer();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be true.\r\nParameter name: memberMustBeDictionaryImplementation");
        }

        [Fact]
        public static void SetDictionarySerializer___Correct___Works()
        {
            // Arrange
            var memberToMap = ClassMap.MapMember(typeof(TestMapping).GetProperty(nameof(TestMapping.EnumIntMap)));

            // Act
            var ret = memberToMap.SetDictionarySerializer();

            // Assert
            ret.Should().NotBeNull();
            var expectedSerializerType = typeof(DictionaryInterfaceImplementerSerializer<Dictionary<AnotherEnumeration, int>>);
            var actualSerializerType = ret.GetSerializer().GetType();
            actualSerializerType.GetGenericTypeDefinition().Should().Be(expectedSerializerType.GetGenericTypeDefinition());
            actualSerializerType.GetGenericArguments().Single().Should().Be(expectedSerializerType.GetGenericArguments().Single());
        }

        private static BsonClassMap ClassMap => new BsonClassMap(typeof(TestMapping));
    }
}
