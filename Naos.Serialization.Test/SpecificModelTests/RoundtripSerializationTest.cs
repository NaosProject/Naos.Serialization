// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoundtripSerializationTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using OBeautifulCode.Type;
    using Xunit;

    using static System.FormattableString;

    public static class RoundtripSerializationTest
    {
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Onlys", Justification = "Spelling/name is correct.")]
        [Fact]
        public static void RoundtripSerializeDeserialize___Using_ClassWithGetterOnlysBase___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<ClassWithGetterOnlys>);
            var jsonConfigType = typeof(GenericJsonConfiguration<ClassWithGetterOnlys>);

            var expected = new ClassWithGetterOnlys();

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, ClassWithGetterOnlys deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.GetMyEnumFromBase.Should().Be(expected.GetMyEnumFromBase);
                deserialized.GetMyEnumFromThis.Should().Be(expected.GetMyEnumFromThis);
                deserialized.GetMyStringFromBase.Should().Be(expected.GetMyStringFromBase);
                deserialized.GetMyStringFromThis.Should().Be(expected.GetMyStringFromThis);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unconfigured", Justification = "Spelling/name is correct.")]
        [Fact]
        public static void RoundtripSerializeDeserialize___Using_Unconfigured_Bson___Works()
        {
            // Arrange
            var expected = A.Dummy<VanillaClass>();

            // Act & Assert
            expected.RoundtripSerializeWithEquatableAssertion(false);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_ClassWithPrivateSetter___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<ClassWithPrivateSetter>);
            var jsonConfigType = typeof(GenericJsonConfiguration<ClassWithPrivateSetter>);

            var privateValue = A.Dummy<string>();
            var expected = new ClassWithPrivateSetter(privateValue);

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, ClassWithPrivateSetter deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.PrivateValue.Should().Be(expected.PrivateValue);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestWrappedFields___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestWrappedFields>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestWrappedFields>);

            var expected = new TestWrappedFields
            {
                NullableDateTimeNull = null,
                NullableDateTimeWithValue = DateTime.UtcNow,
                CollectionOfDateTime = new[] { DateTime.UtcNow, },
                NullableEnumNull = null,
                NullableEnumWithValue = AnotherEnumeration.None,
            };

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestWrappedFields deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.NullableDateTimeNull.Should().Be(expected.NullableDateTimeNull);
                deserialized.NullableDateTimeWithValue.Should().Be(expected.NullableDateTimeWithValue);
                deserialized.CollectionOfDateTime.Should().Equal(expected.CollectionOfDateTime);
                deserialized.NullableEnumNull.Should().Be(expected.NullableEnumNull);
                deserialized.NullableEnumWithValue.Should().Be(expected.NullableEnumWithValue);
                deserialized.EnumerableOfEnum.Should().Equal(expected.EnumerableOfEnum);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestWithInheritor___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestWithInheritorExtraProperty>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestWithInheritorExtraProperty>);

            var expected = new TestWithInheritorExtraProperty { Id = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString(), AnotherName = Guid.NewGuid().ToString(), };

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestWithInheritorExtraProperty deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.Id.Should().Be(expected.Id);
                deserialized.Name.Should().Be(expected.Name);
                deserialized.AnotherName.Should().Be(expected.AnotherName);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestWithId___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestWithId>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestWithId>);

            var expected = new TestWithId { Id = Guid.NewGuid().ToString(), };

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestWithId deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.Id.Should().Be(expected.Id);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestMapping_with_all_defaults___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestMapping>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestMapping>);

            var expected = new TestMapping();

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestMapping deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.StringProperty.Should().BeNull();
                deserialized.IntProperty.Should().Be(default(int));
                deserialized.DateTimePropertyUtc.Should().Be(default(DateTime));
                deserialized.DateTimePropertyLocal.Should().Be(default(DateTime));
                deserialized.DateTimePropertyUnspecified.Should().Be(default(DateTime));
                deserialized.GuidProperty.Should().Be(Guid.Empty);
                deserialized.NonEnumArray.Should().BeNull();
                deserialized.EnumArray.Should().BeNull();
                deserialized.StringIntMap.Should().BeNull();
                deserialized.EnumIntMap.Should().BeNull();
                deserialized.IntIntTuple.Should().BeNull();
                deserialized.EnumProperty.Should().Be(TestEnumeration.None);
                deserialized.IntArray.Should().BeNull();
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestMapping___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestMapping>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestMapping>);

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
                EnumIntMap = new Dictionary<AnotherEnumeration, int>
                    { { A.Dummy<AnotherEnumeration>(), A.Dummy<int>() } },
                IntIntTuple = new Tuple<int, int>(3, 4),
                EnumProperty = A.Dummy<TestEnumeration>(),
                IntArray = A.Dummy<int[]>(),
            };

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestMapping deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.StringProperty.Should().Be(expected.StringProperty);
                deserialized.IntProperty.Should().Be(expected.IntProperty);
                deserialized.DateTimePropertyUtc.Kind.Should().Be(DateTimeKind.Utc);
                deserialized.DateTimePropertyUtc.Should().Be(expected.DateTimePropertyUtc);
                deserialized.DateTimePropertyLocal.Should().Be(expected.DateTimePropertyLocal);
                deserialized.DateTimePropertyLocal.Kind.Should().Be(DateTimeKind.Local);
                deserialized.DateTimePropertyUnspecified.Should().Be(expected.DateTimePropertyUnspecified);
                deserialized.DateTimePropertyUnspecified.Kind.Should().Be(DateTimeKind.Unspecified);
                deserialized.GuidProperty.Should().Be(expected.GuidProperty);
                deserialized.NonEnumArray.Single().Should().Be(expected.NonEnumArray.Single());
                deserialized.EnumArray.Single().Should().Be(expected.EnumArray.Single());
                deserialized.StringIntMap.Single().Should().Be(expected.StringIntMap.Single());
                deserialized.EnumIntMap.Single().Should().Be(expected.EnumIntMap.Single());
                deserialized.IntIntTuple.Should().Be(expected.IntIntTuple);
                deserialized.EnumProperty.Should().Be(expected.EnumProperty);
                deserialized.IntArray.Should().Equal(expected.IntArray);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestDictionaryMapping_with_all_nulls___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestDictionaryFields>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestDictionaryFields>);

            var expected = new TestDictionaryFields();

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestDictionaryFields deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.DictionaryStringString.Should().BeNull();
                deserialized.IDictionaryStringString.Should().BeNull();
                deserialized.ReadOnlyDictionaryStringString.Should().BeNull();
                deserialized.IReadOnlyDictionaryStringString.Should().BeNull();
                deserialized.ConcurrentDictionaryStringString.Should().BeNull();
                deserialized.ReadOnlyDictionaryStringInt.Should().BeNull();
                deserialized.ReadOnlyDictionaryIntString.Should().BeNull();
                deserialized.IDictionaryEnumString.Should().BeNull();
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestDictionaryMapping___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestDictionaryFields>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestDictionaryFields>);

            var expected = A.Dummy<TestDictionaryFields>();

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestDictionaryFields deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.DictionaryStringString.Should().Equal(expected.DictionaryStringString);
                deserialized.IDictionaryStringString.Should().Equal(expected.IDictionaryStringString);
                deserialized.ReadOnlyDictionaryStringString.Should().Equal(expected.ReadOnlyDictionaryStringString);
                deserialized.IReadOnlyDictionaryStringString.Should().Equal(expected.IReadOnlyDictionaryStringString);
                deserialized.ConcurrentDictionaryStringString.Should().Equal(expected.ConcurrentDictionaryStringString);
                deserialized.ReadOnlyDictionaryStringInt.Should().Equal(expected.ReadOnlyDictionaryStringInt);
                deserialized.ReadOnlyDictionaryIntString.Should().Equal(expected.ReadOnlyDictionaryIntString);
                deserialized.IDictionaryEnumString.Should().Equal(expected.IDictionaryEnumString);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestDictionaryMixedKeyValues_with_all_nulls___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestDictionaryMixedKeyValues>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestDictionaryMixedKeyValues>);

            var expected = new TestDictionaryMixedKeyValues();

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestDictionaryMixedKeyValues deserialized)
            {
                deserialized.Should().NotBeNull();

                deserialized.DictionaryBaseString.Should().BeNull();
                deserialized.IDictionaryBaseString.Should().BeNull();
                deserialized.ReadOnlyDictionaryBaseString.Should().BeNull();
                deserialized.IReadOnlyDictionaryBaseString.Should().BeNull();
                deserialized.ConcurrentDictionaryBaseString.Should().BeNull();

                deserialized.DictionaryStringConstructor.Should().BeNull();
                deserialized.IDictionaryStringConstructor.Should().BeNull();
                deserialized.ReadOnlyDictionaryStringConstructor.Should().BeNull();
                deserialized.IReadOnlyDictionaryStringConstructor.Should().BeNull();
                deserialized.ConcurrentDictionaryStringConstructor.Should().BeNull();
                
                deserialized.DictionaryConstructorEnum.Should().BeNull();
                deserialized.IDictionaryConstructorEnum.Should().BeNull();
                deserialized.ReadOnlyDictionaryConstructorEnum.Should().BeNull();
                deserialized.IReadOnlyDictionaryConstructorEnum.Should().BeNull();
                deserialized.ConcurrentDictionaryConstructorEnum.Should().BeNull();

                deserialized.DictionaryEnumBase.Should().BeNull();
                deserialized.IDictionaryEnumBase.Should().BeNull();
                deserialized.ReadOnlyDictionaryEnumBase.Should().BeNull();
                deserialized.IReadOnlyDictionaryEnumBase.Should().BeNull();
                deserialized.ConcurrentDictionaryEnumBase.Should().BeNull();

                deserialized.DictionaryIntPoco.Should().BeNull();
                deserialized.IDictionaryIntPoco.Should().BeNull();
                deserialized.ReadOnlyDictionaryIntPoco.Should().BeNull();
                deserialized.IReadOnlyDictionaryIntPoco.Should().BeNull();
                deserialized.ConcurrentDictionaryIntPoco.Should().BeNull();

                deserialized.DictionaryPocoInt.Should().BeNull();
                deserialized.IDictionaryPocoInt.Should().BeNull();
                deserialized.ReadOnlyDictionaryPocoInt.Should().BeNull();
                deserialized.IReadOnlyDictionaryPocoInt.Should().BeNull();
                deserialized.ConcurrentDictionaryPocoInt.Should().BeNull();
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestDictionaryMixedKeyValues___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestDictionaryMixedKeyValues>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestDictionaryMixedKeyValues>);

            var expected = A.Dummy<TestDictionaryMixedKeyValues>();

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestDictionaryMixedKeyValues deserialized)
            {
                deserialized.Should().NotBeNull();

                deserialized.DictionaryBaseString.Should().Equal(expected.DictionaryBaseString);
                deserialized.IDictionaryBaseString.Should().Equal(expected.IDictionaryBaseString);
                deserialized.ReadOnlyDictionaryBaseString.Should().Equal(expected.ReadOnlyDictionaryBaseString);
                deserialized.IReadOnlyDictionaryBaseString.Should().Equal(expected.IReadOnlyDictionaryBaseString);
                deserialized.ConcurrentDictionaryBaseString.Should().Equal(expected.ConcurrentDictionaryBaseString);

                deserialized.DictionaryStringConstructor.Should().Equal(expected.DictionaryStringConstructor);
                deserialized.IDictionaryStringConstructor.Should().Equal(expected.IDictionaryStringConstructor);
                deserialized.ReadOnlyDictionaryStringConstructor.Should().Equal(expected.ReadOnlyDictionaryStringConstructor);
                deserialized.IReadOnlyDictionaryStringConstructor.Should().Equal(expected.IReadOnlyDictionaryStringConstructor);
                deserialized.ConcurrentDictionaryStringConstructor.Should().Equal(expected.ConcurrentDictionaryStringConstructor);

                deserialized.DictionaryConstructorEnum.Should().Equal(expected.DictionaryConstructorEnum);
                deserialized.IDictionaryConstructorEnum.Should().Equal(expected.IDictionaryConstructorEnum);
                deserialized.ReadOnlyDictionaryConstructorEnum.Should().Equal(expected.ReadOnlyDictionaryConstructorEnum);
                deserialized.IReadOnlyDictionaryConstructorEnum.Should().Equal(expected.IReadOnlyDictionaryConstructorEnum);
                deserialized.ConcurrentDictionaryConstructorEnum.Should().Equal(expected.ConcurrentDictionaryConstructorEnum);

                deserialized.DictionaryEnumBase.Should().Equal(expected.DictionaryEnumBase);
                deserialized.IDictionaryEnumBase.Should().Equal(expected.IDictionaryEnumBase);
                deserialized.ReadOnlyDictionaryEnumBase.Should().Equal(expected.ReadOnlyDictionaryEnumBase);
                deserialized.IReadOnlyDictionaryEnumBase.Should().Equal(expected.IReadOnlyDictionaryEnumBase);
                deserialized.ConcurrentDictionaryEnumBase.Should().Equal(expected.ConcurrentDictionaryEnumBase);

                deserialized.DictionaryIntPoco.Should().Equal(expected.DictionaryIntPoco);
                deserialized.IDictionaryIntPoco.Should().Equal(expected.IDictionaryIntPoco);
                deserialized.ReadOnlyDictionaryIntPoco.Should().Equal(expected.ReadOnlyDictionaryIntPoco);
                deserialized.IReadOnlyDictionaryIntPoco.Should().Equal(expected.IReadOnlyDictionaryIntPoco);
                deserialized.ConcurrentDictionaryIntPoco.Should().Equal(expected.ConcurrentDictionaryIntPoco);

                deserialized.DictionaryPocoInt.Should().Equal(expected.DictionaryPocoInt);
                deserialized.IDictionaryPocoInt.Should().Equal(expected.IDictionaryPocoInt);
                deserialized.ReadOnlyDictionaryPocoInt.Should().Equal(expected.ReadOnlyDictionaryPocoInt);
                deserialized.IReadOnlyDictionaryPocoInt.Should().Equal(expected.IReadOnlyDictionaryPocoInt);
                deserialized.ConcurrentDictionaryPocoInt.Should().Equal(expected.ConcurrentDictionaryPocoInt);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestCollectionFields_with_all_nulls___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestCollectionFields>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestCollectionFields>);

            var expected = new TestCollectionFields();

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestCollectionFields deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.ReadOnlyCollectionDateTime.Should().BeNull();
                deserialized.ICollectionDateTime.Should().BeNull();
                deserialized.IListEnum.Should().BeNull();
                deserialized.IReadOnlyListString.Should().BeNull();
                deserialized.IReadOnlyCollectionGuid.Should().BeNull();
                deserialized.ListDateTime.Should().BeNull();
                deserialized.CollectionGuid.Should().BeNull();
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestCollectionFields___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestCollectionFields>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestCollectionFields>);

            var expected = A.Dummy<TestCollectionFields>();

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestCollectionFields deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.ReadOnlyCollectionDateTime.Should().Equal(expected.ReadOnlyCollectionDateTime);
                deserialized.ICollectionDateTime.Should().Equal(expected.ICollectionDateTime);
                deserialized.IListEnum.Should().Equal(expected.IListEnum);
                deserialized.IReadOnlyListString.Should().Equal(expected.IReadOnlyListString);
                deserialized.IReadOnlyCollectionGuid.Should().Equal(expected.IReadOnlyCollectionGuid);
                deserialized.ListDateTime.Should().Equal(expected.ListDateTime);
                deserialized.CollectionGuid.Should().Equal(expected.CollectionGuid);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_collection_of_Interface_type___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<IDeduceWhoLetTheDogsOut>);
            var jsonConfigType = typeof(GenericJsonConfiguration<IDeduceWhoLetTheDogsOut>);

            IDeduceWhoLetTheDogsOut investigator1 = new NamedInvestigator("bob", 2);
            IDeduceWhoLetTheDogsOut investigator2 = new AnonymousInvestigator(4000);

            var expected = new Investigation
            {
                Investigators = new[] { investigator1, investigator2 },
            };

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, Investigation deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.Investigators.Count.Should().Be(2);

                var namedInvestigator = deserialized.Investigators.ElementAt(0) as NamedInvestigator;
                namedInvestigator.Should().NotBeNull();
                namedInvestigator.Name.Should().Be("bob");
                namedInvestigator.YearsOfPractice.Should().Be(2);

                var anonymousInvestigator = deserialized.Investigators.ElementAt(1) as AnonymousInvestigator;
                anonymousInvestigator.Should().NotBeNull();
                anonymousInvestigator.Fee.Should().Be(4000);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_SerializationDescription___Works()
        {
            // Arrange
            var expected = A.Dummy<SerializationDescription>();

            // Act & Assert
            expected.RoundtripSerializeWithEquatableAssertion(false);
        }

        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Spelling/name is correct.")]
        [Fact]
        public static void RoundtripSerializeDeserialize___Using_ClassWithFlagsEnums___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<ClassWithFlagsEnums>);
            var jsonConfigType = typeof(GenericJsonConfiguration<ClassWithFlagsEnums>);

            var expected = new ClassWithFlagsEnums { Flags = FlagsEnumeration.SecondValue | FlagsEnumeration.ThirdValue };

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, ClassWithFlagsEnums deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.Flags.Should().Be(expected.Flags);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestWithDictionaryKeyedOnEnum___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestWithDictionaryKeyedOnEnum>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestWithDictionaryKeyedOnEnum>);

            var expected = A.Dummy<TestWithDictionaryKeyedOnEnum>();

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestWithDictionaryKeyedOnEnum deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.TestDictionary.Should().NotBeNull();
                deserialized.TestDictionary.Count.Should().NotBe(0);
                deserialized.TestDictionary.Should().Equal(expected.TestDictionary);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestWithEmptyReadOnlyCollectionOfBaseClass___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestWithReadOnlyCollectionOfBaseClass>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestWithReadOnlyCollectionOfBaseClass>);

            var expected = new TestWithReadOnlyCollectionOfBaseClass { TestCollection = new List<TestBase>() };

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestWithReadOnlyCollectionOfBaseClass deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.TestCollection.Should().NotBeNull();
                deserialized.TestCollection.Count.Should().Be(0);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestWithReadOnlyCollectionOfBaseClass___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestWithReadOnlyCollectionOfBaseClass>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestWithReadOnlyCollectionOfBaseClass>);

            var expected = new TestWithReadOnlyCollectionOfBaseClass
                               {
                                   TestCollection =
                                       new TestBase[]
                                           {
                                               new TestImplementationOne { One = "1", Message = "1 one" },
                                               new TestImplementationTwo { Two = "2", Message = "2 two" },
                                           },
                               };

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestWithReadOnlyCollectionOfBaseClass deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.TestCollection.Should().NotBeNull();
                deserialized.TestCollection.Count.Should().Be(2);
                deserialized.TestCollection.First().GetType().Should().Be(expected.TestCollection.First().GetType());
                deserialized.TestCollection.Skip(1).First().GetType().Should().Be(expected.TestCollection.Skip(1).First().GetType());
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestWithDictionaryOfEnumToReadOnlyCollectionOfEnums___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<TestWithDictionaryOfEnumToReadOnlyCollectionOfEnums>);
            var jsonConfigType = typeof(GenericJsonConfiguration<TestWithDictionaryOfEnumToReadOnlyCollectionOfEnums>);

            var expected = new TestWithDictionaryOfEnumToReadOnlyCollectionOfEnums
            {
                TestDictionary = new Dictionary<TestEnumeration, IReadOnlyCollection<AnotherEnumeration>>
                {
                    {
                        TestEnumeration.TestFirst, null
                    },
                    {
                        TestEnumeration.TestSecond, new AnotherEnumeration[0]
                    },
                    {
                        TestEnumeration.TestThird, new[] { AnotherEnumeration.AnotherFirst, AnotherEnumeration.AnotherSecond }
                    },
                },
            };

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestWithDictionaryOfEnumToReadOnlyCollectionOfEnums deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.TestDictionary.Should().NotBeNull();
                deserialized.TestDictionary.Count.Should().Be(3);
                deserialized.TestDictionary.OrderBy(_ => _.Key).First().GetType().Should().Be(expected.TestDictionary.OrderBy(_ => _.Key).First().GetType());
                deserialized.TestDictionary.OrderBy(_ => _.Key).Skip(1).First().GetType().Should().Be(expected.TestDictionary.OrderBy(_ => _.Key).Skip(1).First().GetType());
                deserialized.TestDictionary.OrderBy(_ => _.Key).Skip(2).First().GetType().Should().Be(expected.TestDictionary.OrderBy(_ => _.Key).Skip(2).First().GetType());
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "atop", Justification = "Spelling/name is correct.")]
        [Fact]
        public static void When_registering_a_top_level_interface___An_implementation_of_an_interface_that_implements_top_interface___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<ITopInterface>);
            var jsonConfigType = typeof(GenericJsonConfiguration<ITopInterface>);

            var expected = A.Dummy<BottomClass>();

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, BottomClass deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.Name.Should().Be(expected.Name);
                deserialized.Species.Should().Be(expected.Species);
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }

        [Fact]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FieldWorks", Justification = "This is spelled correctly.")]
        public static void RoundtripSerializeDeserialize___Using_Field_NumberField_YearField__Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericBsonConfiguration<Field>);
            var jsonConfigType = typeof(GenericJsonConfiguration<Field>);

            var expectedId1 = "my-field-1";
            var expectedId2 = "my-field-2";

            var expectedTitle1 = "my-title-1";
            var expectedTitle2 = "my-title-2";

            var expectedNumberOfDecimalPlaces1 = 2;
            var expectedNumberOfDecimalPlaces2 = 4;

            var expected1 = new NumberField(expectedId1)
            {
                Title = expectedTitle1,
                NumberOfDecimalPlaces = expectedNumberOfDecimalPlaces1,
            };

            var expected2 = new YearField(expectedId2)
            {
                Title = expectedTitle2,
                NumberOfDecimalPlaces = expectedNumberOfDecimalPlaces2,
            };

            void ThrowIfObjectsDiffer1(DescribedSerialization serialized, NumberField deserialized)
            {
                (deserialized is YearField).Should().BeFalse();

                deserialized.Should().NotBeNull();
                deserialized.Id.Should().Be(expectedId1);
                deserialized.FieldDataKind.Should().Be(FieldDataKind.NumberWithDecimals);
                deserialized.NumberOfDecimalPlaces.Should().Be(expectedNumberOfDecimalPlaces1);
                deserialized.Title.Should().Be(expectedTitle1);
            }

            void ThrowIfObjectsDiffer2(DescribedSerialization serialized, YearField deserialized)
            {
                deserialized.Should().NotBeNull();
                deserialized.Id.Should().Be(expectedId2);
                deserialized.FieldDataKind.Should().Be(FieldDataKind.Year);
                deserialized.NumberOfDecimalPlaces.Should().Be(expectedNumberOfDecimalPlaces2);
                deserialized.Title.Should().Be(expectedTitle2);
            }

            // Act & Assert
            expected1.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer1, jsonConfigType, bsonConfigType);
            expected2.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer2, jsonConfigType, bsonConfigType);
        }
    }
}
