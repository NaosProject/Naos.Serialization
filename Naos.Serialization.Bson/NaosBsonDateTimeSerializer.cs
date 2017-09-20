// <copyright file="NaosBsonDateTimeSerializer.cs" company="Naos">
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
    /// Custom <see cref="DateTime"/> serializer to do the right thing.
    /// </summary>
    public class NaosBsonDateTimeSerializer : SerializerBase<DateTime>
    {
        private static readonly IStringSerializeAndDeserialize StringSerializer = new NaosDateTimeStringSerializer();

        /// <inheritdoc cref="SerializerBase{T}" />
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
        {
            new { context }.Must().NotBeNull().OrThrow();

            context.Writer.WriteString(StringSerializer.SerializeToString(value));
        }

        /// <inheritdoc cref="SerializerBase{T}" />
        public override DateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            new { context }.Must().NotBeNull().OrThrow();

            var type = context.Reader.GetCurrentBsonType();
            switch (type)
            {
                case BsonType.String:
                    return StringSerializer.Deserialize<DateTime>(context.Reader.ReadString());
                default:
                    throw new NotSupportedException(Invariant($"Cannot convert a {type} to a {nameof(DateTime)}."));
            }
        }
    }

    /// <summary>
    /// Custom <see cref="Nullable{DateTime}"/> serializer to do the right thing.
    /// </summary>
    public class NaosBsonNullableDateTimeSerializer : SerializerBase<DateTime?>
    {
        private static readonly IStringSerializeAndDeserialize StringSerializer = new NaosDateTimeStringSerializer();

        /// <inheritdoc cref="SerializerBase{T}" />
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime? value)
        {
            new { context }.Must().NotBeNull().OrThrow();

            var stringValue = StringSerializer.SerializeToString(value);

            if (stringValue == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                context.Writer.WriteString(stringValue);
            }
        }

        /// <inheritdoc cref="SerializerBase{T}" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Is checked.")]
        public override DateTime? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            new { context, context.Reader }.Must().NotBeNull().OrThrow();

            if (context.Reader.State != BsonReaderState.Type && context.Reader.CurrentBsonType == BsonType.Null)
            {
                return null;
            }

            var type = context.Reader.GetCurrentBsonType();
            switch (type)
            {
                case BsonType.Null:
                    return null;
                case BsonType.String:
                    return StringSerializer.Deserialize<DateTime?>(context.Reader.ReadString());
                default:
                    throw new NotSupportedException(Invariant($"Cannot convert a {type} to a {nameof(Nullable<DateTime>)}."));
            }
        }
    }
}