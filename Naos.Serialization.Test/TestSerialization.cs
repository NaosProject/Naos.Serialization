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
                EnumerableOfDateTime = new[] { DateTime.UtcNow, },
                NullableEnumNull = null,
                NullableEnumWithValue = AnotherEnumeration.None,
            };

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TestWrappedFields;
                actual.Should().NotBeNull();
                actual.NullableDateTimeNull.Should().Be(expected.NullableDateTimeNull);
                actual.NullableDateTimeWithValue.Should().Be(expected.NullableDateTimeWithValue);
                actual.EnumerableOfDateTime.Should().Equal(expected.EnumerableOfDateTime);
                actual.NullableEnumNull.Should().Be(expected.NullableEnumNull);
                actual.NullableEnumWithValue.Should().Be(expected.NullableEnumWithValue);
                actual.EnumerableOfEnum.Should().Equal(expected.EnumerableOfEnum);
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer, bsonSerializer);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestWithInheritorExtraPropertyWrapper___Works()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<BsonConfigurationAutoRegisterType<BsonConfigurationAutoRegisterInherited>>();

            var inherit = new TestWithInheritor { Id = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString(), };
            var inheritWithProp = new TestWithInheritorExtraProperty { Id = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString(), AnotherName = Guid.NewGuid().ToString(), };
            var expected = new TestWithInheritorExtraPropertyWrapper { InheritorPropertyBase = inherit, InheritorPropertyExtended = inheritWithProp, };

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TestWithInheritorExtraPropertyWrapper;
                actual.Should().NotBeNull();
                actual.InheritorPropertyBase.Should().BeOfType<TestWithInheritor>();
                actual.InheritorPropertyBase.Id.Should().Be(expected.InheritorPropertyBase.Id);
                actual.InheritorPropertyBase.Name.Should().Be(expected.InheritorPropertyBase.Name);
                actual.InheritorPropertyExtended.Should().BeOfType<TestWithInheritorExtraProperty>();
                actual.InheritorPropertyExtended.Id.Should().Be(expected.InheritorPropertyExtended.Id);
                actual.InheritorPropertyExtended.Name.Should().Be(expected.InheritorPropertyExtended.Name);
                ((TestWithInheritorExtraProperty)actual.InheritorPropertyExtended).AnotherName.Should()
                    .Be(((TestWithInheritorExtraProperty)expected.InheritorPropertyExtended).AnotherName);
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
            }

            // Act & Assert
            ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer, bsonSerializer);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestDictionaryMapping___Works()
        {
            // Arrange
            var bsonSerializer = new NaosBsonSerializer<BsonConfigurationAutoRegisterType<TestDictionaryFields>>();

            var expected = new TestDictionaryFields
            {
                DictionaryStringString = A.Dummy<Dictionary<string, string>>(),
                IDictionaryStringString = A.Dummy<Dictionary<string, string>>(),
                ReadOnlyDictionaryStringString = A.Dummy<ReadOnlyDictionary<string, string>>(),
                IReadOnlyDictionaryStringString = A.Dummy<ReadOnlyDictionary<string, string>>(),
                ConcurrentDictionaryStringString = new ConcurrentDictionary<string, string>(A.Dummy<Dictionary<string, string>>()),
            };

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as TestDictionaryFields;
                actual.Should().NotBeNull();
                actual.DictionaryStringString.Should().Equal(expected.DictionaryStringString);
                actual.IDictionaryStringString.Should().Equal(expected.IDictionaryStringString);
                actual.ReadOnlyDictionaryStringString.Should().Equal(expected.ReadOnlyDictionaryStringString);
                actual.IReadOnlyDictionaryStringString.Should().Equal(expected.IReadOnlyDictionaryStringString);
                actual.ConcurrentDictionaryStringString.Should().Equal(expected.ConcurrentDictionaryStringString);
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
