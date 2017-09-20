// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestJsonSerializer.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using MongoDB.Bson;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Json;

    using Xunit;

    public static class TestJsonSerializer
    {
        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestMapping___Works()
        {
            // Arrange
            var serializer = new NaosJsonSerializer();

            var expected = new TestMapping
                               {
                                   StringProperty = Guid.NewGuid().ToString(),
                                   IntProperty = 5,
                                   DateTimePropertyUtc = DateTime.UtcNow,
                                   DateTimePropertyLocal = DateTime.UtcNow.ToLocalTime(),
                                   DateTimePropertyUnspecified = DateTime.UtcNow.ToUnspecified(),
                                   GuidProperty = Guid.NewGuid(),
                                   NonEnumArray = new[] { "monkey" },
                                   EnumArray = new[] { TestEnumeration.None, },
                                   StringIntMap = new Dictionary<string, int> { { "key", 0 } },
                                   EnumIntMap = new Dictionary<AnotherEnumeration, int> { { AnotherEnumeration.None, 0 } },
                                   IntIntTuple = new Tuple<int, int>(3, 4),
                                   EnumProperty = TestEnumeration.None,
                               };

            // Act
            var actualBytes = serializer.SerializeToBytes(expected);
            var actualString = serializer.SerializeToString(expected);
            var actualObjectFromBytes = serializer.Deserialize<TestMapping>(actualBytes);
            var actualObjectFromString = serializer.Deserialize<TestMapping>(actualString);

            // Assert
            actualObjectFromBytes.Should().NotBeNull();
            actualObjectFromBytes.StringProperty.Should().Be(expected.StringProperty);
            actualObjectFromBytes.IntProperty.Should().Be(expected.IntProperty);
            actualObjectFromBytes.DateTimePropertyUtc.Kind.Should().Be(DateTimeKind.Utc);
            actualObjectFromBytes.DateTimePropertyUtc.Should().Be(expected.DateTimePropertyUtc);
            actualObjectFromBytes.DateTimePropertyLocal.Should().Be(expected.DateTimePropertyLocal);
            actualObjectFromBytes.DateTimePropertyLocal.Kind.Should().Be(DateTimeKind.Local);
            actualObjectFromBytes.DateTimePropertyUnspecified.Should().Be(expected.DateTimePropertyUnspecified);
            actualObjectFromBytes.DateTimePropertyUnspecified.Kind.Should().Be(DateTimeKind.Unspecified);
            actualObjectFromBytes.GuidProperty.Should().Be(expected.GuidProperty);
            actualObjectFromBytes.NonEnumArray.Single().Should().Be(expected.NonEnumArray.Single());
            actualObjectFromBytes.EnumArray.Single().Should().Be(expected.EnumArray.Single());
            actualObjectFromBytes.StringIntMap.Single().Should().Be(expected.StringIntMap.Single());
            actualObjectFromBytes.EnumIntMap.Single().Should().Be(expected.EnumIntMap.Single());
            actualObjectFromBytes.IntIntTuple.Should().Be(expected.IntIntTuple);
            actualObjectFromBytes.EnumProperty.Should().Be(expected.EnumProperty);

            actualObjectFromString.Should().NotBeNull();
            actualObjectFromString.StringProperty.Should().Be(expected.StringProperty);
            actualObjectFromString.IntProperty.Should().Be(expected.IntProperty);
            actualObjectFromString.DateTimePropertyUtc.Kind.Should().Be(DateTimeKind.Utc);
            actualObjectFromString.DateTimePropertyUtc.Should().Be(expected.DateTimePropertyUtc);
            actualObjectFromString.DateTimePropertyLocal.Should().Be(expected.DateTimePropertyLocal);
            actualObjectFromString.DateTimePropertyLocal.Kind.Should().Be(DateTimeKind.Local);
            actualObjectFromString.DateTimePropertyUnspecified.Should().Be(expected.DateTimePropertyUnspecified);
            actualObjectFromString.DateTimePropertyUnspecified.Kind.Should().Be(DateTimeKind.Unspecified);
            actualObjectFromString.GuidProperty.Should().Be(expected.GuidProperty);
            actualObjectFromString.NonEnumArray.Single().Should().Be(expected.NonEnumArray.Single());
            actualObjectFromString.EnumArray.Single().Should().Be(expected.EnumArray.Single());
            actualObjectFromString.StringIntMap.Single().Should().Be(expected.StringIntMap.Single());
            actualObjectFromString.EnumIntMap.Single().Should().Be(expected.EnumIntMap.Single());
            actualObjectFromString.IntIntTuple.Should().Be(expected.IntIntTuple);
            actualObjectFromString.EnumProperty.Should().Be(expected.EnumProperty);
        }
    }
}
