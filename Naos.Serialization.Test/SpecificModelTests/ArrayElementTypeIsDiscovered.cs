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
        [Fact]
        public static void ElementTypeOfArrayIsOnlyTypeDiscovered()
        {
            // Arrange
            var config = typeof(NullDiscoverySerializationConfiguration<TypeWithObjectArray>);

            // Act
            var configured = SerializationConfigurationManager.ConfigureWithReturn<NullDiscoverySerializationConfiguration<TypeWithObjectArray>>(config);

            // Assert
            configured.RegisteredTypeToDetailsMap.Keys.Should().HaveCount(2 + SerializationConfigurationBase.InternallyRequiredTypes.Count);
            configured.RegisteredTypeToDetailsMap.Keys.Should().Contain(typeof(TypeWithObjectArray));
            configured.RegisteredTypeToDetailsMap.Keys.Should().Contain(typeof(TypeWithObjectArrayElementType));
            configured.RegisteredTypeToDetailsMap.Keys.Should().NotContain(typeof(TypeWithObjectArrayElementType[]));
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
