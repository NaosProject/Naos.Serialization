// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultJsonSerializerTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System.Linq;
    using FluentAssertions;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using Newtonsoft.Json.Linq;
    using Xunit;

    public static class DefaultJsonSerializerTest
    {
        [Fact]
        public static void SerializeObject_without_type_serializes_to_json_using_DefaultSerializerSettings()
        {
            // If Default is being used then there should be new lines
            var dog = new Dog(5, "spud", FurColor.Brindle);

            var json = new NaosJsonSerializer().SerializeToString(dog);

            json.Should().Be("{\r\n  \"name\": \"spud\",\r\n  \"furColor\": \"brindle\",\r\n  \"dogTag\": \"my name is spud\",\r\n  \"nickname\": null,\r\n  \"age\": 5\r\n}");
        }

        [Fact]
        public static void DeserializeObjectOfT_deserialize_json_using_DefaultSerializerSettings()
        {
            // If Default is being used then strict constructor matching will result in a Dog and not a Mouse
            var dogJson = "{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10}";

            var dog = new NaosJsonSerializer().Deserialize<Animal>(dogJson) as Dog;

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

            var dog = new NaosJsonSerializer().Deserialize(dogJson, typeof(Animal)) as Dog;

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
    }
}
