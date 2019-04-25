// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryFlavorSupportTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FakeItEasy;
    using FluentAssertions;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using OBeautifulCode.Math.Recipes;
    using Xunit;

    public static class DictionaryFlavorSupportTest
    {
        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestDictionaryMapping___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericDiscoveryBsonConfiguration<TestDictionaryFields>);
            var jsonConfigType = typeof(GenericDiscoveryJsonConfiguration<TestDictionaryFields>);

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
        public static void RoundtripSerializeDeserialize___Using_TestDictionaryMixedKeyValues___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericDiscoveryBsonConfiguration<TestDictionaryMixedKeyValues>);
            var jsonConfigType = typeof(GenericDiscoveryJsonConfiguration<TestDictionaryMixedKeyValues>);

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
        public static void RoundtripSerializeDeserialize___Using_TestWithDictionaryKeyedOnEnum___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericDiscoveryBsonConfiguration<TestWithDictionaryKeyedOnEnum>);
            var jsonConfigType = typeof(GenericDiscoveryJsonConfiguration<TestWithDictionaryKeyedOnEnum>);

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
        public static void RoundtripSerializeDeserialize___Using_TestWithDictionaryOfEnumToReadOnlyCollectionOfEnums___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericDiscoveryBsonConfiguration<TestWithDictionaryOfEnumToReadOnlyCollectionOfEnums>);
            var jsonConfigType = typeof(GenericDiscoveryJsonConfiguration<TestWithDictionaryOfEnumToReadOnlyCollectionOfEnums>);

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

        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestDictionaryMapping_with_all_nulls___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericDiscoveryBsonConfiguration<TestDictionaryFields>);
            var jsonConfigType = typeof(GenericDiscoveryJsonConfiguration<TestDictionaryFields>);

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
        public static void RoundtripSerializeDeserialize___Using_TestDictionaryMixedKeyValues_with_all_nulls___Works()
        {
            // Arrange
            var bsonConfigType = typeof(GenericDiscoveryBsonConfiguration<TestDictionaryMixedKeyValues>);
            var jsonConfigType = typeof(GenericDiscoveryJsonConfiguration<TestDictionaryMixedKeyValues>);

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
    }

    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Just need a type to test.")]
    public class TestWithDictionaryKeyedOnEnum
    {
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<TestEnumeration, string> TestDictionary { get; set; }
    }

    public class TestWithDictionaryOfEnumToReadOnlyCollectionOfEnums
    {
        public IReadOnlyDictionary<TestEnumeration, IReadOnlyCollection<AnotherEnumeration>> TestDictionary { get; set; }
    }

    public class TestWithDictionaryOfEnumToReadOnlyCollectionOfEnumsConfig : BsonConfigurationBase
    {
        protected override IReadOnlyCollection<Type> ClassTypesToRegisterAlongWithInheritors => new[] { typeof(TestWithDictionaryOfEnumToReadOnlyCollectionOfEnums) };
    }

    public class TestDictionaryFields
    {
        //add datetime key, guid
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<string, string> DictionaryStringString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<string, string> IDictionaryStringString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<string, string> ReadOnlyDictionaryStringString { get; set; }

        public IReadOnlyDictionary<string, string> IReadOnlyDictionaryStringString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ConcurrentDictionary<string, string> ConcurrentDictionaryStringString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<string, int> ReadOnlyDictionaryStringInt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<int, string> ReadOnlyDictionaryIntString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<AnotherEnumeration, string> IDictionaryEnumString { get; set; }
    }

    public class TestDictionaryMixedKeyValues
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<KeyOrValueObjectHierarchyBase, string> DictionaryBaseString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<KeyOrValueObjectHierarchyBase, string> IDictionaryBaseString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<KeyOrValueObjectHierarchyBase, string> ReadOnlyDictionaryBaseString { get; set; }

        public IReadOnlyDictionary<KeyOrValueObjectHierarchyBase, string> IReadOnlyDictionaryBaseString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ConcurrentDictionary<KeyOrValueObjectHierarchyBase, string> ConcurrentDictionaryBaseString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<string, KeyOrValueObjectWithPrivateSetter> DictionaryStringConstructor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<string, KeyOrValueObjectWithPrivateSetter> IDictionaryStringConstructor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<string, KeyOrValueObjectWithPrivateSetter> ReadOnlyDictionaryStringConstructor { get; set; }

        public IReadOnlyDictionary<string, KeyOrValueObjectWithPrivateSetter> IReadOnlyDictionaryStringConstructor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ConcurrentDictionary<string, KeyOrValueObjectWithPrivateSetter> ConcurrentDictionaryStringConstructor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<KeyOrValueObjectWithPrivateSetter, AnotherEnumeration> DictionaryConstructorEnum { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<KeyOrValueObjectWithPrivateSetter, AnotherEnumeration> IDictionaryConstructorEnum { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<KeyOrValueObjectWithPrivateSetter, AnotherEnumeration> ReadOnlyDictionaryConstructorEnum { get; set; }

        public IReadOnlyDictionary<KeyOrValueObjectWithPrivateSetter, AnotherEnumeration> IReadOnlyDictionaryConstructorEnum { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ConcurrentDictionary<KeyOrValueObjectWithPrivateSetter, AnotherEnumeration> ConcurrentDictionaryConstructorEnum { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<AnotherEnumeration, KeyOrValueObjectHierarchyBase> DictionaryEnumBase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<AnotherEnumeration, KeyOrValueObjectHierarchyBase> IDictionaryEnumBase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<AnotherEnumeration, KeyOrValueObjectHierarchyBase> ReadOnlyDictionaryEnumBase { get; set; }

        public IReadOnlyDictionary<AnotherEnumeration, KeyOrValueObjectHierarchyBase> IReadOnlyDictionaryEnumBase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ConcurrentDictionary<AnotherEnumeration, KeyOrValueObjectHierarchyBase> ConcurrentDictionaryEnumBase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Poco", Justification = "Name/spelling is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<int, VanillaClass> DictionaryIntPoco { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Poco", Justification = "Name/spelling is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<int, VanillaClass> IDictionaryIntPoco { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Poco", Justification = "Name/spelling is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<int, VanillaClass> ReadOnlyDictionaryIntPoco { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Poco", Justification = "Name/spelling is correct.")]
        public IReadOnlyDictionary<int, VanillaClass> IReadOnlyDictionaryIntPoco { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Poco", Justification = "Name/spelling is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ConcurrentDictionary<int, VanillaClass> ConcurrentDictionaryIntPoco { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Poco", Justification = "Name/spelling is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<VanillaClass, int> DictionaryPocoInt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Poco", Justification = "Name/spelling is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<VanillaClass, int> IDictionaryPocoInt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Poco", Justification = "Name/spelling is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<VanillaClass, int> ReadOnlyDictionaryPocoInt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Poco", Justification = "Name/spelling is correct.")]
        public IReadOnlyDictionary<VanillaClass, int> IReadOnlyDictionaryPocoInt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Poco", Justification = "Name/spelling is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ConcurrentDictionary<VanillaClass, int> ConcurrentDictionaryPocoInt { get; set; }
    }

    public class KeyOrValueObjectWithPrivateSetter
    {
        public KeyOrValueObjectWithPrivateSetter(string privateValue)
        {
            this.PrivateValue = privateValue;
        }

        public string PrivateValue { get; private set; }

        public static bool operator ==(
            KeyOrValueObjectWithPrivateSetter item1,
            KeyOrValueObjectWithPrivateSetter item2)
        {
            if (ReferenceEquals(item1, item2))
            {
                return true;
            }

            if (ReferenceEquals(item1, null) || ReferenceEquals(item2, null))
            {
                return false;
            }

            var result = item1.PrivateValue == item2.PrivateValue;

            return result;
        }

        public static bool operator !=(
            KeyOrValueObjectWithPrivateSetter item1,
            KeyOrValueObjectWithPrivateSetter item2)
            => !(item1 == item2);

        public bool Equals(KeyOrValueObjectWithPrivateSetter other) => this == other;

        public override bool Equals(object obj) => this == (obj as KeyOrValueObjectWithPrivateSetter);

        public override int GetHashCode() =>
            HashCodeHelper.Initialize()
                .Hash(this.PrivateValue)
                .Value;
    }

    public abstract class KeyOrValueObjectHierarchyBase
    {
        public string Message { get; set; }

        public static bool operator ==(
            KeyOrValueObjectHierarchyBase item1,
            KeyOrValueObjectHierarchyBase item2)
        {
            if (ReferenceEquals(item1, item2))
            {
                return true;
            }

            if (ReferenceEquals(item1, null) || ReferenceEquals(item2, null))
            {
                return false;
            }

            var result = item1.Equals((object)item2);

            return result;
        }

        public static bool operator !=(
            KeyOrValueObjectHierarchyBase item1,
            KeyOrValueObjectHierarchyBase item2)
            => !(item1 == item2);

        public bool Equals(
            KeyOrValueObjectHierarchyBase other)
            => this == other;

        /// <inheritdoc />
        public abstract override bool Equals(
            object obj);

        /// <inheritdoc />
        public abstract override int GetHashCode();
    }

    public class KeyOrValueObjectHierarchyOne : KeyOrValueObjectHierarchyBase
    {
        public string One { get; set; }

        public static bool operator ==(
            KeyOrValueObjectHierarchyOne item1,
            KeyOrValueObjectHierarchyOne item2)
        {
            if (ReferenceEquals(item1, item2))
            {
                return true;
            }

            if (ReferenceEquals(item1, null) || ReferenceEquals(item2, null))
            {
                return false;
            }

            var result =
                (item1.Message == item2.Message) &&
                (item1.One == item2.One);

            return result;
        }

        public static bool operator !=(
            KeyOrValueObjectHierarchyOne item1,
            KeyOrValueObjectHierarchyOne item2)
            => !(item1 == item2);

        public bool Equals(KeyOrValueObjectHierarchyOne other) => this == other;

        public override bool Equals(object obj) => this == (obj as KeyOrValueObjectHierarchyOne);

        public override int GetHashCode() =>
            HashCodeHelper.Initialize()
                .Hash(this.Message)
                .Hash(this.One)
                .Value;
    }

    public class KeyOrValueObjectHierarchyTwo : KeyOrValueObjectHierarchyBase
    {
        public string Two { get; set; }

        public static bool operator ==(
            KeyOrValueObjectHierarchyTwo item1,
            KeyOrValueObjectHierarchyTwo item2)
        {
            if (ReferenceEquals(item1, item2))
            {
                return true;
            }

            if (ReferenceEquals(item1, null) || ReferenceEquals(item2, null))
            {
                return false;
            }

            var result =
                (item1.Message == item2.Message) &&
                (item1.Two == item2.Two);

            return result;
        }

        public static bool operator !=(
            KeyOrValueObjectHierarchyTwo item1,
            KeyOrValueObjectHierarchyTwo item2)
            => !(item1 == item2);

        public bool Equals(KeyOrValueObjectHierarchyTwo other) => this == other;

        public override bool Equals(object obj) => this == (obj as KeyOrValueObjectHierarchyTwo);

        public override int GetHashCode() =>
            HashCodeHelper.Initialize()
                .Hash(this.Message)
                .Hash(this.Two)
                .Value;
    }
}
