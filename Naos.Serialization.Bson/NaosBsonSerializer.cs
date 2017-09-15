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
    /// <typeparam name="TBsonClassManager">Type of <see cref="BsonClassMapperBase"/> to use (can use <see cref="NullBsonClassMapper"/> if none needed).</typeparam>
    public class NaosBsonSerializer<TBsonClassManager> : ISerializeAndDeserializeThings
        where TBsonClassManager : BsonClassMapperBase, new()
    {
        /// <inheritdoc cref="ISerializeAndDeserializeThings"/>
        public byte[] Serialize(object objectToSerialize)
        {
            BsonClassMapManager.Instance<TBsonClassManager>().RegisterClassMaps();

            return NaosBsonSerializerHelper.Serialize(objectToSerialize);
        }

        /// <inheritdoc cref="ISerializeAndDeserializeThings"/>
        public T Deserialize<T>(byte[] serializedBytes)
        {
            BsonClassMapManager.Instance<TBsonClassManager>().RegisterClassMaps();

            return NaosBsonSerializerHelper.Deserialize<T>(serializedBytes);
        }

        /// <inheritdoc cref="ISerializeAndDeserializeThings"/>
        public object Deserialize(byte[] serializedBytes, Type type)
        {
            BsonClassMapManager.Instance<TBsonClassManager>().RegisterClassMaps();

            return NaosBsonSerializerHelper.Deserialize(serializedBytes, type);
        }
    }
}
