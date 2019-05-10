// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosBsonSerializerTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;

    using Xunit;

    using static System.FormattableString;

    public static class NaosBsonSerializerTest
    {
        [Fact]
        public static void NaosBsonSerializer___Invalid_configuration_type___Throws()
        {
            // Arrange
            Action action = () => new NaosBsonSerializer(typeof(string));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Parameter 'typeMustBeSubclassOfNaos.Serialization.Domain.SerializationConfigurationBase' is not true.  Parameter value is 'False'.");
        }

        [Fact]
        public static void Constructor___Type_without_default_constructor___Throws()
        {
            // Arrange
            Action action = () =>
            {
                new NaosBsonSerializer(typeof(CustomNoPublicConstructor));
            };

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Parameter 'typeHasParameterLessConstructor' is not true.  Parameter value is 'False'.");
        }
    }
}
