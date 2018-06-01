// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosDictionarySerializer.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Options;
    using MongoDB.Bson.Serialization.Serializers;

    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Custom dictionary serializer to do the right thing.
    /// Supports:
    /// - <see cref="IDictionary{TKey, TValue}"/>
    /// - <see cref="Dictionary{TKey, TValue}"/>
    /// - <see cref="IReadOnlyDictionary{TKey, TValue}" />
    /// - <see cref="ReadOnlyDictionary{TKey, TValue}" />
    /// - <see cref="ConcurrentDictionary{TKey, TValue}" />
    /// </summary>
    /// <typeparam name="TDictionary">The type of the dictionary.</typeparam>
    /// <typeparam name="TKey">The type of the key of the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the value of the dictionary.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "All of these generic parameters are required.")]

    // ReSharper disable once InheritdocConsiderUsage
    public class NaosDictionarySerializer<TDictionary, TKey, TValue> : SerializerBase<TDictionary>
        where TDictionary : class, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Converts a dictionary returned by the underlying serializer into the type of dictionary of this serializer.
        /// </summary>
        /// <param name="dictionary">The dictionary returned by the underlying serializer.</param>
        /// <returns>
        /// The type of the dictionary to return.
        /// </returns>
        protected delegate TDictionary ConvertToUnderlyingSerializerType(Dictionary<TKey, TValue> dictionary);

        /// <summary>
        /// Maps a supported dictionary type to a func that creates that type from a dictionary returned by the underlying serializer.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This is already an immutable type.")]
        protected static readonly IReadOnlyDictionary<Type, ConvertToUnderlyingSerializerType>
            DeserializationConverterFuncBySerializedType = new Dictionary<Type, ConvertToUnderlyingSerializerType>
            {
                { typeof(Dictionary<TKey, TValue>), dict => dict as TDictionary },
                { typeof(IDictionary<TKey, TValue>), dict => dict as TDictionary },
                { typeof(ReadOnlyDictionary<TKey, TValue>), dict => new ReadOnlyDictionary<TKey, TValue>(dict) as TDictionary },
                { typeof(IReadOnlyDictionary<TKey, TValue>), dict => new ReadOnlyDictionary<TKey, TValue>(dict) as TDictionary },
                { typeof(ConcurrentDictionary<TKey, TValue>), dict => new ConcurrentDictionary<TKey, TValue>(dict) as TDictionary },
            };

        private readonly DictionaryInterfaceImplementerSerializer<Dictionary<TKey, TValue>> underlyingSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NaosDictionarySerializer{T, TKey,TValue}"/> class.
        /// </summary>
        /// <param name="dictionaryRepresentation">The dictionary representation.</param>
        /// <param name="keySerializer">The key serializer.</param>
        /// <param name="valueSerializer">The value serializer.</param>
        public NaosDictionarySerializer(DictionaryRepresentation dictionaryRepresentation, IBsonSerializer keySerializer, IBsonSerializer valueSerializer)
        {
            DeserializationConverterFuncBySerializedType.ContainsKey(typeof(TDictionary)).Named(Invariant($"{typeof(TDictionary)}-mustBeSupportedDictionaryType")).Must().BeTrue();

            this.underlyingSerializer = new DictionaryInterfaceImplementerSerializer<Dictionary<TKey, TValue>>(dictionaryRepresentation, keySerializer, valueSerializer);
        }

        /// <inheritdoc />
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TDictionary value)
        {
            new { context }.Must().NotBeNull();

            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }

            var valueAsDictionary = value as Dictionary<TKey, TValue>;
            if (valueAsDictionary != null)
            {
                this.underlyingSerializer.Serialize(context, args, valueAsDictionary);
            }
            else
            {
                this.underlyingSerializer.Serialize(context, args, ((IDictionary<TKey, TValue>)value).ToDictionary(_ => _.Key, _ => _.Value));
            }
        }

        /// <inheritdoc />
        public override TDictionary Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            new { context }.Must().NotBeNull();

            if (context.Reader.State != BsonReaderState.Type && context.Reader.CurrentBsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return null;
            }

            var dictionary = this.underlyingSerializer.Deserialize(context, args);
            var result = DeserializationConverterFuncBySerializedType[typeof(TDictionary)](dictionary);
            return result;
        }
    }

    /// <summary>
    /// A dictionary serializer that does nothing.
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public class NullNaosDictionarySerializer : NaosDictionarySerializer<IDictionary<string, string>, string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullNaosDictionarySerializer"/> class.
        /// </summary>
        /// <param name="dictionaryRepresentation">The dictionary representation.</param>
        /// <param name="keySerializer">The key serializer.</param>
        /// <param name="valueSerializer">The value serializer.</param>
        // ReSharper disable once InheritdocConsiderUsage
        public NullNaosDictionarySerializer(DictionaryRepresentation dictionaryRepresentation, IBsonSerializer keySerializer, IBsonSerializer valueSerializer)
            : base(dictionaryRepresentation, keySerializer, valueSerializer)
        {
            throw new NotSupportedException("The null dictionary serializer is not intended for use.");
        }

        /// <summary>
        /// Gets the supported unbounded generic dictionary types.
        /// </summary>
        public static IReadOnlyCollection<Type> SupportedUnboundedGenericDictionaryTypes =>
            DeserializationConverterFuncBySerializedType.Keys.Select(_ => _.GetGenericTypeDefinition()).ToList();

        /// <summary>
        /// Determines if the specified type is a supported unbounded generic dictionary type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the specified type is supported; otherwise, false.</returns>
        public static bool IsSupportedUnboundedGenericDictionaryType(Type type)
        {
            new { type }.Must().NotBeNull();

            var result = SupportedUnboundedGenericDictionaryTypes.Contains(type);
            return result;
        }
    }
}