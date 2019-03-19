// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigurationBaseTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;

    using Newtonsoft.Json.Serialization;

    using OBeautifulCode.Reflection;

    using Xunit;

    using static System.FormattableString;

    public static class JsonConfigurationBaseTest
    {
        [Fact]
        public static void JsonConfigurationBase___With_kind_Invalid___Throws()
        {
            // Arrange
            Action action = () => JsonConfigurationManager.Configure<InvalidKindConfiguration>();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Be("Parameter 'InheritSettingsFromKind' is equal to the comparison value using EqualityComparer<T>.Default, where T: SerializationKind.  Specified 'comparisonValue' is 'Invalid'.");
        }

        [Fact]
        public static void JsonConfigurationBase___With_contract_override___Works()
        {
            // Arrange
            var configurationType = typeof(DefaultTestConfiguration);

            // Act
            var actual = JsonConfigurationManager.Configure(configurationType);

            // Assert
            actual.Should().NotBeNull();
            actual.SerializationSettings.ContractResolver.Should().BeOfType<DefaultContractResolver>();
        }

        [Fact]
        public static void JsonConfigurationBase___With_null_implementation___Works()
        {
            // Arrange
            var configurationType = typeof(NullJsonConfiguration);

            // Act
            var actual = JsonConfigurationManager.Configure(configurationType);

            // Assert
            actual.Should().NotBeNull();
            actual.SerializationSettings.ContractResolver.GetType().FullName.Should().Be("Naos.Serialization.Json.CamelStrictConstructorContractResolver"); // this type is not public so we can't use nameof()
        }
    }

    internal class DefaultTestConfiguration : JsonConfigurationBase
    {
        protected override SerializationKind InheritSettingsFromKind => SerializationKind.Default;

        protected override IContractResolver OverrideContractResolver => new DefaultContractResolver();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used above in Configure<T>.")]
    internal class InvalidKindConfiguration : JsonConfigurationBase
    {
        protected override SerializationKind InheritSettingsFromKind => SerializationKind.Invalid;
    }
}
