// <copyright file="NaosBsonEnumStringSerializer.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>

namespace Naos.Serialization.Bson
{
    using System;

    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;

    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Represents a serializer for enums, including support for <see cref="FlagsAttribute"/> ones.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    public class NaosBsonEnumStringSerializer<TEnum> : StructSerializerBase<TEnum>
        where TEnum : struct
    {
        /// <inheritdoc />
        public override TEnum Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            new { context }.Must().NotBeNull();

            var bsonReader = context.Reader;

            var stringValue = bsonReader.ReadString();

            return (TEnum)Enum.Parse(typeof(TEnum), stringValue);
        }

        /// <inheritdoc />
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TEnum value)
        {
            new { context }.Must().NotBeNull();

            var bsonWriter = context.Writer;

            bsonWriter.WriteString(value.ToString());
        }
    }
}