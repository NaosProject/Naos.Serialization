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
    using System.Linq;

    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Options;
    using MongoDB.Bson.Serialization.Serializers;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Custom dictionary serializer to do the right thing.
    /// Supports:
    /// - <see cref="IDictionary{TKey, TValue}"/>
    /// - <see cref="Dictionary{TKey, TValue}"/>
    /// - <see cref="IReadOnlyDictionary{TKey, TValue}"/>
    /// - <see cref="ReadOnlyDictionary{TKey, TValue}"/>
    /// - <see cref="ConcurrentDictionary{TKey, TValue}"/>
    /// </summary>
    /// <typeparam name="TDict">The type of the dictionary.</typeparam>
    /// <typeparam name="TKey">The type of the key of the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the value of the dictionary.</typeparam>
    // ReSharper disable once InheritdocConsiderUsage
    public class NaosDictionarySerializer<TDict, TKey, TValue> : SerializerBase<TDict>
        where TDict : class, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Maps a supported dictionary type to a func that creates that type from a dictionary returned by the underlying serializer.
        /// </summary>
        protected static readonly Dictionary<Type, Func<Dictionary<TKey, TValue>, TDict>>
            DeserializationConverterFuncBySerializedType = new Dictionary<Type, Func<Dictionary<TKey, TValue>, TDict>>
            {
                { typeof(Dictionary<TKey, TValue>), dict => dict as TDict },
                { typeof(IDictionary<TKey, TValue>), dict => dict as TDict },
                { typeof(ReadOnlyDictionary<TKey, TValue>), dict => new ReadOnlyDictionary<TKey, TValue>(dict) as TDict },
                { typeof(IReadOnlyDictionary<TKey, TValue>), dict => new ReadOnlyDictionary<TKey, TValue>(dict) as TDict },
                { typeof(ConcurrentDictionary<TKey, TValue>), dict => new ConcurrentDictionary<TKey, TValue>(dict) as TDict },
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
            DeserializationConverterFuncBySerializedType.ContainsKey(typeof(TDict)).Named(Invariant($"{typeof(TDict)}-mustBeSupportedDictionaryType")).Must().BeTrue().OrThrow();

            this.underlyingSerializer = new DictionaryInterfaceImplementerSerializer<Dictionary<TKey, TValue>>(dictionaryRepresentation, keySerializer, valueSerializer);
        }

        /// <inheritdoc />
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TDict value)
        {
            this.underlyingSerializer.Serialize(context, args, ((IDictionary<TKey, TValue>)value).ToDictionary(_ => _.Key, _ => _.Value));
        }

        /// <inheritdoc />
        public override TDict Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var dictionary = this.underlyingSerializer.Deserialize(context, args);
            var result = DeserializationConverterFuncBySerializedType[typeof(TDict)](dictionary);
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
            new { type }.Must().NotBeNull().OrThrow();

            var result = SupportedUnboundedGenericDictionaryTypes.Contains(type);
            return result;
        }
    }
}