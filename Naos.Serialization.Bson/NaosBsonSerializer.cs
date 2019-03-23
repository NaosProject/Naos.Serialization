// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosBsonSerializer.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using MongoDB.Bson;

    using Naos.Serialization.Domain;

    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Mongo BSON serializer with optional configuration type.
    /// </summary>
    public class NaosBsonSerializer : ISerializeAndDeserialize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NaosBsonSerializer"/> class.
        /// </summary>
        /// <param name="configurationType">Optional <see cref="BsonConfigurationBase"/> implementation to use; default is <see cref="NullBsonConfiguration"/>.</param>
        public NaosBsonSerializer(
            Type configurationType = null)
        {
            if (configurationType != null)
            {
                configurationType.IsSubclassOf(typeof(BsonConfigurationBase)).Named(
                    Invariant($"Configuration type - {configurationType.FullName} - must derive from {nameof(BsonConfigurationBase)}.")).Must().BeTrue();

                configurationType.HasParameterlessConstructor().Named(
                    Invariant($"{nameof(configurationType)} must contain a default constructor to use in {nameof(NaosBsonSerializer)}.")).Must().BeTrue();
            }

            this.ConfigurationType = configurationType ?? typeof(NullBsonConfiguration);

            SerializationConfigurationManager.Configure(this.ConfigurationType);
        }

        /// <inheritdoc />
        public Type ConfigurationType { get; private set; }

        /// <inheritdoc />
        public byte[] SerializeToBytes(object objectToSerialize)
        {
            return NaosBsonSerializerHelper.SerializeToBytes(objectToSerialize);
        }

        /// <inheritdoc />
        public T Deserialize<T>(byte[] serializedBytes)
        {
            return NaosBsonSerializerHelper.Deserialize<T>(serializedBytes);
        }

        /// <inheritdoc />
        public object Deserialize(byte[] serializedBytes, Type type)
        {
            new { type }.Must().NotBeNull();

            return NaosBsonSerializerHelper.Deserialize(serializedBytes, type);
        }

        /// <inheritdoc />
        public string SerializeToString(object objectToSerialize)
        {
            var document = NaosBsonSerializerHelper.SerializeToDocument(objectToSerialize);
            var json = document.ToJson();
            return json;
        }

        /// <inheritdoc />
        public T Deserialize<T>(string serializedString)
        {
            var document = serializedString.ToBsonDocument();
            return NaosBsonSerializerHelper.DeserializeFromDocument<T>(document);
        }

        /// <inheritdoc />
        public object Deserialize(string serializedString, Type type)
        {
            new { type }.Must().NotBeNull();

            var document = serializedString.ToBsonDocument();
            return NaosBsonSerializerHelper.DeserializeFromDocument(document, type);
        }
    }

    /// <summary>
    /// Mongo BSON serializer with generic configuration type.
    /// </summary>
    /// <typeparam name="TBsonConfiguration">Type of <see cref="BsonConfigurationBase"/> to use (can use <see cref="NullBsonConfiguration"/> if none needed).</typeparam>
    public sealed class NaosBsonSerializer<TBsonConfiguration> : NaosBsonSerializer
        where TBsonConfiguration : BsonConfigurationBase, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NaosBsonSerializer{TBsonConfiguration}"/> class.
        /// </summary>
        public NaosBsonSerializer()
            : base(typeof(TBsonConfiguration))
        {
        }
    }
}
