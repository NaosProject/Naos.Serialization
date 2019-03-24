// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigurationTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security;
    using FluentAssertions;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using Newtonsoft.Json;
    using Xunit;

    public static class JsonConfigurationTest
    {
        [Fact]
        public static void Serializer_serializes_camel_cased_properties()
        {
            var value = new CamelCasedPropertyTest()
            {
                TestName = "Hello",
            };
            
            var result = new NaosJsonSerializer().SerializeToString(value);

            var serializedValue = "{" + Environment.NewLine +
                                  "  \"testName\": \"Hello\"" + Environment.NewLine +
                                  "}";

            result.Should().Be(serializedValue);
        }

        [Fact]
        public static void Serializer_deserialize_camel_cased_properties()
        {
            var serializedValue = "{" + Environment.NewLine +
                                  "  \"testName\": \"there\"" + Environment.NewLine +
                                  "}";

            var result = new NaosJsonSerializer().Deserialize<CamelCasedPropertyTest>(serializedValue);

            result.TestName.Should().Be("there");
        }

        [Fact]
        public static void Serializer_serializes_camel_cased_enumerations()
        {
            var value = new CamelCasedEnumTest()
            {
                Value = TestEnum.FirstOption,
            };

            var result = new NaosJsonSerializer().SerializeToString(value);

            var serializedValue = "{" + Environment.NewLine +
                                  "  \"value\": \"firstOption\"" + Environment.NewLine +
                                  "}";

            result.Should().Be(serializedValue);
        }

        [Fact]
        public static void Serializer_deserialize_camel_cased_enumerations()
        {
            var serializedValue = "{" + Environment.NewLine +
                                  "  \"value\": \"secondOption\"" + Environment.NewLine +
                                  "}";

            var result = new NaosJsonSerializer().Deserialize<CamelCasedEnumTest>(serializedValue);

            result.Value.Should().Be(TestEnum.SecondOption);
        }

        [Fact]
        public static void Serializer_serializes_and_deserialize_SecureString_types()
        {
            var serializedValue = "{" + Environment.NewLine +
                                  "  \"secure\": \"Password\"" + Environment.NewLine +
                                  "}";

            var deserialized = new NaosJsonSerializer().Deserialize<SecureStringTest>(serializedValue);

            var result = new NaosJsonSerializer().SerializeToString(deserialized);

            result.Should().Be(serializedValue);
        }

        [Fact]
        public static void Serializer_serializes_InheritedTypes()
        {
            var value = new InheritedTypeBase[]
            {
                new InheritedType1
                {
                    Base = "Base",
                    Child1 = "Child1",
                },
                new InheritedType2
                {
                    Base = "my base",
                    Child2 = "my child 2",
                },
            };

            var result = new NaosJsonSerializer().SerializeToString(value);

            var serializedValue = "[" + Environment.NewLine +
                                  "  {" + Environment.NewLine +
                                  "    \"child1\": \"Child1\"," + Environment.NewLine +
                                  "    \"base\": \"Base\"" + Environment.NewLine +
                                  "  }," + Environment.NewLine +
                                  "  {" + Environment.NewLine +
                                  "    \"child2\": \"my child 2\"," + Environment.NewLine +
                                  "    \"base\": \"my base\"" + Environment.NewLine +
                                  "  }" + Environment.NewLine +
                                  "]";

            result.Should().Be(serializedValue);
        }

        [Fact]
        public static void Serializer_deserialize_InheritedTypes()
        {
            var serializedValue = "[" + Environment.NewLine +
                                  "  {" + Environment.NewLine +
                                  "    \"child1\": \"My child 1\"," + Environment.NewLine +
                                  "    \"base\": \"My base\"" + Environment.NewLine +
                                  "  }," + Environment.NewLine +
                                  "  {" + Environment.NewLine +
                                  "    \"child2\": \"child 2\"," + Environment.NewLine +
                                  "    \"base\": \"base\"" + Environment.NewLine +
                                  "  }" + Environment.NewLine +
                                  "]";

            var result = new NaosJsonSerializer(typeof(GenericJsonConfiguration<InheritedTypeBase>)).Deserialize<InheritedTypeBase[]>(serializedValue);

            result.Length.Should().Be(2);
            result[0].Base.Should().Be("My base");
            (result[0] as InheritedType1).Should().NotBeNull();
            (result[0] as InheritedType1).Child1.Should().Be("My child 1");
            result[1].Base.Should().Be("base");
            (result[1] as InheritedType2).Should().NotBeNull();
            (result[1] as InheritedType2).Child2.Should().Be("child 2");
        }

        [Fact]
        public static void Serializer_deserialize_partial_InheritedTypes()
        {
            var serializedValue = "[" + Environment.NewLine +
                                  "  {" + Environment.NewLine +
                                  "    \"string\": \"My string\"," + Environment.NewLine +
                                  "    \"int32\": 5" + Environment.NewLine +
                                  "  }," + Environment.NewLine +
                                  "  {" + Environment.NewLine +
                                  "    \"int32\": 55," + Environment.NewLine +
                                  "    \"float\": 3.56" + Environment.NewLine +
                                  "  }" + Environment.NewLine +
                                  "]";

            var result = new NaosJsonSerializer(typeof(GenericJsonConfiguration<IBaseInterface>)).Deserialize<IBaseInterface[]>(serializedValue);

            result.Length.Should().Be(2);
            result[0].String.Should().Be("My string");
            (result[0] as InheritedType3).Should().NotBeNull();
            (result[0] as InheritedType3).Int32.Should().Be(5);
            (result[0] as InheritedType3).Float.Should().Be(default(float));
            result[1].String.Should().BeNull();
            (result[1] as InheritedType3).Should().NotBeNull();
            (result[1] as InheritedType3).Int32.Should().Be(55);
            Math.Round((result[1] as InheritedType3).Float, 2).Should().Be(Math.Round(3.56, 2));
        }

        [Fact]
        public static void Serializer_deserialize_inherited_types_with_constructors()
        {
            var dogJson = "{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10,\"dogTag\":\"my name is Barney\"}";
            var catJson = "{\"numberOfLives\":9,\"name\":\"Cleo\",\"age\":3}";
            var tigerJson = "{\"tailLength\":2,\"name\":\"Ronny\",\"numberOfTeeth\":50,\"age\":5}";
            var mouseJson = "{\"tailLength\":8,\"name\":\"Missy\",\"furColor\":\"black\",\"age\":7}";

            var jsonSerializer = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>));

            var dog = jsonSerializer.Deserialize<Animal>(dogJson) as Dog;
            var cat = jsonSerializer.Deserialize<Animal>(catJson) as Cat;
            var tiger = jsonSerializer.Deserialize<Animal>(tigerJson) as Tiger;
            var mouse = jsonSerializer.Deserialize<Animal>(mouseJson) as Mouse;

            dog.Should().NotBeNull();
            dog.Name.Should().Be("Barney");
            dog.Age.Should().Be(10);
            dog.FurColor.Should().Be(FurColor.Brindle);
            dog.DogTag.Should().Be("my name is Barney");

            cat.Should().NotBeNull();
            cat.Name.Should().Be("Cleo");
            cat.Age.Should().Be(3);
            cat.NumberOfLives.Should().Be(9);

            tiger.Should().NotBeNull();
            tiger.Name.Should().Be("Ronny");
            tiger.Age.Should().Be(5);
            tiger.TailLength.Should().Be(2);
            tiger.NumberOfTeeth.Should().Be(50);

            mouse.Should().NotBeNull();
            mouse.Name.Should().Be("Missy");
            mouse.Age.Should().Be(7);
            mouse.TailLength.Should().Be(8);
            mouse.FurColor.Should().Be(FurColor.Black);
        }

        [Fact]
        public static void Serializer_deserialize_to_type_having_all_constructor_parameters_in_json_when_another_types_constructor_has_all_the_same_parameters_but_one_additional_one_which_is_not_in_the_json()
        {
            var dogJson = "{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10}";

            var dog = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>)).Deserialize<Animal>(dogJson) as Dog;

            dog.Should().NotBeNull();
            dog.Name.Should().Be("Barney");
            dog.Age.Should().Be(10);
            dog.FurColor.Should().Be(FurColor.Brindle);
            dog.DogTag.Should().Be("my name is Barney");
        }

        [Fact]
        public static void Serializer_throws_JsonSerializationException_when_json_deserialize_to_multiple_child_types_and_lists_all_possible_child_types_when_none_strictly_match()
        {
            var inheritedTypeJson = "{\"base\":\"my base string\"}";

            var ex = Assert.Throws<JsonSerializationException>(() => new NaosJsonSerializer(typeof(GenericJsonConfiguration<InheritedTypeBase>)).Deserialize<InheritedTypeBase>(inheritedTypeJson));
            ex.Message.Should().Contain("InheritedType1");
            ex.Message.Should().Contain("InheritedType2");
        }

        [Fact]
        public static void Serializer_throws_JsonSerializationException_when_json_deserialize_to_multiple_child_types_and_lists_only_strictly_matching_child_types_when_there_are_some_that_strictly_match()
        {
            var lightingJson = "{\"watts\":10, \"wattageEquivalent\":60}";

            var ex = Assert.Throws<JsonSerializationException>(() => new NaosJsonSerializer(typeof(GenericJsonConfiguration<Lighting>)).Deserialize<Lighting>(lightingJson));

            ex.Message.Should().Contain("CompactFluorescent");
            ex.Message.Should().Contain("Led");
            ex.Message.Should().NotContain("SmartLed");
        }

        [Fact]
        public static void Serializer_deserialize_to_only_child_type_that_strictly_matches_json_when_json_can_deserialize_into_multiple_child_types()
        {
            var noLightingJson = "{}";
            var incandescentJson = "{\"watts\":60}";

            var jsonSerializer = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Lighting>));

            var noLighting = jsonSerializer.Deserialize<Lighting>(noLightingJson) as NoLighting;
            var incandescent = jsonSerializer.Deserialize<Lighting>(incandescentJson) as Incandescent;

            noLighting.Should().NotBeNull();

            incandescent.Should().NotBeNull();
            incandescent.Watts.Should().Be(60);
        }

        [Fact]
        public static void Serializer_throws_JsonSerializationException_when_json_cannot_be_deserialized_into_any_child_types()
        {
            var json = "{\"none\":\"none\"}";

            var ex = Assert.Throws<JsonSerializationException>(() => new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>)).Deserialize<Animal>(json));

            ex.Message.Should().StartWith("The json payload could not be deserialized into any of the candidate types.");
        }

        [Fact]
        public static void Serializer_deserialize_inherited_types_when_null_property_is_not_included_in_json()
        {
            var json = "{\"float\":.2,\"int32\":50}";

            var inheritedType3 = new NaosJsonSerializer(typeof(GenericJsonConfiguration<IBaseInterface>)).Deserialize<IBaseInterface>(json) as InheritedType3;

            inheritedType3.Should().NotBeNull();
            inheritedType3.Int32.Should().Be(50);
            inheritedType3.Float.Should().Be(.2f);
            inheritedType3.String.Should().BeNull();
        }

        [Fact]
        public static void Serializer_throws_JsonSerializationException_when_json_has_extra_properties_not_available_on_any_child_type()
        {
            var tigerJson = "{\"tailLength\":2,\"name\":\"Ronny\",\"numberOfTeeth\":50,\"age\":5, \"newProperty\":66}";

            var ex = Assert.Throws<JsonSerializationException>(() => new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>)).Deserialize<Animal>(tigerJson));

            ex.Message.Should().StartWith("The json payload could not be deserialized into any of the candidate types.");
        }

        [Fact]
        public static void Serializer_throws_JsonSerializationException_when_candidate_child_type_cannot_be_deserialized_because_property_type_in_json_does_not_match_childs_property_type()
        {
            // name was changed from string to object
            var tigerJson = "{\"tailLength\":2,\"name\":{ \"first\":\"Ronny\",\"last\":\"Ronnerson\" },\"numberOfTeeth\":50,\"age\":5}";

            var ex = Assert.Throws<JsonSerializationException>(() => new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>)).Deserialize<Animal>(tigerJson));

            ex.Message.Should().StartWith("The json payload could not be deserialized into any of the candidate types.");
        }

        [Fact]
        public static void Serializer_deserialize_empty_json_into_the_only_child_type_that_has_a_default_constructor()
        {
            var atkinsJson = "{}";

            var atkins = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Diet>)).Deserialize<Diet>(atkinsJson) as Atkins;

            atkins.Should().NotBeNull();
        }

        [Fact]
        public static void Serializer_throws_JsonSerializationException_when_a_constructor_parameter_is_missing_in_json_and_that_parameter_is_a_property_on_the_child_type()
        {
            var catJson1 = "{\"numberOfLives\":9,\"name\":\"Cleo\"}";
            var catJson2 = "{\"numberOfLives\":9,\"age\":3}";

            var jsonSerializer = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Animal>));

            var ex1 = Assert.Throws<JsonSerializationException>(() => jsonSerializer.Deserialize<Animal>(catJson1));
            var ex2 = Assert.Throws<JsonSerializationException>(() => jsonSerializer.Deserialize<Animal>(catJson2));

            ex1.Message.Should().StartWith("The json payload could not be deserialized into any of the candidate types.");
            ex2.Message.Should().StartWith("The json payload could not be deserialized into any of the candidate types.");
        }

        [Fact]
        public static void Serializer_throws_JsonSerializationException_when_a_constructor_parameter_is_missing_in_json_and_that_parameter_is_not_a_property_on_the_child_type()
        {
            var fructoseJson = "{\"minOuncesOfFructose\":5}";

            var ex = Assert.Throws<JsonSerializationException>(() => new NaosJsonSerializer(typeof(GenericJsonConfiguration<Diet>)).Deserialize<Diet>(fructoseJson));

            ex.Message.Should().StartWith("The json payload could not be deserialized into any of the candidate types.");
        }

        [Fact]
        public static void Serializer_throws_JsonSerializationException_when_constructor_parameter_is_in_json_but_type_has_no_corresponding_property()
        {
            var fructoseJson = "{\"minGramsOfFructose\":5}";

            var ex = Assert.Throws<JsonSerializationException>(() => new NaosJsonSerializer(typeof(GenericJsonConfiguration<Diet>)).Deserialize<Diet>(fructoseJson));

            ex.Message.Should().StartWith("The json payload could not be deserialized into any of the candidate types.");
        }

        [Fact]
        public static void Serializer_deserialize_type_with_no_constructor_that_has_a_property_that_is_an_inherited_type()
        {
            var dogDietJson = "{\"dog\":{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10}, \"diet\":{}}";

            var dogDiet = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Diet>)).Deserialize<DogDiet>(dogDietJson);

            dogDiet.Should().NotBeNull();

            dogDiet.Dog.Should().NotBeNull();
            dogDiet.Dog.Name.Should().Be("Barney");
            dogDiet.Dog.Age.Should().Be(10);
            dogDiet.Dog.FurColor.Should().Be(FurColor.Brindle);
            dogDiet.Dog.DogTag.Should().Be("my name is Barney");

            dogDiet.Diet.Should().BeOfType<Atkins>();
        }

        [Fact]
        public static void Serializer_deserialize_type_with_no_constructor_that_has_a_property_that_is_an_inherited_type_and_is_null_in_json()
        {
            var dogDietJson = "{\"dog\":{\"name\":\"Barney\",\"furColor\":\"brindle\",\"age\":10}, \"diet\":null}";

            var dogDiet = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Diet>)).Deserialize<DogDiet>(dogDietJson);

            dogDiet.Should().NotBeNull();

            dogDiet.Dog.Should().NotBeNull();
            dogDiet.Dog.Name.Should().Be("Barney");
            dogDiet.Dog.Age.Should().Be(10);
            dogDiet.Dog.FurColor.Should().Be(FurColor.Brindle);
            dogDiet.Dog.DogTag.Should().Be("my name is Barney");

            dogDiet.Diet.Should().BeNull();
        }

        [Fact]
        public static void Serializer_deserialize_type_with_constructor_that_has_a_property_that_is_an_inherited_type()
        {
            var catDietJson = "{\"cat\":{\"numberOfLives\":9,\"name\":\"Cleo\",\"age\":3}, \"diet\":{}}";
            
            var catDiet = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Diet>)).Deserialize<CatDiet>(catDietJson);

            catDiet.Should().NotBeNull();
            catDiet.Cat.Should().NotBeNull();
            catDiet.Cat.Name.Should().Be("Cleo");
            catDiet.Cat.Age.Should().Be(3);
            catDiet.Cat.NumberOfLives.Should().Be(9);

            catDiet.Diet.Should().BeOfType<Atkins>();
        }

        [Fact]
        public static void Serializer_deserialize_type_with_constructor_that_has_a_property_that_is_an_inherited_type_and_is_null_in_json()
        {
            var catDietJson = "{\"cat\":{\"numberOfLives\":9,\"name\":\"Cleo\",\"age\":3}, \"diet\":null}";

            var catDiet = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Diet>)).Deserialize<CatDiet>(catDietJson);

            catDiet.Should().NotBeNull();
            catDiet.Cat.Should().NotBeNull();
            catDiet.Cat.Name.Should().Be("Cleo");
            catDiet.Cat.Age.Should().Be(3);
            catDiet.Cat.NumberOfLives.Should().Be(9);

            catDiet.Diet.Should().BeNull();
        }

        [Fact]
        public static void Serializer_deserialize_type_where_constructor_parameter_is_different_type_than_corresponding_property_but_is_assignable_from_that_property_type()
        {
            var familyJson = "{\"firstNames\": [\"joe\",\"jane\",\"jackie\"]}";

            var family = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Family>)).Deserialize<Family>(familyJson);
            var expectedFirstNames = new[] { "joe", "jane", "jackie" };

            family.Should().NotBeNull();
            expectedFirstNames.Should().BeEquivalentTo(family.FirstNames);
        }

        [Fact]
        public static void Serializer_throws_JsonSerializationException_when_deserializing_type_where_constructor_parameter_is_different_type_than_corresponding_property_and_is_not_assignable_from_that_property_type()
        {
            var friendsJson = "{\"firstNames\": [\"betty\",\"bob\",\"bailey\"]}";

            Assert.Throws<JsonSerializationException>(() => new NaosJsonSerializer(typeof(GenericJsonConfiguration<Friends>)).Deserialize<Friends>(friendsJson));            
        }

        [Fact]
        public static void Serializer_serializes_object_where_constructor_parameter_is_different_type_than_corresponding_property_but_is_assignable_from_that_property_type()
        {
            var family = new Family(new List<string> { "joe", "jane", "jackie" });
            var expectedFamilyJson = "{\r\n  \"firstNames\": [\r\n    \"joe\",\r\n    \"jane\",\r\n    \"jackie\"\r\n  ],\r\n  \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Family, Naos.Serialization.Test\"\r\n}";

            var actualFamilyJson = new NaosJsonSerializer(typeof(GenericJsonConfiguration<Family>)).SerializeToString(family);

            expectedFamilyJson.Should().Be(actualFamilyJson);
        }

        [Fact]
        public static void Serializer_throws_JsonSerializationException_when_serializing_object_where_constructor_parameter_is_different_type_than_corresponding_property_and_is_not_assignable_from_that_property_type()
        {
            var friends = new Friends(new List<string> { "betty", "bob", "bailey" });

            Assert.Throws<JsonSerializationException>(() => new NaosJsonSerializer(typeof(GenericJsonConfiguration<Friends>)).SerializeToString(friends));
        }

        [Fact]
        public static void Serializer_deserialize_type_where_constructor_does_not_throw_exception_when_another_candidate_has_constructor_that_does_throw_exception()
        {
            var sometimesThrowsJson = "{\"triggerNumber\":123456}";

            var doesNotThrow =
                new NaosJsonSerializer(typeof(GenericJsonConfiguration<SometimesThrows>)).Deserialize<SometimesThrows>(sometimesThrowsJson) as DoesNotThrow;
            
            doesNotThrow.Should().NotBeNull();
            doesNotThrow.TriggerNumber.Should().Be(123456);
        }

        [Fact]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "parentis", Justification = "this is not a compound word")]
        public static void Serializer_serializes_concrete_type_whose_abstract_parent_is_TwoWay_bindable_with_type_information_written_into_the_json()
        {
            var starfish = new Starfish();
            var crab = new Crab(SeaCreatureSize.Large);
            var salmon = new Salmon(SeaCreatureSize.Medium, "brown");

            var expectedStarfishJson = "{\r\n  \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Starfish, Naos.Serialization.Test\"\r\n}";
            var expectedCrabJson = "{\r\n  \"size\": \"large\",\r\n  \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Crab, Naos.Serialization.Test\"\r\n}";
            var expectedSalmonJson = "{\r\n  \"color\": \"brown\",\r\n  \"size\": \"medium\",\r\n  \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Salmon, Naos.Serialization.Test\"\r\n}";

            var actualStarfishJson = new NaosJsonSerializer(typeof(GenericJsonConfiguration<SeaCreature>)).SerializeToString(starfish);
            var actualCrabJson = new NaosJsonSerializer(typeof(GenericJsonConfiguration<SeaCreature>)).SerializeToString(crab);
            var actualSalmonJson = new NaosJsonSerializer(typeof(GenericJsonConfiguration<SeaCreature>)).SerializeToString(salmon);

            expectedStarfishJson.Should().Be(actualStarfishJson);
            expectedCrabJson.Should().Be(actualCrabJson);
            expectedSalmonJson.Should().Be(actualSalmonJson);
        }

        [Fact]
        public static void Serializer_deserialize_into_concrete_type_where_multiple_inherited_types_have_the_same_properties_and_abstract_type_is_marked_TwoWay_bindable()
        {
            var salmonJson = "{\r\n  \"color\": \"brown\",\r\n  \"size\": \"medium\",\r\n  \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Salmon, Naos.Serialization.Test\"\r\n}";

            var salmon = new NaosJsonSerializer(typeof(GenericJsonConfiguration<SeaCreature>)).Deserialize<Salmon>(salmonJson);

            salmon.Should().NotBeNull();
            salmon.Color.Should().Be("brown");
            salmon.Size.Should().Be(SeaCreatureSize.Medium);
        }

        [Fact]
        public static void Serializer_deserialize_into_abstract_type_where_multiple_inherited_types_have_the_same_properties_using_type_information_written_into_json()
        {
            var salmonJson = "{\r\n  \"color\": \"brown\",\r\n  \"size\": \"medium\",\r\n  \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Salmon, Naos.Serialization.Test\"\r\n}";

            var salmon1 = new NaosJsonSerializer(typeof(GenericJsonConfiguration<SeaCreature>)).Deserialize<SeaCreature>(salmonJson) as Salmon;
            var salmon2 = new NaosJsonSerializer(typeof(GenericJsonConfiguration<SeaCreature>)).Deserialize<Fish>(salmonJson) as Salmon;

            salmon1.Should().NotBeNull();
            salmon1.Color.Should().Be("brown");
            salmon1.Size.Should().Be(SeaCreatureSize.Medium);

            salmon2.Should().NotBeNull();
            salmon2.Color.Should().Be("brown");
            salmon2.Size.Should().Be(SeaCreatureSize.Medium);
        }

        [Fact]
        public static void Serializer_serializes_TwoWay_bindable_type_that_embeds_a_OneWay_bindable_type_using_specified_serializer()
        {
            var whale = new Whale("willy", new LowCalorie(50000));
            
            var expectedWhaleJson = "{\r\n  \"name\": \"willy\",\r\n  \"diet\": {\r\n    \"maxCalories\": 50000\r\n  },\r\n  \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Whale, Naos.Serialization.Test\"\r\n}";
            
            var actualWhaleJson = new NaosJsonSerializer(typeof(GenericJsonConfiguration<SeaCreature>)).SerializeToString(whale);

            expectedWhaleJson.Should().Be(actualWhaleJson);
        }

        [Fact]
        public static void Serializer_deserialize_TwoWay_bindable_type_into_abstract_type_when_concrete_type_embeds_a_OneWay_bindable_type_using_specified_serializer()
        {
            var whaleJson = "{\r\n  \"name\": \"willy\",\r\n  \"diet\": {\r\n    \"maxCalories\": 50000\r\n  },\r\n  \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Whale, Naos.Serialization.Test\"\r\n}";

            var whale = new NaosJsonSerializer(typeof(GenericJsonConfiguration<SeaCreature, Diet>)).Deserialize<SeaCreature>(whaleJson) as Whale;

            whale.Should().NotBeNull();
            whale.Name.Should().Be("willy");
            whale.Diet.Should().NotBeNull();
            whale.Diet.Should().BeOfType<LowCalorie>();
            ((LowCalorie)whale.Diet).MaxCalories.Should().Be(50000);
        }

        [Fact]
        public static void Serializer_serializes_TwoWay_bindable_type_that_embeds_a_TwoWay_bindable_type_using_specified_serializer()
        {
            var tuna = new Tuna(SeaCreatureSize.Medium, "black");
            var shark = new Shark("sammy", tuna);

            var expectedSharkJson = "{\r\n  \"name\": \"sammy\",\r\n  \"likesToEat\": {\r\n    \"color\": \"black\",\r\n    \"size\": \"medium\",\r\n    \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Tuna, Naos.Serialization.Test\"\r\n  },\r\n  \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Shark, Naos.Serialization.Test\"\r\n}";

            var actualSharkJson = new NaosJsonSerializer(typeof(GenericJsonConfiguration<SeaCreature>)).SerializeToString(shark);

            expectedSharkJson.Should().Be(actualSharkJson);
        }

        [Fact]
        public static void Serializer_deserialize_TwoWay_bindable_type_into_abstract_type_when_concrete_type_embeds_a_TwoWay_bindable_type_using_specified_serializer()
        {
            var sharkJson = "{\r\n  \"name\": \"sammy\",\r\n  \"likesToEat\": {\r\n    \"color\": \"black\",\r\n    \"size\": \"medium\",\r\n    \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Tuna, Naos.Serialization.Test\"\r\n  },\r\n  \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Shark, Naos.Serialization.Test\"\r\n}";

            var shark = new NaosJsonSerializer(typeof(GenericJsonConfiguration<SeaCreature>)).Deserialize<SeaCreature>(sharkJson) as Shark;

            shark.Should().NotBeNull();
            shark.Name.Should().Be("sammy");
            shark.LikesToEat.Should().NotBeNull();
            shark.LikesToEat.Should().BeOfType<Tuna>();
            ((Tuna)shark.LikesToEat).Size.Should().Be(SeaCreatureSize.Medium);
            ((Tuna)shark.LikesToEat).Color.Should().Be("black");            
        }

        [Fact]
        public static void Serializer_serializes_OneWay_bindable_type_that_embeds_a_TwoWay_bindable_type_using_specified_serializer()
        {
            var seafoodDiet = new SeafoodDiet(new Salmon(SeaCreatureSize.Medium, "red"), 345);
            
            var expectedSeafoodDietJson = "{\r\n  \"seaCreature\": {\r\n    \"color\": \"red\",\r\n    \"size\": \"medium\",\r\n    \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Salmon, Naos.Serialization.Test\"\r\n  },\r\n  \"amount\": 345\r\n}";

            var actualSeafoodDietJson = new NaosJsonSerializer(typeof(GenericJsonConfiguration<SeaCreature>)).SerializeToString(seafoodDiet);

            expectedSeafoodDietJson.Should().Be(actualSeafoodDietJson);
        }

        [Fact]
        public static void Serializer_deserialize_OneWay_bindable_type_into_abstract_type_when_concrete_type_embeds_a_TwoWay_bindable_type_using_specified_serializer()
        {
            var seafoodDietJson = "{\r\n  \"seaCreature\": {\r\n    \"color\": \"red\",\r\n    \"size\": \"medium\",\r\n    \"$concreteType\": \"Naos.Serialization.Test.JsonConfigurationTest+Salmon, Naos.Serialization.Test\"\r\n  },\r\n  \"amount\": 345\r\n}";

            var seafoodDiet = new NaosJsonSerializer(typeof(GenericJsonConfiguration<SeaCreature, Diet>)).Deserialize<Diet>(seafoodDietJson) as SeafoodDiet;

            seafoodDiet.Should().NotBeNull();
            seafoodDiet.Amount.Should().Be(345);
            seafoodDiet.SeaCreature.Should().NotBeNull();
            seafoodDiet.SeaCreature.Should().BeOfType<Salmon>();
            ((Salmon)seafoodDiet.SeaCreature).Color.Should().Be("red");
            ((Salmon)seafoodDiet.SeaCreature).Size.Should().Be(SeaCreatureSize.Medium);
        }

        [Fact]
        public static void Serializer_roundtrips_type_with_dictionary_properties_whose_keys_are_strings_or_enums_without_manipulating_case_of_first_letter_in_string_key()
        {
            var expected = new DictionaryPropertiesTest
            {
                Names = new Dictionary<string, string>
                {
                    { "Joe", "Locks" },
                    { "sally", "fields" },
                },
                ReadOnlyNames = new ReadOnlyDictionary<string, string>(
                    new Dictionary<string, string>
                    {
                        { "billy", "Bob" },
                        { "Harry", "wright" },
                    }),
                NamesByColor = new Dictionary<Color, string>
                {
                    { Color.Green, "Billy" },
                    { Color.White, "jean" },
                },
            };

            var json = new NaosJsonSerializer().SerializeToString(expected);

            var actual = new NaosJsonSerializer().Deserialize<DictionaryPropertiesTest>(json);

            actual.Should().NotBeNull();
            expected.Names.Should().BeEquivalentTo(actual.Names);
            expected.ReadOnlyNames.Should().BeEquivalentTo(actual.ReadOnlyNames);
            expected.NamesByColor.Should().BeEquivalentTo(actual.NamesByColor);
        }

        [Fact]
        public static void Presence_of_concreteType_token_does_not_interfere_with_normal_deserialization()
        {
            // Arrange
            var expectedId = "year-field";
            var expectedDecimalPlaces = 5;
            var expectedTitle = "my-title";

            var year = new YearField(expectedId)
            {
                NumberOfDecimalPlaces = expectedDecimalPlaces,
                Title = expectedTitle,
            };

            var serializer = new NaosJsonSerializer(typeof(FieldConfiguration));

            var jsonWithConcreteType = serializer.SerializeToString(year);

            var settings = new NullJsonConfiguration().BuildJsonSerializerSettings(SerializationDirection.Deserialize);
            settings.Converters = new JsonConverter[0];

            // Act
            var actual = JsonConvert.DeserializeObject<YearField>(jsonWithConcreteType, settings);

            // Assert
            jsonWithConcreteType.Should().Contain("$concreteType");
            actual.Id.Should().Be(expectedId);
            actual.NumberOfDecimalPlaces.Should().Be(expectedDecimalPlaces);
            actual.Title.Should().Be(expectedTitle);
        }

        private class CamelCasedPropertyTest
        {
            public string TestName;
        }

        private class CamelCasedEnumTest
        {
            public TestEnum Value;
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used by code external to test")]
        private class SecureStringTest
        {
#pragma warning disable 649

            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Field is used by code external to test")]
            public SecureString Secure;

#pragma warning restore
        }

        private enum TestEnum
        {
            FirstOption,
            SecondOption,
        }

        private class InheritedTypeBase
        {
            public string Base;
        }

        private class InheritedType1 : InheritedTypeBase
        {
            public string Child1;
        }

        private class InheritedType2 : InheritedTypeBase
        {
            public string Child2;
        }

        private interface IBaseInterface
        {
            string String { get; set; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used via reflection and code analysis cannot detect that.")]
        private class InheritedType3 : IBaseInterface
        {
            public float Float { get; set; }

            public int Int32 { get; set; }

            public string String { get; set; }
        }

        private class Diet
        {
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
        private class Atkins : Diet
        {
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
        private class LowCalorie : Diet
        {
            public LowCalorie(int maxCalories)
            {
                this.MaxCalories = maxCalories;
            }

            public int MaxCalories { get; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
        private class Vegan : Diet
        {
            public Vegan(bool isHoneyAllowed)
            {
                this.IsHoneyAllowed = isHoneyAllowed;
            }

            public bool IsHoneyAllowed { get; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
        private class HighFructose : Diet
        {
            public HighFructose(int minGramsOfFructose)
            {
                this.MinOuncesOfFructose = minGramsOfFructose * .035;
            }

            public double MinOuncesOfFructose { get; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
        private class DogDiet
        {
            public Dog Dog { get; set; }

            public Diet Diet { get; set; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
        private class CatDiet
        {
            public CatDiet(Cat cat, Diet diet)
            {
                this.Cat = cat;
                this.Diet = diet;
            }

            public Cat Cat { get; }

            public Diet Diet { get; }
        }

        private class Family
        {
            public Family(IEnumerable<string> firstNames)
            {
                if (firstNames == null)
                {
                    throw new ArgumentNullException(nameof(firstNames));
                }

                this.FirstNames = new ReadOnlyCollection<string>(firstNames.ToList());
            }

            public IReadOnlyCollection<string> FirstNames { get; }
        }

        private class Friends
        {
            public Friends(IReadOnlyCollection<string> firstNames)
            {
                if (firstNames == null)
                {
                    throw new ArgumentNullException(nameof(firstNames));
                }

                this.FirstNames = firstNames;
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Property is used via reflection and code analysis cannot detect that ")]
            public IEnumerable<string> FirstNames { get; }
        }

        private abstract class SometimesThrows
        {
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
        private class DoesNotThrow : SometimesThrows
        {
            public DoesNotThrow(int triggerNumber)
            {
                this.TriggerNumber = triggerNumber;
            }

            public int TriggerNumber { get; set; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
        private class DoesThrow : SometimesThrows
        {
            public DoesThrow(int triggerNumber)
            {
                if (triggerNumber == 123456)
                {
                    throw new ArgumentException("hit the trigger!");
                }

                this.TriggerNumber = triggerNumber;
            }

            public int TriggerNumber { get; set; }
        }
        
        private class SeaCreature
        {
        }

        private class Starfish : SeaCreature
        {
        }

        private enum SeaCreatureSize
        {
            Small,

            Medium,

            Large,
        }

        private class Crab : SeaCreature
        {
            public Crab(SeaCreatureSize size)
            {
                this.Size = size;
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
            public SeaCreatureSize Size { get; }
        }

        private abstract class Fish : SeaCreature
        {
            public Fish(SeaCreatureSize size)
            {
                this.Size = size;
            }

            public SeaCreatureSize Size { get; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
        private class Tuna : Fish
        {
            public Tuna(SeaCreatureSize size, string color)
                : base(size)
            {
                this.Color = color;
            }

            public string Color { get; }
        }

        private class Salmon : Fish
        {
            public Salmon(SeaCreatureSize size, string color)
                : base(size)
            {
                this.Color = color;
            }

            public string Color { get; }
        }

        private class Whale : SeaCreature
        {
            public Whale(string name, Diet diet)
            {
                this.Name = name;
                this.Diet = diet;
            }

            public string Name { get; }

            public Diet Diet { get; }
        }

        private class Shark : SeaCreature
        {
            public Shark(string name, SeaCreature likesToEat)
            {
                this.Name = name;
                this.LikesToEat = likesToEat;
            }

            public string Name { get; }

            public SeaCreature LikesToEat { get; }
        }

        private class SeafoodDiet : Diet
        {
            public SeafoodDiet(SeaCreature seaCreature, int amount)
            {
                this.SeaCreature = seaCreature;
                this.Amount = amount;
            }

            public SeaCreature SeaCreature { get; }

            public int Amount { get; }
        }

        private enum Color
        {
            Brown,

            Blue,

            Green,

            White,
        }

        private class DictionaryPropertiesTest
        {
            public Dictionary<string, string> Names { get; set; }

            public IReadOnlyDictionary<string, string> ReadOnlyNames { get; set; }

            public Dictionary<Color, string> NamesByColor { get; set; }
        }
    }
}
