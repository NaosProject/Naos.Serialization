// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultilevelDependencyTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;

    using Naos.Serialization.Json;

    using Xunit;

    public static class MultilevelDependencyTest
    {
        [Fact]
        public static void RootFirstWorks()
        {
            var middleSerializer = new NaosJsonSerializer(typeof(ConfigMiddle));
            middleSerializer.SerializeToString(null);

            var topSerializer = new NaosJsonSerializer(typeof(ConfigTop));
            topSerializer.SerializeToString(null);
        }
    }

    public class ConfigTop : JsonConfigurationBase
    {
        public override IReadOnlyCollection<Type> DependentConfigurationTypes => new[] { typeof(ConfigMiddle) };
    }

    public class ConfigMiddle : JsonConfigurationBase
    {
        public override IReadOnlyCollection<Type> DependentConfigurationTypes => new[] { typeof(ConfigBottom1), typeof(ConfigBottom2), };
    }

    public class ConfigBottom1 : JsonConfigurationBase
    {
    }

    public class ConfigBottom2 : JsonConfigurationBase
    {
    }
}
