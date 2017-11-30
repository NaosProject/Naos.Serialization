﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigurationBaseTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must not be equal to Invalid.\r\nParameter name: InheritSettingsFromKind");
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
            actual.SerializationSettings.ContractResolver.GetType().FullName.Should().Be("Spritely.Recipes.CamelStrictConstructorContractResolver");
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