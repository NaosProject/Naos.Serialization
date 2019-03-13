// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosCollectionSerializer.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;

    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Custom collection serializer to do the right thing.
    /// Supports:
    /// - <see cref="ReadOnlyCollection{TElement}"/>
    /// - <see cref="ICollection{TElement}"/>
    /// - <see cref="IList{TElement}"/>
    /// - <see cref="IReadOnlyList{TElement}"/>
    /// - <see cref="IReadOnlyCollection{TElement}"/>
    /// - <see cref="List{TElement}"/>
    /// - <see cref="Collection{TElement}"/>
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection.</typeparam>
    /// <typeparam name="TElement">The type of the elements in the collection.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "All of these generic parameters are required.")]

    // ReSharper disable once InheritdocConsiderUsage
    public class NaosCollectionSerializer<TCollection, TElement> : SerializerBase<TCollection>
        where TCollection : class, IEnumerable<TElement>
    {
        /// <summary>
        /// Converts a read-only collection returned by the underlying serializer into the type of collection of this serializer.
        /// </summary>
        /// <param name="collection">The read-only collection returned by the underlying serializer.</param>
        /// <returns>
        /// The type of the collection to return.
        /// </returns>
        protected delegate TCollection ConvertToUnderlyingSerializerType(ReadOnlyCollection<TElement> collection);

        /// <summary>
        /// Maps a supported collection type to a func that creates that type from a collection returned by the underlying serializer.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This is already an immutable type.")]
        protected static readonly IReadOnlyDictionary<Type, ConvertToUnderlyingSerializerType>
            DeserializationConverterFuncBySerializedType = new Dictionary<Type, ConvertToUnderlyingSerializerType>
            {
                { typeof(ReadOnlyCollection<TElement>), collection => collection as TCollection },
                { typeof(ICollection<TElement>), collection => collection.ToList() as TCollection },
                { typeof(IList<TElement>), collection => collection.ToList() as TCollection },
                { typeof(IReadOnlyList<TElement>), collection => collection.ToList() as TCollection },
                { typeof(IReadOnlyCollection<TElement>), collection => collection.ToList() as TCollection },
                { typeof(List<TElement>), collection => collection.ToList() as TCollection },
                { typeof(Collection<TElement>), collection => new Collection<TElement>(collection) as TCollection },
            };

        private readonly ReadOnlyCollectionSerializer<TElement> underlyingSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NaosCollectionSerializer{TCollection, TElement}"/> class.
        /// </summary>
        /// <param name="elementSerializer">The element serializer.</param>
        public NaosCollectionSerializer(IBsonSerializer<TElement> elementSerializer)
        {
            DeserializationConverterFuncBySerializedType.ContainsKey(typeof(TCollection)).Named(Invariant($"{typeof(TCollection)}-mustBeSupportedCollectionType")).Must().BeTrue();

            this.underlyingSerializer = elementSerializer == null
                ? new ReadOnlyCollectionSerializer<TElement>()
                : new ReadOnlyCollectionSerializer<TElement>(elementSerializer);
        }

        /// <inheritdoc />
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TCollection value)
        {
            new { context }.Must().NotBeNull();

            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }

            var valueAsReadOnlyCollection = value as ReadOnlyCollection<TElement>;
            if (valueAsReadOnlyCollection != null)
            {
                this.underlyingSerializer.Serialize(context, args, valueAsReadOnlyCollection);
            }
            else
            {
                if (value is IList<TElement> valueAsIList)
                {
                    this.underlyingSerializer.Serialize(context, args, new ReadOnlyCollection<TElement>(valueAsIList));
                }
                else
                {
                    this.underlyingSerializer.Serialize(context, args, new ReadOnlyCollection<TElement>(value.ToList()));
                }
            }
        }

        /// <inheritdoc />
        public override TCollection Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            new { context }.Must().NotBeNull();

            if (context.Reader.State != BsonReaderState.Type && context.Reader.CurrentBsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return null;
            }

            var collection = this.underlyingSerializer.Deserialize(context, args);
            var result = DeserializationConverterFuncBySerializedType[typeof(TCollection)](collection);
            return result;
        }
    }

    /// <summary>
    /// A collection serializer that does nothing.
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public class NullNaosCollectionSerializer : NaosCollectionSerializer<ICollection<string>, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullNaosCollectionSerializer"/> class.
        /// </summary>
        /// <param name="elementSerializer">The element serializer.</param>
        // ReSharper disable once InheritdocConsiderUsage
        public NullNaosCollectionSerializer(IBsonSerializer<string> elementSerializer)
            : base(elementSerializer)
        {
            throw new NotSupportedException("The null collection serializer is not intended for use.");
        }

        /// <summary>
        /// Gets the supported unbounded generic collection types.
        /// </summary>
        public static IReadOnlyCollection<Type> SupportedUnboundedGenericCollectionTypes =>
            DeserializationConverterFuncBySerializedType.Keys.Select(_ => _.GetGenericTypeDefinition()).ToList();

        /// <summary>
        /// Determines if the specified type is a supported unbounded generic collection type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the specified type is supported; otherwise, false.</returns>
        public static bool IsSupportedUnboundedGenericCollectionType(Type type)
        {
            new { type }.Must().NotBeNull();

            var result = SupportedUnboundedGenericCollectionTypes.Contains(type);
            return result;
        }
    }
}