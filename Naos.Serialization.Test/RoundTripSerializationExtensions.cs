// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoundtripSerializationExtensions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public static class RoundtripSerializationExtensions
    {
        public delegate void RoundtripSerializationCallback<in T>(DescribedSerialization yieldedDescribedSerialization, T deserializedObject);

        public static void RoundtripSerializeWithEquatableAssertion<T>(
            this T expected,
            bool shouldUseConfiguration = true)
            where T : IEquatable<T>
        {
            RoundtripSerializeWithCallback(
                expected,
                (yieldedDescribedSerialization, deserializedObject) => deserializedObject.Should().Be(expected),
                shouldUseConfiguration ? typeof(GenericJsonConfiguration<T>) : null,
                shouldUseConfiguration ? typeof(GenericBsonConfiguration<T>) : null);
        }

        public static void RoundtripSerializeWithCallback<T>(
            this T expected,
            RoundtripSerializationCallback<T> validationCallback,
            Type jsonConfigType = null,
            Type bsonConfigType = null,
            bool testBson = true,
            bool testJson = true)
        {
            var serializers = new List<ISerializeAndDeserialize>();

            if (testJson)
            {
                var jsonSerializer = new NaosJsonSerializer(jsonConfigType);
                serializers.Add(jsonSerializer);
            }

            if (testBson)
            {
                var bsonSerializer = new NaosBsonSerializer(bsonConfigType);
                serializers.Add(bsonSerializer);
            }

            if (!serializers.Any())
            {
                throw new InvalidOperationException("No serializers are being tested.");
            }

            foreach (var serializer in serializers)
            {
                var configurationTypeDescription = serializer.ConfigurationType.ToTypeDescription();

                var stringDescription = new SerializationDescription(
                    serializer.Kind,
                    SerializationFormat.String,
                    configurationTypeDescription);

                var binaryDescription = new SerializationDescription(
                    serializer.Kind,
                    SerializationFormat.Binary,
                    configurationTypeDescription);

                var actualString = expected.ToDescribedSerializationUsingSpecificSerializer(stringDescription, serializer);
                var actualBinary = expected.ToDescribedSerializationUsingSpecificSerializer(binaryDescription, serializer);

                var actualFromString = actualString.DeserializePayloadUsingSpecificSerializer(serializer);
                var actualFromBinary = actualBinary.DeserializePayloadUsingSpecificSerializer(serializer);

                try
                {
                    validationCallback(actualString, (T)actualFromString);
                }
                catch (Exception ex)
                {
                    throw new NaosSerializationException(Invariant($"Failure confirming from string with {nameof(serializer)} - {serializer.GetType()} - {actualFromString}"), ex);
                }

                try
                {
                    validationCallback(actualBinary, (T)actualFromBinary);
                }
                catch (Exception ex)
                {
                    throw new NaosSerializationException(Invariant($"Failure confirming from binary with {nameof(serializer)} - {serializer.GetType()} - {actualFromBinary}"), ex);
                }
            }
        }
    }
}
