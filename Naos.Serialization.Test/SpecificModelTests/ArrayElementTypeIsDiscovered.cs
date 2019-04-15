// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayElementTypeIsDiscovered.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Naos.Serialization.Domain;
    using Xunit;

    public static class ArrayElementTypeIsDiscovered
    {
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Onlys", Justification = "Spelling/name is correct.")]
        [Fact]
        public static void ElementTypeOfArrayIsOnlyTypeDiscovered()
        {
            // Arrange
            var config = typeof(AccumulatingTypeConfiguration<TypeWithObjectArray>);

            // Act
            var configured = SerializationConfigurationManager.ConfigureWithReturn<AccumulatingTypeConfiguration<TypeWithObjectArray>>(config);

            // Assert
            configured.AccumulatedTypes.Should().HaveCount(2 + SerializationConfigurationBase.InternallyRequiredTypes.Count);
            configured.AccumulatedTypes.Should().Contain(typeof(TypeWithObjectArray));
            configured.AccumulatedTypes.Should().Contain(typeof(TypeWithObjectArrayElementType));
            configured.AccumulatedTypes.Should().NotContain(typeof(TypeWithObjectArrayElementType[]));
        }
    }

    public class TypeWithObjectArray
    {
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Need an array for testing purposes.")]
        public TypeWithObjectArrayElementType[] Array { get; set; }
    }

    public class TypeWithObjectArrayElementType
    {
        public string Property { get; set; }
    }
}
