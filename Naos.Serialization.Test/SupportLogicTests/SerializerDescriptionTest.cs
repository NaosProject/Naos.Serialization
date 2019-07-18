// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerDescriptionTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Compression.Domain;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using OBeautifulCode.Representation;

    using Xunit;

    using static System.FormattableString;

    public static class SerializerDescriptionTest
    {
        [Fact]
        public static void Constructor___Invalid_SerializationKind___Throws()
        {
            // Arrange
            Action action = () => new SerializationDescription(
                SerializationKind.Invalid,
                SerializationFormat.Binary);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Be("Parameter 'serializationKind' is equal to the comparison value using EqualityComparer<T>.Default, where T: SerializationKind.  Specified 'comparisonValue' is 'Invalid'.");
        }

        [Fact]
        public static void Constructor___Invalid_SerializationFormat___Throws()
        {
            // Arrange
            Action action = () => new SerializationDescription(
                SerializationKind.Bson,
                SerializationFormat.Invalid);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Be("Parameter 'serializationFormat' is equal to the comparison value using EqualityComparer<T>.Default, where T: SerializationFormat.  Specified 'comparisonValue' is 'Invalid'.");
        }

        [Fact]
        public static void Constructor___Invalid_CompressionKind___Throws()
        {
            // Arrange
            Action action = () => new SerializationDescription(
                SerializationKind.Bson,
                SerializationFormat.String,
                null,
                CompressionKind.Invalid);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Be("Parameter 'compressionKind' is equal to the comparison value using EqualityComparer<T>.Default, where T: CompressionKind.  Specified 'comparisonValue' is 'Invalid'.");
        }

        [Fact]
        public static void Equality___Interface___Implemented()
        {
            typeof(SerializationDescription).GetInterfaces().SingleOrDefault(_ => _ == typeof(IEquatable<SerializationDescription>)).Should().NotBeNull();
        }

        [Fact]
        public static void EqualityLogic___Should_be_valid___When_different_data()
        {
            // Arrange
            var typeRepresentation1 = typeof(string).ToRepresentation();
            var typeRepresentation2 = typeof(decimal).ToRepresentation();

            var metadata1 = new Dictionary<string, string> { { A.Dummy<string>(), A.Dummy<string>() } };
            var metadata1Plus = new Dictionary<string, string> { { A.Dummy<string>(), A.Dummy<string>() } };
            var metadata2 = new Dictionary<string, string> { { A.Dummy<string>(), A.Dummy<string>() } };

            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new SerializationDescription(SerializationKind.Bson, SerializationFormat.Binary, typeRepresentation1, CompressionKind.DotNetZip, metadata1),
                                                Second = new SerializationDescription(SerializationKind.Bson, SerializationFormat.Binary, typeRepresentation1, CompressionKind.DotNetZip, metadata1Plus),
                                            },
                                        new
                                            {
                                                First = new SerializationDescription(SerializationKind.Bson, SerializationFormat.Binary, typeRepresentation1, CompressionKind.DotNetZip, metadata1),
                                                Second = new SerializationDescription(SerializationKind.Bson, SerializationFormat.Binary, typeRepresentation1, CompressionKind.DotNetZip, metadata2),
                                            },
                                        new
                                            {
                                                First = new SerializationDescription(SerializationKind.Bson, SerializationFormat.Binary, typeRepresentation1, CompressionKind.DotNetZip),
                                                Second = new SerializationDescription(SerializationKind.Bson, SerializationFormat.Binary, typeRepresentation1, CompressionKind.None),
                                            },
                                        new
                                            {
                                                First = new SerializationDescription(SerializationKind.Bson, SerializationFormat.Binary, typeRepresentation1),
                                                Second = new SerializationDescription(SerializationKind.Json, SerializationFormat.Binary, typeRepresentation1),
                                            },
                                        new
                                            {
                                                First = new SerializationDescription(SerializationKind.Bson, SerializationFormat.Binary, typeRepresentation1),
                                                Second = new SerializationDescription(SerializationKind.Bson, SerializationFormat.String, typeRepresentation1),
                                            },
                                        new
                                            {
                                                First = new SerializationDescription(SerializationKind.Bson, SerializationFormat.Binary, typeRepresentation1),
                                                Second = new SerializationDescription(SerializationKind.Bson, SerializationFormat.Binary, typeRepresentation2),
                                            },
                                        new
                                            {
                                                First = new SerializationDescription(SerializationKind.Bson, SerializationFormat.Binary, typeRepresentation1),
                                                Second = (SerializationDescription)null,
                                            },
                                        new
                                            {
                                                First = (SerializationDescription)null,
                                                Second = new SerializationDescription(SerializationKind.Bson, SerializationFormat.Binary, typeRepresentation2),
                                            },
                                    }.ToList();

            // Act & Assert
            notEqualTests.ForEach(
                _ =>
                    {
                        if (_.First != null && _.Second != null)
                        {
                            (_.First.GetHashCode() == _.Second.GetHashCode()).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                            _.First.Equals(_.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                            _.First.Equals((object)_.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                        }

                        (_.First == _.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                        (_.First != _.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                    });
        }

        [Fact]
        public static void EqualityLogic___Should_be_valid___When_same_data()
        {
            // Arrange
            var typeRepresentation = typeof(string).ToRepresentation();
            var serializationKind = SerializationKind.Bson;
            var serializationRepresentation = SerializationFormat.Binary;
            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new SerializationDescription(serializationKind, serializationRepresentation, typeRepresentation),
                                                Second = new SerializationDescription(serializationKind, serializationRepresentation, typeRepresentation),
                                            },
                                        new
                                            {
                                                First = (SerializationDescription)null,
                                                Second = (SerializationDescription)null,
                                            },
                                    }.ToList();

            // Act & Assert
            notEqualTests.ForEach(
                _ =>
                    {
                        if (_.First != null && _.Second != null)
                        {
                            _.First.Equals(_.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                            _.First.Equals((object)_.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                            (_.First.GetHashCode() == _.Second.GetHashCode()).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                        }

                        (_.First == _.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                        (_.First != _.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                    });
        }
    }
}
