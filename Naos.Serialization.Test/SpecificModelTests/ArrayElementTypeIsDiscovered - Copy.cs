// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayElementTypeIsDiscovered.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using Xunit;

    public static class NewingUpSerializersWithDependencies
    {
        [Fact]
        public static void RootFirstWorks()
        {
            var rootSerializer = new NaosJsonSerializer(typeof(ConfigRoot));
            var root = rootSerializer.SerializeToString(null);
            var middleSerializer = new NaosJsonSerializer(typeof(ConfigMiddle));
            var middle = middleSerializer.SerializeToString(null);
        }
    }

    public class ConfigBehindOne : JsonConfigurationBase
    {
    }

    public class ConfigBehindTwo : JsonConfigurationBase
    {
    }

    public class ConfigRoot : JsonConfigurationBase
    {
        public override IReadOnlyCollection<Type> DependentConfigurationTypes => new[] { typeof(ConfigBehindOne), typeof(ConfigBehindTwo), };
    }

    public class ConfigMiddle : JsonConfigurationBase
    {
        public override IReadOnlyCollection<Type> DependentConfigurationTypes => new[] { typeof(ConfigRoot) };
    }
}
