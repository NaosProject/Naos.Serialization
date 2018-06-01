// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonConfigurationManagerTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;

    using FluentAssertions;

    using Naos.Serialization.Bson;

    using Xunit;

    public static class BsonConfigurationManagerTest
    {
        [Fact]
        public static void Configure___Type_not_BsonConfigurationBase___Throws()
        {
            // Arrange
            Action action = () => BsonConfigurationManager.Configure(typeof(string));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Parameter 'typeMustBeSubclassOfBsonConfigurationBase' is not true.  Parameter value is 'False'.");
        }

        [Fact]
        public static void Configure___Type_does_not_have_default_constructor___Throws()
        {
            // Arrange
            Action action = () => BsonConfigurationManager.Configure(typeof(TestConfigureParameterConstructor));

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
            BsonConfigurationManager.Configure<TestConfigure>();
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
        protected override void CustomConfiguration()
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
        protected override void CustomConfiguration()
        {
            /* no-op */
        }
    }
}
