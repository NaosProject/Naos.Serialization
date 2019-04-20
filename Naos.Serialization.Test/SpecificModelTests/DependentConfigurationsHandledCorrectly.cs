// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependentConfigurationsHandledCorrectly.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy;
    using FluentAssertions;
    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OBeautifulCode.Validation.Recipes;
    using Xunit;
    using static System.FormattableString;
    using JsonReader = Newtonsoft.Json.JsonReader;
    using JsonToken = Newtonsoft.Json.JsonToken;
    using JsonWriter = Newtonsoft.Json.JsonWriter;

    public static class DependentConfigurationsHandledCorrectly
    {
        [Fact]
        public static void DependentConfigurationsAreDuplicatingStuffs()
        {
            // Arrange
            var bsonConfigType = typeof(BsonConfigA);
            var jsonConfigType = typeof(JsonConfigA);

            var expected = A.Dummy<TestingDependentConfigType>();

            void ThrowIfObjectsDiffer(DescribedSerialization serialized, TestingDependentConfigType deserialized)
            {
                deserialized.Should().NotBeNull();
            }

            // Act & Assert
            expected.RoundtripSerializeWithCallback(ThrowIfObjectsDiffer, jsonConfigType, bsonConfigType);
        }
    }

    public class JsonConfigA : JsonConfigurationBase
    {
        public override IReadOnlyCollection<Type> DependentConfigurationTypes =>
            new[] { typeof(JsonConfigB), typeof(JsonConfigC) };
    }

    public class BsonConfigA : BsonConfigurationBase
    {
        public override IReadOnlyCollection<Type> DependentConfigurationTypes =>
            new[] { typeof(BsonConfigB), typeof(BsonConfigC) };
    }

    public class JsonConfigB : JsonConfigurationBase
    {
        public override IReadOnlyCollection<Type> DependentConfigurationTypes =>
            new[] { typeof(JsonConfigC) };
    }

    public class BsonConfigB : BsonConfigurationBase
    {
        public override IReadOnlyCollection<Type> DependentConfigurationTypes =>
            new[] { typeof(BsonConfigC) };
    }

    public class JsonConfigC : JsonConfigurationBase
    {
        protected override IReadOnlyCollection<RegisteredJsonConverter> ConvertersToRegister => new[]
        {
            new RegisteredJsonConverter(
                () => new TestingDependentConverter(),
                () => new TestingDependentConverter(),
                A.Dummy<RegisteredJsonConverterOutputKind>(),
                new[] { typeof(TestingDependentConfigType) }),
        };
    }

    public class TestingDependentConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            var payload = ((TestingDependentConfigType)value).SomeValue;

            var payloadObject = new JValue(payload);
            payloadObject.WriteTo(writer);
        }

        /// <inheritdoc />
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var payload = reader.Value;
            var stringValue = payload.ToString();
            var result = new TestingDependentConfigType { SomeValue = stringValue };

            return result;
        }

        /// <inheritdoc />
        public override bool CanConvert(
            Type objectType)
        {
            return objectType == typeof(TestingDependentConfigType);
        }
    }

    public class BsonConfigC : BsonConfigurationBase
    {
        protected override IReadOnlyCollection<RegisteredBsonSerializer> SerializersToRegister => new[]
        {
            new RegisteredBsonSerializer(
                () => new TestingDependentSerializer(),
                new[] { typeof(TestingDependentConfigType) }),
        };
    }

    public class TestingDependentSerializer : SerializerBase<TestingDependentConfigType>
    {
        /// <inheritdoc />
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TestingDependentConfigType value)
        {
            new { context }.Must().NotBeNull();
            new { value }.Must().NotBeNull();

            context.Writer.WriteStartDocument();
            context.Writer.WriteName(nameof(value.SomeValue));
            context.Writer.WriteString(value.SomeValue);
            context.Writer.WriteEndDocument();
        }

        /// <inheritdoc />
        public override TestingDependentConfigType Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            new { context }.Must().NotBeNull();

            context.Reader.ReadStartDocument();
            context.Reader.ReadName(new Utf8NameDecoder());
            var type = context.Reader.GetCurrentBsonType();

            TestingDependentConfigType result;
            switch (type)
            {
                case BsonType.String:
                    result = new TestingDependentConfigType { SomeValue = context.Reader.ReadString() };
                    break;
                default:
                    throw new NotSupportedException(Invariant($"Cannot convert a {type} to a {nameof(TestingDependentConfigType)}."));
            }

            context.Reader.ReadEndDocument();

            return result;
        }
    }

    public class TestingDependentConfigType
    {
        public string SomeValue { get; set; }
    }
}
