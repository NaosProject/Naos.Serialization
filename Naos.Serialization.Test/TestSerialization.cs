// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSerialization.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;

    using Xunit;

    using static System.FormattableString;

    public static class TestSerialization
    {
        private static readonly NaosJsonSerializer JsonSerializerToUse = new NaosJsonSerializer();

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestWrappedFields___Works()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<BsonConfigurationAutoRegisterType<TestWrappedFields>>();

            var expected = new TestWrappedFields
            {
                NullableDateTimeNull = null,
                NullableDateTimeWithValue = DateTime.UtcNow,
                CollectionOfDateTime = new[] { DateTime.UtcNow, },
                NullableEnumNull = null,
                NullableEnumWithValue = AnotherEnumeration.None,
            };

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TestWrappedFields;
                actual.Should().NotBeNull();
                actual.NullableDateTimeNull.Should().Be(expected.NullableDateTimeNull);
                actual.NullableDateTimeWithValue.Should().Be(expected.NullableDateTimeWithValue);
                actual.CollectionOfDateTime.Should().Equal(expected.CollectionOfDateTime);
                actual.NullableEnumNull.Should().Be(expected.NullableEnumNull);
                actual.NullableEnumWithValue.Should().Be(expected.NullableEnumWithValue);
                actual.EnumerableOfEnum.Should().Equal(expected.EnumerableOfEnum);
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer, bsonSerializer);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestWithInheritor___Works()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<BsonConfigurationAutoRegisterType<TestWithInheritorExtraProperty>>();

            var expected = new TestWithInheritorExtraProperty { Id = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString(), AnotherName = Guid.NewGuid().ToString(), };

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TestWithInheritorExtraProperty;
                actual.Should().NotBeNull();
                actual.Id.Should().Be(expected.Id);
                actual.Name.Should().Be(expected.Name);
                actual.AnotherName.Should().Be(expected.AnotherName);
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer, bsonSerializer);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestWithId___Works()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<BsonConfigurationAutoRegisterType<TestWithId>>();

            var expected = new TestWithId { Id = Guid.NewGuid().ToString(), };

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TestWithId;
                actual.Should().NotBeNull();
                actual.Id.Should().Be(expected.Id);
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer, bsonSerializer);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestMapping_with_all_defaults___Works()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<BsonConfigurationAutoRegisterType<TestMapping>>();

            var expected = new TestMapping();

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TestMapping;
                actual.Should().NotBeNull();
                actual.StringProperty.Should().BeNull();
                actual.IntProperty.Should().Be(default(int));
                actual.DateTimePropertyUtc.Should().Be(default(DateTime));
                actual.DateTimePropertyLocal.Should().Be(default(DateTime));
                actual.DateTimePropertyUnspecified.Should().Be(default(DateTime));
                actual.GuidProperty.Should().Be(Guid.Empty);
                actual.NonEnumArray.Should().BeNull();
                actual.EnumArray.Should().BeNull();
                actual.StringIntMap.Should().BeNull();
                actual.EnumIntMap.Should().BeNull();
                actual.IntIntTuple.Should().BeNull();
                actual.EnumProperty.Should().Be(TestEnumeration.None);
                actual.IntArray.Should().BeNull();
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer, bsonSerializer);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestMapping___Works()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<BsonConfigurationAutoRegisterType<TestMapping>>();

            var expected = new TestMapping
                               {
                                   StringProperty = Guid.NewGuid().ToString(),
                                   IntProperty = A.Dummy<int>(),
                                   DateTimePropertyUtc = DateTime.UtcNow,
                                   DateTimePropertyLocal = DateTime.UtcNow.ToLocalTime(),
                                   DateTimePropertyUnspecified = DateTime.UtcNow.ToUnspecified(),
                                   GuidProperty = Guid.NewGuid(),
                                   NonEnumArray = new[] { A.Dummy<string>() },
                                   EnumArray = new[] { A.Dummy<TestEnumeration>(), },
                                   StringIntMap = new Dictionary<string, int> { { "key", A.Dummy<int>() } },
                                   EnumIntMap = new Dictionary<AnotherEnumeration, int> { { A.Dummy<AnotherEnumeration>(), A.Dummy<int>() } },
                                   IntIntTuple = new Tuple<int, int>(3, 4),
                                   EnumProperty = A.Dummy<TestEnumeration>(),
                                   IntArray = A.Dummy<int[]>(),
                                };

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TestMapping;
                actual.Should().NotBeNull();
                actual.StringProperty.Should().Be(expected.StringProperty);
                actual.IntProperty.Should().Be(expected.IntProperty);
                actual.DateTimePropertyUtc.Kind.Should().Be(DateTimeKind.Utc);
                actual.DateTimePropertyUtc.Should().Be(expected.DateTimePropertyUtc);
                actual.DateTimePropertyLocal.Should().Be(expected.DateTimePropertyLocal);
                actual.DateTimePropertyLocal.Kind.Should().Be(DateTimeKind.Local);
                actual.DateTimePropertyUnspecified.Should().Be(expected.DateTimePropertyUnspecified);
                actual.DateTimePropertyUnspecified.Kind.Should().Be(DateTimeKind.Unspecified);
                actual.GuidProperty.Should().Be(expected.GuidProperty);
                actual.NonEnumArray.Single().Should().Be(expected.NonEnumArray.Single());
                actual.EnumArray.Single().Should().Be(expected.EnumArray.Single());
                actual.StringIntMap.Single().Should().Be(expected.StringIntMap.Single());
                actual.EnumIntMap.Single().Should().Be(expected.EnumIntMap.Single());
                actual.IntIntTuple.Should().Be(expected.IntIntTuple);
                actual.EnumProperty.Should().Be(expected.EnumProperty);
                actual.IntArray.Should().Equal(expected.IntArray);
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer, bsonSerializer);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestDictionaryMapping_with_all_nulls___Works()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<BsonConfigurationAutoRegisterType<TestDictionaryFields>>();

            var expected = new TestDictionaryFields();

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TestDictionaryFields;
                actual.Should().NotBeNull();
                actual.DictionaryStringString.Should().BeNull();
                actual.IDictionaryStringString.Should().BeNull();
                actual.ReadOnlyDictionaryStringString.Should().BeNull();
                actual.IReadOnlyDictionaryStringString.Should().BeNull();
                actual.ConcurrentDictionaryStringString.Should().BeNull();
                actual.ReadOnlyDictionaryStringInt.Should().BeNull();
                actual.ReadOnlyDictionaryIntString.Should().BeNull();
                actual.IDictionaryEnumString.Should().BeNull();
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer, bsonSerializer);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestDictionaryMapping___Works()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<BsonConfigurationAutoRegisterType<TestDictionaryFields>>();

            var expected = A.Dummy<TestDictionaryFields>();

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TestDictionaryFields;
                actual.Should().NotBeNull();
                actual.DictionaryStringString.Should().Equal(expected.DictionaryStringString);
                actual.IDictionaryStringString.Should().Equal(expected.IDictionaryStringString);
                actual.ReadOnlyDictionaryStringString.Should().Equal(expected.ReadOnlyDictionaryStringString);
                actual.IReadOnlyDictionaryStringString.Should().Equal(expected.IReadOnlyDictionaryStringString);
                actual.ConcurrentDictionaryStringString.Should().Equal(expected.ConcurrentDictionaryStringString);
                actual.ReadOnlyDictionaryStringInt.Should().Equal(expected.ReadOnlyDictionaryStringInt);
                actual.ReadOnlyDictionaryIntString.Should().Equal(expected.ReadOnlyDictionaryIntString);
                actual.IDictionaryEnumString.Should().Equal(expected.IDictionaryEnumString);
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer, bsonSerializer);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestCollectionFields_with_all_nulls___Works()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<BsonConfigurationAutoRegisterType<TestCollectionFields>>();

            var expected = new TestCollectionFields();

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TestCollectionFields;
                actual.Should().NotBeNull();
                actual.ReadOnlyCollectionDateTime.Should().BeNull();
                actual.ICollectionDateTime.Should().BeNull();
                actual.IListEnum.Should().BeNull();
                actual.IReadOnlyListString.Should().BeNull();
                actual.IReadOnlyCollectionGuid.Should().BeNull();
                actual.ListDateTime.Should().BeNull();
                actual.CollectionGuid.Should().BeNull();
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer, bsonSerializer);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestCollectionFields___Works()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<BsonConfigurationAutoRegisterType<TestCollectionFields>>();

            var expected = A.Dummy<TestCollectionFields>();

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TestCollectionFields;
                actual.Should().NotBeNull();
                actual.ReadOnlyCollectionDateTime.Should().Equal(expected.ReadOnlyCollectionDateTime);
                actual.ICollectionDateTime.Should().Equal(expected.ICollectionDateTime);
                actual.IListEnum.Should().Equal(expected.IListEnum);
                actual.IReadOnlyListString.Should().Equal(expected.IReadOnlyListString);
                actual.IReadOnlyCollectionGuid.Should().Equal(expected.IReadOnlyCollectionGuid);
                actual.ListDateTime.Should().Equal(expected.ListDateTime);
                actual.CollectionGuid.Should().Equal(expected.CollectionGuid);
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer, bsonSerializer);
        }

        private static void ActAndAssertForRoundtripSerialization<T>(object expected, Action<object> throwIfObjectsDiffer, NaosBsonSerializer<BsonConfigurationAutoRegisterType<T>> bsonSerializer)
        {
            var stringSerializers = new IStringSerializeAndDeserialize[] { JsonSerializerToUse, bsonSerializer };
            var binarySerializers = new IBinarySerializeAndDeserialize[] { JsonSerializerToUse, bsonSerializer };

            foreach (var stringSerializer in stringSerializers)
            {
                var actualString = stringSerializer.SerializeToString(expected);
                var actualObject = stringSerializer.Deserialize(actualString, expected.GetType());

                try
                {
                    throwIfObjectsDiffer(actualObject);
                }
                catch (Exception ex)
                {
                    throw new NaosSerializationException(Invariant($"Failure with {nameof(stringSerializer)} - {stringSerializer.GetType()}"), ex);
                }
            }

            foreach (var binarySerializer in binarySerializers)
            {
                var actualBytes = binarySerializer.SerializeToBytes(expected);
                var actualObject = binarySerializer.Deserialize(actualBytes, expected.GetType());

                try
                {
                    throwIfObjectsDiffer(actualObject);
                }
                catch (Exception ex)
                {
                    throw new NaosSerializationException(Invariant($"Failure with {nameof(binarySerializer)} - {binarySerializer.GetType()}"), ex);
                }
            }
        }
    }
}
