// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestMapping.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
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

    public class TestWithInheritorExtraPropertyWrapper
    {
        public TestWithInheritor InheritorPropertyBase { get; set; }

        public TestWithInheritor InheritorPropertyExtended { get; set; }
    }

    public class TestWrappedFields
    {
        public DateTime? NullableDateTimeNull { get; set; }

        public DateTime? NullableDateTimeWithValue { get; set; }

        public IEnumerable<DateTime> EnumerableOfDateTime { get; set; }

        public AnotherEnumeration? NullableEnumNull { get; set; }

        public AnotherEnumeration? NullableEnumWithValue { get; set; }

        public IEnumerable<AnotherEnumeration> EnumerableOfEnum { get; set; }
    }

    public class TestMappingThatFails
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Just need a type to test.")]
        public Dictionary<int, int> StringIntMap { get; set; }
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
}