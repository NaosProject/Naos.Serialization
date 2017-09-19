// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBsonConfigurationBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ExceptionServices;

    using FluentAssertions;

    using MongoDB.Bson.Serialization;

    using Xunit;

    public static class TestBsonConfigurationBase
    {
        [Fact]
        public static void RegisterClassMapsTypeFullyAutomatic___Type_with_no_constraints___Works()
        {
            // Arrange
            var configuration = new BsonConfigurationTestAutoConstrainedType(typeof(TestMapping));

            // Act
            var classMap = RunTestCode(configuration);

            // Assert
            classMap.Should().NotBeNull();
        }

        [Fact]
        public static void RegisterClassMapsTypeFullyAutomatic___Type_with_valid_constraints___Works()
        {
            // Arrange
            var constraints = new[] { nameof(TestMapping.GuidProperty), nameof(TestMapping.StringIntMap) };
            var configuration = new BsonConfigurationTestAutoConstrainedType(typeof(TestMapping), constraints);

            // Act
            var classMap = RunTestCode(configuration);

            // Assert
            classMap.Should().NotBeNull();
        }

        [Fact]
        public static void RegisterClassMapsTypeFullyAutomatic___Type_with_invalid_constraints___Throws()
        {
            // Arrange
            var configuration = new BsonConfigurationTestAutoConstrainedType(typeof(TestMapping), new[] { "monkey" });
            Action action = () => RunTestCode(configuration);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be false.\r\nParameter name: constrainedPropertyDoesNotExistOnType");
        }

        [Fact]
        public static void RegisterClassMapsTypeFullyAutomatic___All_null_type___Throws()
        {
            // Arrange
            var configuration = new BsonConfigurationTestAutoConstrainedType(null);
            Action action = () => RunTestCode(configuration);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: type");
        }

        [Fact]
        public static void RegisterClassMapsTypeFullyAutomatic___Constrained_null_type___Throws()
        {
            // Arrange
            var configuration = new BsonConfigurationTestAutoConstrainedType(null, new[] { "monkeys" });
            Action action = () => RunTestCode(configuration);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: type");
        }

        private static BsonClassMap RunTestCode(object configuration)
        {
            try
            {
                return (BsonClassMap)configuration.GetType().GetMethod("TestCode", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(configuration, null);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            throw new NotSupportedException();
        }
    }
}
