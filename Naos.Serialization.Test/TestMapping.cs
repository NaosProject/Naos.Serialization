// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestMapping.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

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
            return $"Dunno.  You owe me ${this.Fee}";
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
        public AnotherEnumeration GetMyEnumFromThis => AnotherEnumeration.AnotherFirst;

        public string GetMyStringFromThis => "TurtleBusiness";

        public override AnotherEnumeration GetMyEnumFromBase => AnotherEnumeration.AnotherSecond;

        public override string GetMyStringFromBase => "MonkeyBusiness";
    }
}