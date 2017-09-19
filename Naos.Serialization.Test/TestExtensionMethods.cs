// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using MongoDB.Bson.Serialization;

    using Naos.Serialization.Bson;

    using Xunit;

    public static class TestExtensionMethods
    {
        [Fact]
        public static void SetEnumStringSerializer___Map_is_null___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ExtensionMethods.SetEnumStringSerializer<TestEnumeration>(null);

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
            Func<BsonMemberMap> action = () => ClassMap.MapMember(typeof(TestMapping).GetProperty(nameof(TestMapping.StructProperty))).SetEnumStringSerializer<TestStruct>();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be true.\r\nParameter name: memberTypeIsEnumeration");
        }

        [Fact]
        public static void SetArraySerializer___Map_is_null___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ExtensionMethods.SetArraySerializer<string>(null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: map");
        }

        [Fact]
        public static void SetDictionarySerializer___Map_is_null___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ExtensionMethods.SetDictionarySerializer<string, string>(null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: map");
        }

        [Fact]
        public static void SetDictionarySerializer___Map_member_not_dictionary___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ClassMap.MapMember(typeof(TestMapping).GetProperty(nameof(TestMapping.StructProperty))).SetDictionarySerializer<string, string>();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be true.\r\nParameter name: memberMustBeDictionaryImplementationOfSpecifiedKeyAndValueTypes");
        }

        [Fact]
        public static void SetDictionarySerializer___Map_member_not_correct_types_in_dictionary___Throws()
        {
            // Arrange
            Func<BsonMemberMap> action = () => ClassMap.MapMember(typeof(TestMapping).GetProperty(nameof(TestMapping.StructProperty))).SetDictionarySerializer<string, string>();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be true.\r\nParameter name: memberMustBeDictionaryImplementationOfSpecifiedKeyAndValueTypes");
        }

        private static BsonClassMap ClassMap => new BsonClassMap(typeof(TestMapping));
    }

    public class TestMapping
    {
        public TestStruct StructProperty { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<int, int> IntIntMap { get; set; }
    }

    public struct TestStruct
    {
    }

    public enum TestEnumeration
    {
        /// <summary>
        /// No value specified.
        /// </summary>
        None,
    }
}
