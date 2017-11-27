// <copyright file="NaosBsonHashAlgorithmNameStringSerializer.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>

namespace Naos.Serialization.Bson
{
    using System;
    using System.Security.Cryptography;

    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;

    using Spritely.Recipes;

    /// <summary>
    /// Represents a serializer for enums, including support for <see cref="FlagsAttribute"/> ones.
    /// </summary>
    public class NaosBsonHashAlgorithmNameStringSerializer : StructSerializerBase<HashAlgorithmName>
    {
        private const string DefaultHashAlgorithmNameStringValue = "default";

        /// <inheritdoc />
        public override HashAlgorithmName Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            new { context }.Must().NotBeNull().OrThrowFirstFailure();

            var bsonReader = context.Reader;

            var stringValue = bsonReader.ReadString();

            if (stringValue == DefaultHashAlgorithmNameStringValue)
            {
                return default(HashAlgorithmName);
            }
            else
            {
                return new HashAlgorithmName(stringValue);
            }
        }

        /// <inheritdoc />
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, HashAlgorithmName value)
        {
            new { context }.Must().NotBeNull().OrThrowFirstFailure();

            var bsonWriter = context.Writer;

            var stringValueToWrite = value == default(HashAlgorithmName) ? DefaultHashAlgorithmNameStringValue : value.ToString();

            bsonWriter.WriteString(stringValueToWrite);
        }
    }
}