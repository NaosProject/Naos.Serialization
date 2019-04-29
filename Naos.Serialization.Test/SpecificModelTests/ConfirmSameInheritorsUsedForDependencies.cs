// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfirmSameInheritorsUsedForDependencies.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using Xunit;

    public static class ConfirmSameInheritorsUsedForDependencies
    {
        [Fact]
        public static void SerializationConfigurationManagerDoesNotAllow()
        {
            // Arrange
            var config = typeof(SameInheritorJsonConfig);
            Action action = () => SerializationConfigurationManager
                .Configure(config);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<InvalidOperationException>();
            exception.Message.Should().Be("Configuration Naos.Serialization.Test.SameInheritorJsonConfig has DependentConfigurationTypes (Naos.Serialization.Test.SameInheritorBsonConfigA) that do not share the same first layer of inheritance Naos.Serialization.Json.JsonConfigurationBase.");
        }
    }

    public class SameInheritorJsonConfig : JsonConfigurationBase
    {
        public override IReadOnlyCollection<Type> DependentConfigurationTypes =>
            new[] { typeof(SameInheritorJsonConfigA), typeof(SameInheritorBsonConfigA) };
    }

    public class SameInheritorBsonConfigA : BsonConfigurationBase
    {
    }

    public class SameInheritorJsonConfigA : JsonConfigurationBase
    {
    }
}