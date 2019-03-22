// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinimalJsonSerializerTest.cs" company="Naos Project">
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

    public static class MinimalJsonSerializerTest
    {
        [Fact]
        public static void SerializeObject_without_type_serializes_to_json_using_MinimalSerializerSettings()
        {
            // If Minimal is being used then the null Nickname property won't be serialized
            var dog = new Dog(5, "spud", FurColor.Brindle);

            var json = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>), SerializationKind.Minimal).SerializeToString(dog);

            json.Should().Be("{\"name\":\"spud\",\"furColor\":\"brindle\",\"dogTag\":\"my name is spud\",\"age\":5}");
        }

        [Fact]
        public static void DeserializeObjectOfT_deserialize_json_using_MinimalSerializerSettings()
        {
            // If Minimal is being used then empty JSON string will deserialize into NoLighting
            // otherwise, out-of-the-box json.net will create an anonymous object
            var lightingJson = "{}";

            var lighting = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Lighting>), SerializationKind.Minimal).Deserialize<Lighting>(lightingJson) as NoLighting;

            lighting.Should().NotBeNull();
        }

        [Fact]
        public static void DeserializeObject_with_type_deserialize_json_using_MinimalSerializerSettings()
        {
            // If Minimal is being used then empty JSON string will deserialize into NoLighting
            // otherwise, out-of-the-box json.net will create an anonymous object
            var lightingJson = "{}";

            var lighting = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Lighting>), SerializationKind.Minimal).Deserialize(lightingJson, typeof(Lighting)) as NoLighting;

            lighting.Should().NotBeNull();
        }

        [Fact]
        public static void DeserializeObject_without_type_deserialize_json_into_JObject_using_MinimalSerializerSettings()
        {
            var dogJson = "{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10}";

            var dog = new NaosJsonSerializer(serializationKind: SerializationKind.Minimal).Deserialize<dynamic>(dogJson) as JObject;

            dog.Properties().Count().Should().Be(3);
            dog["name"].ToString().Should().Be("Barney");
            dog["age"].ToString().Should().Be("10");
            dog["furColor"].ToString().Should().Be("brindle");
        }
    }
}
