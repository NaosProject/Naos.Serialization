// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosJsonSerializerTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Linq;
    using FakeItEasy;

    using FluentAssertions;

    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using Newtonsoft.Json.Linq;
    using OBeautifulCode.Reflection;

    using Xunit;

    using static System.FormattableString;

    public static class NaosJsonSerializerTest
    {
        [Fact]
        public static void NaosJsonSerializer___With_type_Default___Uses_default()
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

            var test = new TestObject { Property1 = property1, Property2 = property2, Property3 = property3, };
            var serializer = new NaosJsonSerializer();

            // Act
            var actual = serializer.SerializeToString(test);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public static void SerializeObject_without_type_serializes_to_json_using_DefaultSerializerSettings()
        {
            // If Default is being used then there should be new lines
            var dog = new Dog(5, "spud", FurColor.Brindle);

            var json = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>)).SerializeToString(dog);

            json.Should().Be("{\r\n  \"name\": \"spud\",\r\n  \"furColor\": \"brindle\",\r\n  \"dogTag\": \"my name is spud\",\r\n  \"nickname\": null,\r\n  \"age\": 5,\r\n  \"$concreteType\": \"Naos.Serialization.Test.Dog, Naos.Serialization.Test\"\r\n}");
        }

        [Fact]
        public static void DeserializeObjectOfT_deserialize_json_using_DefaultSerializerSettings()
        {
            // If Default is being used then strict constructor matching will result in a Dog and not a Mouse
            var dogJson = "{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10}";

            var dog = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>)).Deserialize<Animal>(dogJson) as Dog;

            dog.Should().NotBeNull();
            dog.Name.Should().Be("Barney");
            dog.Age.Should().Be(10);
            dog.FurColor.Should().Be(FurColor.Brindle);
            dog.DogTag.Should().Be("my name is Barney");
        }

        [Fact]
        public static void DeserializeObject_with_type_deserialize_json_using_DefaultSerializerSettings()
        {
            // If Default is being used then strict constructor matching will result in a Dog and not a Mouse
            var dogJson = "{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10}";

            var dog = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>)).Deserialize(dogJson, typeof(Animal)) as Dog;

            dog.Should().NotBeNull();
            dog.Name.Should().Be("Barney");
            dog.Age.Should().Be(10);
            dog.FurColor.Should().Be(FurColor.Brindle);
            dog.DogTag.Should().Be("my name is Barney");
        }

        [Fact]
        public static void DeserializeObject_without_type_deserialize_json_into_JObject_using_DefaultSerializerSettings()
        {
            var dogJson = "{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10}";

            var dog = new NaosJsonSerializer().Deserialize<dynamic>(dogJson) as JObject;

            dog.Properties().Count().Should().Be(3);
            dog["name"].ToString().Should().Be("Barney");
            dog["age"].ToString().Should().Be("10");
            dog["furColor"].ToString().Should().Be("brindle");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CompactUses", Justification = "Spelling/name is correct.")]
        [Fact]
        public static void NaosJsonSerializer___With_type_Compact___Uses_compact()
        {
            // Arrange
            var property1 = A.Dummy<string>();
            var property2 = A.Dummy<string>();
            var property3 = A.Dummy<string>();

            var expected = "{"
                           + Invariant($"\"property1\":\"{property1}\",")
                           + Invariant($"\"property2\":\"{property2}\",")
                           + Invariant($"\"property3\":\"{property3}\",")
                           + "\"property4\":null"
                           + "}";

            var test = new TestObject { Property1 = property1, Property2 = property2, Property3 = property3, };
            var serializer = new NaosJsonSerializer(formattingKind: JsonFormattingKind.Compact);

            // Act
            var actual = serializer.SerializeToString(test);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public static void SerializeObject_without_type_serializes_to_json_using_CompactSerializerSettings()
        {
            // If Compact is being used then there should be no new lines
            var dog = new Dog(5, "spud", FurColor.Brindle);

            var json = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>), JsonFormattingKind.Compact).SerializeToString(dog);

            var expected = "{\"name\":\"spud\",\"furColor\":\"brindle\",\"dogTag\":\"my name is spud\",\"nickname\":null,\"age\":5,\"$concreteType\":\"Naos.Serialization.Test.Dog, Naos.Serialization.Test\"}";
            json.Should().Be(expected);
        }

        [Fact]
        public static void DeserializeObjectOfT_deserialize_json_using_CompactSerializerSettings()
        {
            // If Compact is being used then strict constructor matching will result in a Dog and not a Mouse
            var dogJson = "{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10}";

            var dog = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>), JsonFormattingKind.Compact).Deserialize<Animal>(dogJson) as Dog;

            dog.Should().NotBeNull();
            dog.Name.Should().Be("Barney");
            dog.Age.Should().Be(10);
            dog.FurColor.Should().Be(FurColor.Brindle);
            dog.DogTag.Should().Be("my name is Barney");
        }

        [Fact]
        public static void DeserializeObject_with_type_deserialize_json_using_CompactSerializerSettings()
        {
            // If Compact is being used then strict constructor matching will result in a Dog and not a Mouse
            var dogJson = "{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10}";

            var dog = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>), JsonFormattingKind.Compact).Deserialize(dogJson, typeof(Animal)) as Dog;

            dog.Should().NotBeNull();
            dog.Name.Should().Be("Barney");
            dog.Age.Should().Be(10);
            dog.FurColor.Should().Be(FurColor.Brindle);
            dog.DogTag.Should().Be("my name is Barney");
        }

        [Fact]
        public static void DeserializeObject_without_type_deserialize_json_into_JObject_using_CompactSerializerSettings()
        {
            var dogJson = "{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10}";

            var dog = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>), JsonFormattingKind.Compact).Deserialize<dynamic>(dogJson) as JObject;

            dog.Properties().Count().Should().Be(3);
            dog["name"].ToString().Should().Be("Barney");
            dog["age"].ToString().Should().Be("10");
            dog["furColor"].ToString().Should().Be("brindle");
        }

        [Fact]
        public static void NaosJsonSerializer___With_type_Minimal___Uses_minimal()
        {
            // Arrange
            var property1 = A.Dummy<string>();
            var property2 = A.Dummy<string>();
            var property3 = A.Dummy<string>();

            var expected = "{"
                           + Invariant($"\"property1\":\"{property1}\",")
                           + Invariant($"\"property2\":\"{property2}\",")
                           + Invariant($"\"property3\":\"{property3}\"")
                           + "}";

            var test = new TestObject { Property1 = property1, Property2 = property2, Property3 = property3, };
            var serializer = new NaosJsonSerializer(formattingKind: JsonFormattingKind.Minimal);

            // Act
            var actual = serializer.SerializeToString(test);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public static void SerializeObject_without_type_serializes_to_json_using_MinimalSerializerSettings()
        {
            // If Minimal is being used then the null Nickname property won't be serialized
            var dog = new Dog(5, "spud", FurColor.Brindle);

            var json = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>), JsonFormattingKind.Minimal).SerializeToString(dog);

            json.Should().Be("{\"name\":\"spud\",\"furColor\":\"brindle\",\"dogTag\":\"my name is spud\",\"age\":5}");
        }

        [Fact]
        public static void DeserializeObjectOfT_deserialize_json_using_MinimalSerializerSettings()
        {
            // If Minimal is being used then empty JSON string will deserialize into NoLighting
            // otherwise, out-of-the-box json.net will create an anonymous object
            var lightingJson = "{}";

            var lighting = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Lighting>), JsonFormattingKind.Minimal).Deserialize<Lighting>(lightingJson) as NoLighting;

            lighting.Should().NotBeNull();
        }

        [Fact]
        public static void DeserializeObject_with_type_deserialize_json_using_MinimalSerializerSettings()
        {
            // If Minimal is being used then empty JSON string will deserialize into NoLighting
            // otherwise, out-of-the-box json.net will create an anonymous object
            var lightingJson = "{}";

            var lighting = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Lighting>), JsonFormattingKind.Minimal).Deserialize(lightingJson, typeof(Lighting)) as NoLighting;

            lighting.Should().NotBeNull();
        }

        [Fact]
        public static void DeserializeObject_without_type_deserialize_json_into_JObject_using_MinimalSerializerSettings()
        {
            var dogJson = "{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10}";

            var dog = new NaosJsonSerializer(formattingKind: JsonFormattingKind.Minimal).Deserialize<dynamic>(dogJson) as JObject;

            dog.Properties().Count().Should().Be(3);
            dog["name"].ToString().Should().Be("Barney");
            dog["age"].ToString().Should().Be("10");
            dog["furColor"].ToString().Should().Be("brindle");
        }
    }

    public class TestObject
    {
        public string Property1 { get; set; }

        public string Property2 { get; set; }

        public string Property3 { get; set; }

        public string Property4 { get; set; }
    }
}
