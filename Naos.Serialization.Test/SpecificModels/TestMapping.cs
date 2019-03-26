// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestMapping.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Json;
    using OBeautifulCode.Math.Recipes;

    public class TestMapping
    {
        public string StringProperty { get; set; }

        public TestEnumeration EnumProperty { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Need to test an array.")]
        public TestEnumeration[] EnumArray { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Need to test an array.")]
        public string[] NonEnumArray { get; set; }

        public Guid GuidProperty { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Justification = "Name is correct.")]
        public int IntProperty { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<AnotherEnumeration, int> EnumIntMap { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<string, int> StringIntMap { get; set; }

        public Tuple<int, int> IntIntTuple { get; set; }

        public DateTime DateTimePropertyUtc { get; set; }

        public DateTime DateTimePropertyLocal { get; set; }

        public DateTime DateTimePropertyUnspecified { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Just need a type to test.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public int[] IntArray { get; set; }
    }

    public class TestWithId
    {
        public string Id { get; set; }
    }

    public class TestWithInheritor
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class TestWithInheritorExtraProperty : TestWithInheritor
    {
        public string AnotherName { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Here for testing.")]
    public interface ITestConfigureActionFromInterface
    {
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Here for testing.")]
    public interface ITestConfigureActionFromAuto
    {
    }

    public class TestConfigureActionFromInterface : ITestConfigureActionFromInterface
    {
    }

    public class TestConfigureActionFromAuto : ITestConfigureActionFromAuto
    {
    }

    public abstract class TestConfigureActionBaseFromSub
    {
    }

    public abstract class TestConfigureActionBaseFromAuto
    {
    }

    public class TestConfigureActionInheritedSub : TestConfigureActionBaseFromSub
    {
    }

    public class TestConfigureActionInheritedAuto : TestConfigureActionBaseFromAuto
    {
    }

    public class TestConfigureActionSingle
    {
    }

    public class TestTracking
    {
    }

    public class TestWrappedFields
    {
        public DateTime? NullableDateTimeNull { get; set; }

        public DateTime? NullableDateTimeWithValue { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ICollection<DateTime> CollectionOfDateTime { get; set; }

        public AnotherEnumeration? NullableEnumNull { get; set; }

        public AnotherEnumeration? NullableEnumWithValue { get; set; }

        public IEnumerable<AnotherEnumeration> EnumerableOfEnum { get; set; }
    }

    public class TestDictionaryFields
    {
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
        public Dictionary<TestBase, string> DictionaryBaseString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<TestBase, string> IDictionaryBaseString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<TestBase, string> ReadOnlyDictionaryBaseString { get; set; }

        public IReadOnlyDictionary<TestBase, string> IReadOnlyDictionaryBaseString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ConcurrentDictionary<TestBase, string> ConcurrentDictionaryBaseString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<string, ClassWithPrivateSetter> DictionaryStringConstructor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<string, ClassWithPrivateSetter> IDictionaryStringConstructor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<string, ClassWithPrivateSetter> ReadOnlyDictionaryStringConstructor { get; set; }

        public IReadOnlyDictionary<string, ClassWithPrivateSetter> IReadOnlyDictionaryStringConstructor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ConcurrentDictionary<string, ClassWithPrivateSetter> ConcurrentDictionaryStringConstructor { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<ClassWithPrivateSetter, AnotherEnumeration> DictionaryConstructorEnum { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<ClassWithPrivateSetter, AnotherEnumeration> IDictionaryConstructorEnum { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<ClassWithPrivateSetter, AnotherEnumeration> ReadOnlyDictionaryConstructorEnum { get; set; }

        public IReadOnlyDictionary<ClassWithPrivateSetter, AnotherEnumeration> IReadOnlyDictionaryConstructorEnum { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ConcurrentDictionary<ClassWithPrivateSetter, AnotherEnumeration> ConcurrentDictionaryConstructorEnum { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<AnotherEnumeration, TestBase> DictionaryEnumBase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<AnotherEnumeration, TestBase> IDictionaryEnumBase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyDictionary<AnotherEnumeration, TestBase> ReadOnlyDictionaryEnumBase { get; set; }

        public IReadOnlyDictionary<AnotherEnumeration, TestBase> IReadOnlyDictionaryEnumBase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ConcurrentDictionary<AnotherEnumeration, TestBase> ConcurrentDictionaryEnumBase { get; set; }

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

    public class TestCollectionFields
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ReadOnlyCollection<DateTime> ReadOnlyCollectionDateTime { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public ICollection<string> ICollectionDateTime { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IList<AnotherEnumeration> IListEnum { get; set; }

        public IReadOnlyList<string> IReadOnlyListString { get; set; }

        public IReadOnlyCollection<DateTime> IReadOnlyCollectionGuid { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Just need a type to test.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public List<DateTime> ListDateTime { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Collection<Guid> CollectionGuid { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Just need a type to test.")]
    public class TestWithDictionaryKeyedOnEnum
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public IDictionary<TestEnumeration, string> TestDictionary { get; set; }
    }

    public class TestWithReadOnlyCollectionOfBaseClass
    {
        public IReadOnlyCollection<TestBase> TestCollection { get; set; }

        public TestImplementationOne RootOne { get; set; }

        public TestImplementationTwo RootTwo { get; set; }
    }

    public class TestWithDictionaryOfEnumToReadOnlyCollectionOfEnums
    {
        public IReadOnlyDictionary<TestEnumeration, IReadOnlyCollection<AnotherEnumeration>> TestDictionary { get; set; }
    }

    public class TestWithReadOnlyCollectionOfBaseClassConfig : BsonConfigurationBase
    {
        protected override IReadOnlyCollection<Type> ClassTypesToRegisterAlongWithInheritors => new[] { typeof(TestBase), typeof(TestWithReadOnlyCollectionOfBaseClass) };
    }

    public class TestWithDictionaryOfEnumToReadOnlyCollectionOfEnumsConfig : BsonConfigurationBase
    {
        protected override IReadOnlyCollection<Type> ClassTypesToRegisterAlongWithInheritors => new[] { typeof(TestWithDictionaryOfEnumToReadOnlyCollectionOfEnums) };
    }

    public abstract class TestBase
    {
        public string Message { get; set; }

        public static bool operator ==(
            TestBase item1,
            TestBase item2)
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
            TestBase item1,
            TestBase item2)
            => !(item1 == item2);

        public bool Equals(
            TestBase other)
            => this == other;

        /// <inheritdoc />
        public abstract override bool Equals(
            object obj);

        /// <inheritdoc />
        public abstract override int GetHashCode();
    }

    public class TestImplementationOne : TestBase
    {
        public string One { get; set; }

        public static bool operator ==(
            TestImplementationOne item1,
            TestImplementationOne item2)
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
            TestImplementationOne item1,
            TestImplementationOne item2)
            => !(item1 == item2);

        public bool Equals(TestImplementationOne other) => this == other;

        public override bool Equals(object obj) => this == (obj as TestImplementationOne);

        public override int GetHashCode() =>
            HashCodeHelper.Initialize()
                .Hash(this.Message)
                .Hash(this.One)
                .Value;
    }

    public class TestImplementationTwo : TestBase
    {
        public string Two { get; set; }

        public static bool operator ==(
            TestImplementationTwo item1,
            TestImplementationTwo item2)
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
            TestImplementationTwo item1,
            TestImplementationTwo item2)
            => !(item1 == item2);

        public bool Equals(TestImplementationTwo other) => this == other;

        public override bool Equals(object obj) => this == (obj as TestImplementationTwo);

        public override int GetHashCode() =>
            HashCodeHelper.Initialize()
                .Hash(this.Message)
                .Hash(this.Two)
                .Value;
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

        /// <summary>
        /// First value.
        /// </summary>
        TestFirst,

        /// <summary>
        /// Second value.
        /// </summary>
        TestSecond,

        /// <summary>
        /// Third value.
        /// </summary>
        TestThird,
    }

    public enum AnotherEnumeration
    {
        /// <summary>
        /// No value specified.
        /// </summary>
        None,

        /// <summary>
        /// First value.
        /// </summary>
        AnotherFirst,

        /// <summary>
        /// Second value.
        /// </summary>
        AnotherSecond,
    }

    public class Investigation
    {
        public IReadOnlyCollection<IDeduceWhoLetTheDogsOut> Investigators { get; set; }
    }

    public interface IDeduceWhoLetTheDogsOut
    {
        string WhoLetTheDogsOut();
    }

    public class NamedInvestigator : IDeduceWhoLetTheDogsOut
    {
        public NamedInvestigator(string name, int yearsOfPractice)
        {
            this.Name = name;

            this.YearsOfPractice = yearsOfPractice;
        }

        public string Name { get; private set; }

        public int YearsOfPractice { get; private set; }

        public string WhoLetTheDogsOut()
        {
            return $"I don't know.  I, {this.Name} quit!";
        }
    }

    public class AnonymousInvestigator : IDeduceWhoLetTheDogsOut
    {
        public AnonymousInvestigator(int fee)
        {
            this.Fee = fee;
        }

        public int Fee { get; private set; }

        public string WhoLetTheDogsOut()
        {
            return FormattableString.Invariant($"Dunno.  You owe me ${this.Fee}");
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Onlys", Justification = "Spelling/name is correct.")]
    public abstract class ClassWithGetterOnlysBase
    {
        public AnotherEnumeration GetMyEnumOnlyBase { get; }

        public string GetMyStringOnlyBase { get; }

        public abstract AnotherEnumeration GetMyEnumFromBase { get; }

        public abstract string GetMyStringFromBase { get; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Onlys", Justification = "Spelling/name is correct.")]
    public class ClassWithGetterOnlys : ClassWithGetterOnlysBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is strictly for testing.")]
        public AnotherEnumeration GetMyEnumFromThis => AnotherEnumeration.AnotherFirst;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is strictly for testing.")]
        public string GetMyStringFromThis => "TurtleBusiness";

        public override AnotherEnumeration GetMyEnumFromBase => AnotherEnumeration.AnotherSecond;

        public override string GetMyStringFromBase => "MonkeyBusiness";
    }

    public class ClassWithPrivateSetter
    {
        public ClassWithPrivateSetter(string privateValue)
        {
            this.PrivateValue = privateValue;
        }

        public string PrivateValue { get; private set; }

        public static bool operator ==(
            ClassWithPrivateSetter item1,
            ClassWithPrivateSetter item2)
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
            ClassWithPrivateSetter item1,
            ClassWithPrivateSetter item2)
            => !(item1 == item2);

        public bool Equals(ClassWithPrivateSetter other) => this == other;

        public override bool Equals(object obj) => this == (obj as ClassWithPrivateSetter);

        public override int GetHashCode() =>
            HashCodeHelper.Initialize()
                .Hash(this.PrivateValue)
                .Value;
    }

    public class VanillaClass
    {
        public string Something { get; set; }

        public static bool operator ==(
            VanillaClass item1,
            VanillaClass item2)
        {
            if (ReferenceEquals(item1, item2))
            {
                return true;
            }

            if (ReferenceEquals(item1, null) || ReferenceEquals(item2, null))
            {
                return false;
            }

            var result = item1.Something == item2.Something;

            return result;
        }

        public static bool operator !=(
            VanillaClass item1,
            VanillaClass item2)
            => !(item1 == item2);

        public bool Equals(VanillaClass other) => this == other;

        public override bool Equals(object obj) => this == (obj as VanillaClass);

        public override int GetHashCode() =>
            HashCodeHelper.Initialize()
                .Hash(this.Something)
                .Value;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Spelling/name is correct.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Spelling/name is correct.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Spelling/name is correct.")]
    [Flags]
    public enum FlagsEnumeration
    {
        /// <summary>
        /// None value.
        /// </summary>
        None,

        /// <summary>
        /// Second value.
        /// </summary>
        SecondValue,

        /// <summary>
        /// Third value.
        /// </summary>
        ThirdValue,
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Spelling/name is correct.")]
    public class ClassWithFlagsEnums
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Spelling/name is correct.")]
        public FlagsEnumeration Flags { get; set; }
    }

    public abstract class Field
    {
        protected Field(
            string id)
        {
            this.Id = id;
        }

        public string Id { get; private set; }

        public abstract FieldDataKind FieldDataKind { get; }

        public string Title { get; set; }
    }

    public abstract class DecimalField : Field
    {
        protected DecimalField(string id)
            : base(id)
        {
        }

        public int NumberOfDecimalPlaces { get; set; }
    }

    public class CurrencyField : DecimalField
    {
        public CurrencyField(string id)
            : base(id)
        {
        }

        public override FieldDataKind FieldDataKind
        {
            get
            {
                var result = this.NumberOfDecimalPlaces == 0
                    ? FieldDataKind.CurrencyWithoutDecimals
                    : FieldDataKind.CurrencyWithDecimals;

                return result;
            }
        }
    }

    public class NumberField : DecimalField
    {
        public NumberField(string id)
            : base(id)
        {
        }

        public override FieldDataKind FieldDataKind
        {
            get
            {
                var result = this.NumberOfDecimalPlaces == 0
                    ? FieldDataKind.NumberWithoutDecimals
                    : FieldDataKind.NumberWithDecimals;

                return result;
            }
        }
    }

    public class YearField : NumberField
    {
        public YearField(string id)
            : base(id)
        {
        }

        public override FieldDataKind FieldDataKind => FieldDataKind.Year;
    }

    public enum FieldDataKind
    {
#pragma warning disable SA1602 // Enumeration items should be documented
        CurrencyWithDecimals,

        CurrencyWithoutDecimals,

        NumberWithDecimals,

        NumberWithoutDecimals,

        Text,

        Date,

        Year,
#pragma warning restore SA1602 // Enumeration items should be documented
    }

    internal class FieldConfiguration : JsonConfigurationBase
    {
        protected override IReadOnlyCollection<Type> TypesToAutoRegister => new[]
        {
            typeof(Field),
            typeof(DecimalField),
            typeof(NumberField),
            typeof(YearField),
        };
    }
}