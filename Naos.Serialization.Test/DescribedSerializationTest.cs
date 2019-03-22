// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DescribedSerializationTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Serialization.Domain;
    using Naos.Serialization.Factory.Extensions;

    using OBeautifulCode.TypeRepresentation;

    using Xunit;

    using static System.FormattableString;

    public static class DescribedSerializationTest
    {
        [Fact]
        public static void Constructor__Should_throw_ArgumentNullException___When_parameter_typeDescription_is_null()
        {
            // Arrange
            Action action = () => new DescribedSerialization(null, A.Dummy<string>(), A.Dummy<SerializationDescription>());

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("Parameter 'payloadTypeDescription' is null.");
        }

        [Fact]
        public static void Constructor__Should_throw_ArgumentException___When_parameter_SerializerDescription_is_null()
        {
            // Arrange
            Action action = () => new DescribedSerialization(A.Dummy<TypeDescription>(), string.Empty, null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("Parameter 'serializationDescription' is null.");
        }

        [Fact]
        public static void TypeDescription__Should_return_same_typeDescription_passed_to_constructor___When_getting()
        {
            // Arrange
            var typeDescription = A.Dummy<TypeDescription>();
            var payload = A.Dummy<string>();
            var serializer = A.Dummy<SerializationDescription>();
            var systemUnderTest = new DescribedSerialization(typeDescription, payload, serializer);

            // Act
            var actual = systemUnderTest.PayloadTypeDescription;

            // Assert
            actual.Should().BeSameAs(typeDescription);
        }

        [Fact]
        public static void Payload__Should_return_same_payload_passed_to_constructor___When_getting()
        {
            // Arrange
            var typeDescription = A.Dummy<TypeDescription>();
            var payload = A.Dummy<string>();
            var serializer = A.Dummy<SerializationDescription>();
            var systemUnderTest = new DescribedSerialization(typeDescription, payload, serializer);

            // Act
            var actual = systemUnderTest.SerializedPayload;

            // Assert
            actual.Should().Be(payload);
        }

        [Fact]
        public static void Serializer__Should_return_same_serializer_passed_to_constructor___When_getting()
        {
            // Arrange
            var typeDescription = A.Dummy<TypeDescription>();
            var payload = A.Dummy<string>();
            var serializer = A.Dummy<SerializationDescription>();
            var systemUnderTest = new DescribedSerialization(typeDescription, payload, serializer);

            // Act
            var actual = systemUnderTest.SerializationDescription;

            // Assert
            actual.Should().Be(serializer);
        }

        [Fact]
        public static void EqualityLogic___Should_be_valid___When_different_data()
        {
            // Arrange
            var typeDescription1 = typeof(string).ToTypeDescription();
            var typeDescription2 = typeof(decimal).ToTypeDescription();

            var payload1 = A.Dummy<string>();
            var payload2 = A.Dummy<string>();

            var serializationDescription1 = A.Dummy<SerializationDescription>();
            var serializationDescription2 = A.Dummy<SerializationDescription>();

            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new DescribedSerialization(typeDescription1, payload1, serializationDescription1),
                                                Second = new DescribedSerialization(typeDescription2, payload1, serializationDescription1),
                                            },
                                        new
                                            {
                                                First = new DescribedSerialization(typeDescription1, payload1, serializationDescription1),
                                                Second = new DescribedSerialization(typeDescription1, payload2, serializationDescription1),
                                            },
                                        new
                                            {
                                                First = new DescribedSerialization(typeDescription1, payload1, serializationDescription1),
                                                Second = new DescribedSerialization(typeDescription1, payload1, serializationDescription2),
                                            },
                                        new
                                            {
                                                First = new DescribedSerialization(typeDescription1, payload1, serializationDescription1),
                                                Second = (DescribedSerialization)null,
                                            },
                                        new
                                            {
                                                First = (DescribedSerialization)null,
                                                Second = new DescribedSerialization(typeDescription1, payload1, serializationDescription1),
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
            var typeDescription = typeof(string).ToTypeDescription();
            var serializedPayload = A.Dummy<string>();
            var serializationDescription = A.Dummy<SerializationDescription>();

            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new DescribedSerialization(typeDescription, serializedPayload, serializationDescription),
                                                Second = new DescribedSerialization(typeDescription, serializedPayload, serializationDescription),
                                            },
                                        new
                                            {
                                                First = (DescribedSerialization)null,
                                                Second = (DescribedSerialization)null,
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

        [Fact]
        public static void AnonymousObject___Can_be_round_tripped_back_into_a_dynamic()
        {
            // Arrange
            var input = new { Item = "item", Items = new[] { "item1", "item2" } };
            var serializationDescriptionJson = new SerializationDescription(SerializationFormat.Json, SerializationRepresentation.String, SerializationKind.Compact);
            var serializationDescriptionBson = new SerializationDescription(SerializationFormat.Bson, SerializationRepresentation.String, SerializationKind.Default);

            // Act
            var serializedJson = input.ToDescribedSerialization(serializationDescriptionJson);
            dynamic deserializedJson = serializedJson.DeserializePayload();

            var serializedBson = input.ToDescribedSerialization(serializationDescriptionBson);
            dynamic deserializedBson = serializedBson.DeserializePayload();

            // Assert
            ((string)deserializedJson.Item).Should().Be(input.Item);
            ((string)deserializedJson.Items[0]).Should().Be(input.Items[0]);
            ((string)deserializedJson.Items[1]).Should().Be(input.Items[1]);

            ((string)deserializedBson.Item).Should().Be(input.Item);
            ((string)deserializedBson.Items[0]).Should().Be(input.Items[0]);
            ((string)deserializedBson.Items[1]).Should().Be(input.Items[1]);
        }
    }
}