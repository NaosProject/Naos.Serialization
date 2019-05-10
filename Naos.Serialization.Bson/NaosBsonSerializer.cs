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
            if (objectType != null &&
                this.unregisteredTypeEncounteredStrategy == UnregisteredTypeEncounteredStrategy.Throw &&
                !this.bsonConfiguration.RegisteredTypeToDetailsMap.ContainsKey(objectType))
            {
                throw new UnregisteredTypeAttemptException(Invariant($"Attempted to perform '{nameof(this.SerializeToBytes)}({nameof(objectToSerialize)})' on unregistered type '{objectType.FullName}'"), objectType);
            }

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
            if (this.unregisteredTypeEncounteredStrategy == UnregisteredTypeEncounteredStrategy.Throw &&
                !this.bsonConfiguration.RegisteredTypeToDetailsMap.ContainsKey(objectType))
            {
                throw new UnregisteredTypeAttemptException(Invariant($"Attempted to perform '{nameof(this.Deserialize)}<T>({nameof(serializedBytes)})' on unregistered type '{objectType.FullName}'"), objectType);
            }

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

            if (this.unregisteredTypeEncounteredStrategy == UnregisteredTypeEncounteredStrategy.Throw &&
                !this.bsonConfiguration.RegisteredTypeToDetailsMap.ContainsKey(type))
            {
                throw new UnregisteredTypeAttemptException(Invariant($"Attempted to perform '{nameof(this.Deserialize)}({nameof(serializedBytes)}, {nameof(type)})' on unregistered type '{type.FullName}'"), type);
            }

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
            if (objectType == typeof(string))
            {
                throw new NotSupportedException("String is not supported as a type for this serializer.");
            }

            if (objectType != null &&
                this.unregisteredTypeEncounteredStrategy == UnregisteredTypeEncounteredStrategy.Throw &&
                !this.bsonConfiguration.RegisteredTypeToDetailsMap.ContainsKey(objectType))
            {
                throw new UnregisteredTypeAttemptException(Invariant($"Attempted to perform '{nameof(this.SerializeToString)}({nameof(objectToSerialize)})' on unregistered type '{objectType.FullName}'"), objectType);
            }

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
            if (this.unregisteredTypeEncounteredStrategy == UnregisteredTypeEncounteredStrategy.Throw &&
                !this.bsonConfiguration.RegisteredTypeToDetailsMap.ContainsKey(objectType))
            {
                throw new UnregisteredTypeAttemptException(Invariant($"Attempted to perform '{nameof(this.Deserialize)}<T>({nameof(serializedString)})' on unregistered type '{objectType.FullName}'"), objectType);
            }

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

            if (this.unregisteredTypeEncounteredStrategy == UnregisteredTypeEncounteredStrategy.Throw &&
                !this.bsonConfiguration.RegisteredTypeToDetailsMap.ContainsKey(type))
            {
                throw new UnregisteredTypeAttemptException(Invariant($"Attempted to perform '{nameof(this.Deserialize)}({nameof(serializedString)}, {nameof(type)})' on unregistered type '{type.FullName}'"), type);
            }

            if (serializedString == SerializationConfigurationBase.NullSerializedStringValue)
            {
                return null;
            }

            var document = serializedString.ToBsonDocument();
            return NaosBsonSerializerHelper.DeserializeFromDocument(document, type);
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
