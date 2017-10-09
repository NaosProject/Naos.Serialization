// <copyright file="NaosBsonEnumStringSerializer.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>

namespace Naos.Serialization.Bson
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;

    using Naos.Serialization.Domain;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Represents a serializer for enums, including support for <see cref="FlagsAttribute"/> ones.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    public class NaosBsonEnumStringSerializer<TEnum> : StructSerializerBase<TEnum>
        where TEnum : struct
    {
        /// <inheritdoc cref="SerializerBase{TValue}" />
        public override TEnum Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            new { context }.Must().NotBeNull().OrThrowFirstFailure();

            var bsonReader = context.Reader;

            var stringValue = bsonReader.ReadString();

            return (TEnum)Enum.Parse(typeof(TEnum), stringValue);
        }

        /// <inheritdoc cref="SerializerBase{TValue}" />
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TEnum value)
        {
            new { context }.Must().NotBeNull().OrThrowFirstFailure();

            var bsonWriter = context.Writer;

            bsonWriter.WriteString(value.ToString());
        }
    }
}