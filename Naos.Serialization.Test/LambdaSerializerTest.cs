// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LambdaSerializerTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using FakeItEasy;

    using FluentAssertions;

    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using Xunit;

    using static System.FormattableString;

    public static class LambdaSerializerTest
    {
        [Fact]
        public static void LambdaSerializer__Passes_through()
        {
            // Arrange
            var property1 = A.Dummy<string>();
            var property2 = A.Dummy<string>();
            var property3 = A.Dummy<string>();

            var expected = "{"
                           + Environment.NewLine
                           + Invariant($"  \"property1\": \"{property1}\",") + Environment.NewLine
                           + Invariant($"  \"property2\": \"{property2}\",") + Environment.NewLine
                           + Invariant($"  \"property3\": \"{property3}\",") + Environment.NewLine
                           + "  \"property4\": null" + Environment.NewLine
                           + "}";

            var test = new TestObjectForLambda { Property1 = property1, Property2 = property2, Property3 = property3, };
            var backingSerializer = new NaosJsonSerializer();
            var serializer = new LambdaSerializer(
                backingSerializer.SerializeToString,
                backingSerializer.Deserialize,
                backingSerializer.SerializeToBytes,
                backingSerializer.Deserialize);

            // Act
            var stringOut = serializer.SerializeToString(test);
            var stringRoundTrip = serializer.Deserialize(stringOut, test.GetType());
            var bytesOut = serializer.SerializeToBytes(stringRoundTrip);
            var bytesRoundTrip = serializer.Deserialize(bytesOut, stringRoundTrip.GetType());
            var actual = backingSerializer.SerializeToString(bytesRoundTrip);

            // Assert
            actual.Should().Be(expected);
            stringOut.Should().Be(expected);
        }
    }

    public class TestObjectForLambda
    {
        public string Property1 { get; set; }

        public string Property2 { get; set; }

        public string Property3 { get; set; }

        public string Property4 { get; set; }
    }
}
