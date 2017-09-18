// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBsonConfigurationManager.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;

    using FluentAssertions;

    using Naos.Serialization.Bson;

    using Xunit;

    public static class TestBsonConfigurationManager
    {
        [Fact]
        public static void ConfigureWorksWithType()
        {
            BsonConfigurationManager.Configure(typeof(TestConfigure));
            BsonConfigurationManager.Configure(typeof(TestConfigure));
            BsonConfigurationManager.Configure<TestConfigure>();

            TestConfigure.Configured.Should().BeTrue();
        }

        [Fact]
        public static void ConfigureWorksWithGeneric()
        {
            BsonConfigurationManager.Configure<TestConfigure>();
            BsonConfigurationManager.Configure<TestConfigure>();
            BsonConfigurationManager.Configure(typeof(TestConfigure));

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
}
