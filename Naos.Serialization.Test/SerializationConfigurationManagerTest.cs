// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationConfigurationManagerTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;

    using FluentAssertions;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Xunit;

    public static class SerializationConfigurationManagerTest
    {
        [Fact]
        public static void Configure___Type_not_BsonConfigurationBase___Throws()
        {
            // Arrange
            Action action = () => SerializationConfigurationManager.Configure(typeof(string));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Parameter 'typeMustBeSubclassOfSerializationConfigurationBase' is not true.  Parameter value is 'False'.");
        }

        [Fact]
        public static void Configure___Type_does_not_have_default_constructor___Throws()
        {
            // Arrange
            Action action = () => SerializationConfigurationManager.Configure(typeof(TestConfigureParameterConstructor));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Parameter 'typeHasParameterLessConstructor' is not true.  Parameter value is 'False'.");
        }

        [Fact]
        public static void Configure___Valid_type_as_generic___Works()
        {
            SerializationConfigurationManager.Configure<TestConfigure>();
            TestConfigure.Configured.Should().BeTrue();
        }
    }

    public class TestConfigure : BsonConfigurationBase
    {
        /// <summary>
        /// Gets a value indicating whether or not it has been configured.
        /// </summary>
        public static bool Configured { get; private set; }

        /// <inheritdoc cref="BsonConfigurationBase" />
        protected override void FinalConfiguration()
        {
            if (Configured)
            {
                throw new NotSupportedException("Configuration is not reentrant and should not have been called a second time.");
            }

            Configured = true;
        }
    }

    public class TestConfigureParameterConstructor : BsonConfigurationBase
    {
        public TestConfigureParameterConstructor(string thingy)
        {
            this.Thingy = thingy;
        }

        public string Thingy { get; set; }

        /// <inheritdoc cref="BsonConfigurationBase" />
        protected override void FinalConfiguration()
        {
            /* no-op */
        }
    }
}
