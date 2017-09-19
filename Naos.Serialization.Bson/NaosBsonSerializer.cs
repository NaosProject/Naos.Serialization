// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosBsonSerializer.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;

    using Naos.Serialization.Domain;

    /// <summary>
    /// Mongo BSON serializer.
    /// </summary>
    /// <typeparam name="TBsonConfiguration">Type of <see cref="BsonConfigurationBase"/> to use (can use <see cref="NullBsonConfiguration"/> if none needed).</typeparam>
    public sealed class NaosBsonSerializer<TBsonConfiguration> : IBinarySerializeAndDeserialize
        where TBsonConfiguration : BsonConfigurationBase, new()
    {
        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public byte[] Serialize(object objectToSerialize)
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
            BsonConfigurationManager.Configure<TBsonConfiguration>();

            return NaosBsonSerializerHelper.Deserialize(serializedBytes, type);
        }
    }
}
