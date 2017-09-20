// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosBsonSerializer.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;

    using MongoDB.Bson;

    using Naos.Serialization.Domain;

    using Spritely.Recipes;

    /// <summary>
    /// Mongo BSON serializer.
    /// </summary>
    /// <typeparam name="TBsonConfiguration">Type of <see cref="BsonConfigurationBase"/> to use (can use <see cref="NullBsonConfiguration"/> if none needed).</typeparam>
    public sealed class NaosBsonSerializer<TBsonConfiguration> : IBinarySerializeAndDeserialize, IStringSerializeAndDeserialize
        where TBsonConfiguration : BsonConfigurationBase, new()
    {
        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public byte[] SerializeToBytes(object objectToSerialize)
        {
            BsonConfigurationManager.Configure<TBsonConfiguration>();

            return NaosBsonSerializerHelper.SerializeToBytes(objectToSerialize);
        }

        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public T Deserialize<T>(byte[] serializedBytes)
        {
            BsonConfigurationManager.Configure<TBsonConfiguration>();

            return NaosBsonSerializerHelper.Deserialize<T>(serializedBytes);
        }

        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public object Deserialize(byte[] serializedBytes, Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            BsonConfigurationManager.Configure<TBsonConfiguration>();

            return NaosBsonSerializerHelper.Deserialize(serializedBytes, type);
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public string SerializeToString(object objectToSerialize)
        {
            BsonConfigurationManager.Configure<TBsonConfiguration>();

            var document = NaosBsonSerializerHelper.SerializeToDocument(objectToSerialize);
            var json = document.ToJson();
            return json;
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public T Deserialize<T>(string serializedString)
        {
            BsonConfigurationManager.Configure<TBsonConfiguration>();

            var document = serializedString.ToBsonDocument();
            return NaosBsonSerializerHelper.DeserializeFromDocument<T>(document);
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public object Deserialize(string serializedString, Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            BsonConfigurationManager.Configure<TBsonConfiguration>();

            var document = serializedString.ToBsonDocument();
            return NaosBsonSerializerHelper.DeserializeFromDocument(document, type);
        }
    }
}
