// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosBsonSerializer.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.Linq;
    using System.Reflection;

    using MongoDB.Bson;

    using Naos.Serialization.Domain;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Mongo BSON serializer with optional configuration type.
    /// </summary>
    public class NaosBsonSerializer : ISerializeAndDeserialize
    {
#pragma warning disable SA1401 // Fields should be private - I want these to be readonly fields...
        /// <summary>
        /// <see cref="Domain.SerializationKind" /> that was used in construction.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "I want a readonly field.")]
        protected readonly SerializationKind SerializationKind;
#pragma warning restore SA1401 // Fields should be private

#pragma warning disable SA1401 // Fields should be private - I want these to be readonly fields...
        /// <summary>
        /// <see cref="BsonConfigurationBase" /> that was used in construction.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "I want a readonly field.")]
        protected readonly Type BsonConfigurationType;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Initializes a new instance of the <see cref="NaosBsonSerializer"/> class.
        /// </summary>
        /// <param name="serializationKind">Optional kind of serialization to use; default is <see cref="Domain.SerializationKind.Default"/>.</param>
        /// <param name="configurationType">Optional <see cref="BsonConfigurationBase"/> implmentation to use; default is <see cref="NullBsonConfiguration"/>.</param>
        public NaosBsonSerializer(SerializationKind serializationKind = SerializationKind.Default, Type configurationType = null)
        {
            new { serializationKind }.Must().NotBeEqualTo(SerializationKind.Invalid).OrThrowFirstFailure();

            if (configurationType != null)
            {
                configurationType.IsSubclassOf(typeof(BsonConfigurationBase))
                    .Named(Invariant($"Configuration type - {configurationType.FullName} - must derive from {nameof(BsonConfigurationBase)}")).Must().BeTrue()
                    .OrThrowFirstFailure();

                configurationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).SingleOrDefault(_ => _.GetParameters().Length == 0)
                    .Named(Invariant($"{nameof(configurationType)} must contain a default constructor to use in {nameof(NaosBsonSerializer)}.")).Must().NotBeNull().OrThrowFirstFailure();
            }

            this.SerializationKind = serializationKind;
            this.BsonConfigurationType = configurationType ?? typeof(NullBsonConfiguration);
        }

        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public byte[] SerializeToBytes(object objectToSerialize)
        {
            BsonConfigurationManager.Configure(this.BsonConfigurationType);

            return NaosBsonSerializerHelper.SerializeToBytes(objectToSerialize);
        }

        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public T Deserialize<T>(byte[] serializedBytes)
        {
            BsonConfigurationManager.Configure(this.BsonConfigurationType);

            return NaosBsonSerializerHelper.Deserialize<T>(serializedBytes);
        }

        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public object Deserialize(byte[] serializedBytes, Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            BsonConfigurationManager.Configure(this.BsonConfigurationType);

            return NaosBsonSerializerHelper.Deserialize(serializedBytes, type);
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public string SerializeToString(object objectToSerialize)
        {
            BsonConfigurationManager.Configure(this.BsonConfigurationType);

            var document = NaosBsonSerializerHelper.SerializeToDocument(objectToSerialize);
            var json = document.ToJson();
            return json;
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public T Deserialize<T>(string serializedString)
        {
            BsonConfigurationManager.Configure(this.BsonConfigurationType);

            var document = serializedString.ToBsonDocument();
            return NaosBsonSerializerHelper.DeserializeFromDocument<T>(document);
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public object Deserialize(string serializedString, Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            BsonConfigurationManager.Configure(this.BsonConfigurationType);

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
        /// <param name="serializationKind">Type of serialization to use.</param>
        public NaosBsonSerializer(SerializationKind serializationKind = SerializationKind.Default)
            : base(serializationKind, typeof(TBsonConfiguration))
        {
        }
    }
}
