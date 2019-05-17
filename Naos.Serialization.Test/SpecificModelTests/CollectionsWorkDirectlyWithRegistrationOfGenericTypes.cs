// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionsWorkDirectlyWithRegistrationOfGenericTypes.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using FluentAssertions;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using Xunit;

    public static class CollectionsWorkDirectlyWithRegistrationOfGenericTypes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "bsonSerializer", Justification = "Adding bson support later.")]
        [Fact]
        public static void ElementTypeOfArrayIsOnlyTypeDiscovered()
        {
            // Arrange
            var bsonConfig = typeof(GenericDiscoveryBsonConfiguration<RegisteredKey, RegisteredValue>);
            var bsonSerializer = new NaosBsonSerializer(bsonConfig);

            var jsonConfig = typeof(GenericDiscoveryJsonConfiguration<RegisteredKey, RegisteredValue>);
            var jsonSerializer = new NaosJsonSerializer(jsonConfig);

            var expectedKey = new RegisteredKey { Property = A.Dummy<string>() };
            var expectedValue = new RegisteredValue { Property = A.Dummy<string>() };

            var expectedTuple = new Tuple<RegisteredKey, RegisteredValue>(expectedKey, expectedValue);
            var expectedDictionary = new Dictionary<RegisteredKey, RegisteredValue> { { expectedKey, expectedValue } };
            var expectedList = new List<RegisteredKey>(new[] { expectedKey });
            var expectedArray = new[] { expectedValue };

            // Act
            // var tupleBsonString = bsonSerializer.SerializeToString(expectedTuple);
            // var tupleBson = bsonSerializer.Deserialize<Tuple<RegisteredKey, RegisteredValue>>(tupleBsonString);

            var tupleJsonString = jsonSerializer.SerializeToString(expectedTuple);
            var tupleJson = jsonSerializer.Deserialize<Tuple<RegisteredKey, RegisteredValue>>(tupleJsonString);
            var dictionaryJsonString = jsonSerializer.SerializeToString(expectedDictionary);
            var dictionaryJson = jsonSerializer.Deserialize<Dictionary<RegisteredKey, RegisteredValue>>(dictionaryJsonString);
            var listJsonString = jsonSerializer.SerializeToString(expectedList);
            var listJson = jsonSerializer.Deserialize<List<RegisteredKey>>(listJsonString);
            var arrayJsonString = jsonSerializer.SerializeToString(expectedArray);
            var arrayJson = jsonSerializer.Deserialize<RegisteredValue[]>(arrayJsonString);

            // Assert

            // tupleBson.Should().BeEquivalentTo(expectedTuple);

            tupleJson.Item1.Property.Should().Be(expectedTuple.Item1.Property);
            tupleJson.Item2.Property.Should().Be(expectedTuple.Item2.Property);
            dictionaryJson.Single().Key.Property.Should().Be(expectedDictionary.Single().Key.Property);
            dictionaryJson.Single().Value.Property.Should().Be(expectedDictionary.Single().Value.Property);
            listJson.Single().Property.Should().Be(expectedList.Single().Property);
            arrayJson.Single().Property.Should().Be(expectedArray.Single().Property);
        }
    }

    public class RegisteredKey : IEquatable<RegisteredKey>
    {
        public string Property { get; set; }

        public bool Equals(RegisteredKey other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(this.Property, other.Property);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((RegisteredKey)obj);
        }

        public override int GetHashCode()
        {
            return this.Property != null ? this.Property.GetHashCode() : 0;
        }
    }

    public class RegisteredValue
    {
        public string Property { get; set; }
    }
}
