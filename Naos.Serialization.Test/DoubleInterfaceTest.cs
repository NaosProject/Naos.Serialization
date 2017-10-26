// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleInterfaceTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Naos.Serialization.Bson;

    using Xunit;

    public static class DoubleInterfaceTest
    {
        [Fact]
        public static void NaosJsonSerializer___With_kind_Invalid___Throws()
        {
            // Arrange
            BsonConfigurationManager.Configure<DoubleInterfaceBsonConfiguration>();
            var expected = new Dog
            {
                Name = "fido",
                Species = "k9",
            };

            var serializedBytes = NaosBsonSerializerHelper.SerializeToBytes(expected);

            // Act
            var actual = NaosBsonSerializerHelper.Deserialize<Dog>(serializedBytes);

            // Assert
            actual.Name.Should().Be("fido");
            actual.Species.Should().Be("k9");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "required for test")]
        private class DoubleInterfaceBsonConfiguration : BsonConfigurationBase
        {
            protected override IReadOnlyCollection<Type> TypesToAutoRegister => new[] { typeof(IAmAnimal) };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "required for test")]
        public interface IAmAnimal
        {
            string Species { get; set; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "required for test")]
        public interface IAmDog : IAmAnimal
        {
            string Name { get; set; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "required for test")]
        public class Dog : IAmDog
        {
            public string Species { get; set; }

            public string Name { get; set; }
        }
    }
}
