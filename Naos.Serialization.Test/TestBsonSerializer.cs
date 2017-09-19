// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBsonSerializer.cs" company="Naos">
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
    using MongoDB.Bson.IO;

    using Naos.Serialization.Bson;

    using Newtonsoft.Json.Linq;

    using Xunit;

    public static class TestBsonSerializer
    {
        [Fact]
        public static void RoundtripSerializeDeserialize___Using_TestMapping_and_BsonConfigurationTestAutoAllGeneric___Works()
        {
            // Arrange
            var configuration = new BsonConfigurationAutoRegisterType<TestMapping>();
            configuration.Configure();

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
            var actualBytes = NaosBsonSerializerHelper.SerializeToBytes(expected);
            var document = NaosBsonSerializerHelper.SerializeToDocument(expected);
            var json = document.ToJson();
            var actualObject = NaosBsonSerializerHelper.Deserialize<TestMapping>(actualBytes);

            // Assert
            document.Should().NotBeNull();
            json.Should().NotBeNullOrEmpty();
            actualObject.StringProperty.Should().Be(expected.StringProperty);
            actualObject.IntProperty.Should().Be(expected.IntProperty);
            actualObject.DateTimePropertyUtc.Kind.Should().Be(DateTimeKind.Utc);
            actualObject.DateTimePropertyUtc.Should().Be(expected.DateTimePropertyUtc);
            actualObject.DateTimePropertyLocal.Should().Be(expected.DateTimePropertyLocal);
            actualObject.DateTimePropertyLocal.Kind.Should().Be(DateTimeKind.Local);
            actualObject.DateTimePropertyUnspecified.Should().Be(expected.DateTimePropertyUnspecified);
            actualObject.DateTimePropertyUnspecified.Kind.Should().Be(DateTimeKind.Unspecified);
            actualObject.GuidProperty.Should().Be(expected.GuidProperty);
            actualObject.NonEnumArray.Single().Should().Be(expected.NonEnumArray.Single());
            actualObject.EnumArray.Single().Should().Be(expected.EnumArray.Single());
            actualObject.StringIntMap.Single().Should().Be(expected.StringIntMap.Single());
            actualObject.EnumIntMap.Single().Should().Be(expected.EnumIntMap.Single());
            actualObject.IntIntTuple.Should().Be(expected.IntIntTuple);
            actualObject.EnumProperty.Should().Be(expected.EnumProperty);
        }
    }
}
