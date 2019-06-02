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
    public class NaosBsonSerializer : SerializerBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Keeping for easy extension.")]
        private readonly BsonConfigurationBase bsonConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="NaosBsonSerializer"/> class.
        /// </summary>
        /// <param name="configurationType">Optional <see cref="BsonConfigurationBase"/> implementation to use; default is <see cref="NullBsonConfiguration"/>.</param>
        /// <param name="unregisteredTypeEncounteredStrategy">Optional strategy of what to do when encountering a type that has never been registered; DEFAULT is <see cref="UnregisteredTypeEncounteredStrategy.Throw" />.</param>
        public NaosBsonSerializer(
            Type configurationType = null,
            UnregisteredTypeEncounteredStrategy unregisteredTypeEncounteredStrategy = UnregisteredTypeEncounteredStrategy.Default)
            : base(configurationType ?? typeof(NullBsonConfiguration), unregisteredTypeEncounteredStrategy)
        {
            if (configurationType != null)
            {
                configurationType.IsSubclassOf(typeof(BsonConfigurationBase)).Named(
                    Invariant($"Configuration type - {configurationType.FullName} - must derive from {nameof(BsonConfigurationBase)}.")).Must().BeTrue();

                configurationType.HasParameterlessConstructor().Named(
                    Invariant($"{nameof(configurationType)} must contain a default constructor to use in {nameof(NaosBsonSerializer)}.")).Must().BeTrue();
            }

            this.bsonConfiguration = (BsonConfigurationBase)this.configuration;
        }

        /// <inheritdoc />
        public override SerializationKind Kind => SerializationKind.Bson;

        /// <inheritdoc />
        public override byte[] SerializeToBytes(object objectToSerialize)
        {
            var objectType = objectToSerialize?.GetType();

            this.InternalBsonThrowOnUnregisteredTypeIfAppropriate(objectType);

            if (objectToSerialize == null)
            {
                return null;
            }

            return NaosBsonSerializerHelper.SerializeToBytes(objectToSerialize);
        }

        /// <inheritdoc />
        public override T Deserialize<T>(byte[] serializedBytes)
        {
            var objectType = typeof(T);

            this.InternalBsonThrowOnUnregisteredTypeIfAppropriate(objectType);

            if (serializedBytes == null)
            {
                return default(T);
            }

            return NaosBsonSerializerHelper.Deserialize<T>(serializedBytes);
        }

        /// <inheritdoc />
        public override object Deserialize(byte[] serializedBytes, Type type)
        {
            new { type }.Must().NotBeNull();

            this.InternalBsonThrowOnUnregisteredTypeIfAppropriate(type);

            if (serializedBytes == null)
            {
                return null;
            }

            return NaosBsonSerializerHelper.Deserialize(serializedBytes, type);
        }

        /// <inheritdoc />
        public override string SerializeToString(object objectToSerialize)
        {
            var objectType = objectToSerialize?.GetType();

            this.InternalBsonThrowOnUnregisteredTypeIfAppropriate(objectType);

            if (objectToSerialize == null)
            {
                return SerializationConfigurationBase.NullSerializedStringValue;
            }

            var document = NaosBsonSerializerHelper.SerializeToDocument(objectToSerialize);
            var json = document.ToJson();
            return json;
        }

        /// <inheritdoc />
        public override T Deserialize<T>(string serializedString)
        {
            var objectType = typeof(T);

            this.InternalBsonThrowOnUnregisteredTypeIfAppropriate(objectType);

            if (serializedString == SerializationConfigurationBase.NullSerializedStringValue)
            {
                return default(T);
            }

            var document = serializedString.ToBsonDocument();
            return NaosBsonSerializerHelper.DeserializeFromDocument<T>(document);
        }

        /// <inheritdoc />
        public override object Deserialize(string serializedString, Type type)
        {
            new { type }.Must().NotBeNull();

            this.InternalBsonThrowOnUnregisteredTypeIfAppropriate(type);

            if (serializedString == SerializationConfigurationBase.NullSerializedStringValue)
            {
                return null;
            }

            var document = serializedString.ToBsonDocument();
            return NaosBsonSerializerHelper.DeserializeFromDocument(document, type);
        }

        private void InternalBsonThrowOnUnregisteredTypeIfAppropriate(Type objectType)
        {
            if (objectType == typeof(string))
            {
                throw new NotSupportedException("String is not supported as a root type by the underlying BSON Serializer.");
            }

            this.ThrowOnUnregisteredTypeIfAppropriate(objectType);
        }
    }

    /// <inheritdoc />
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
