// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetDrawingTypeTests.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System.Drawing;
    using FakeItEasy;
    using FluentAssertions;
    using Naos.Serialization.Bson;
    using Xunit;

    public static class NetDrawingTypeTests
    {
        [Fact]
        public static void RegularColorRoundtrip()
        {
            // Arrange
            var serializer = new NaosBsonSerializer();
            var expected = new ObjectWithNetDrawingTypes
            {
                Color = A.Dummy<Color>(),
                NullableWithValueColor = A.Dummy<Color>(),
                NullableWithoutValueColor = null,
            };

            // Act
            var actualString = serializer.SerializeToString(expected);
            var actual = serializer.Deserialize<ObjectWithNetDrawingTypes>(actualString);

            // Assert
            actual.Color.Should().Be(expected.Color);
            actual.NullableWithValueColor.Should().Be(expected.NullableWithValueColor);
            actual.NullableWithoutValueColor.Should().BeNull();
        }
    }

    public class ObjectWithNetDrawingTypes
    {
        public Color Color { get; set; }

        public Color? NullableWithValueColor { get; set; }

        public Color? NullableWithoutValueColor { get; set; }
    }
}
