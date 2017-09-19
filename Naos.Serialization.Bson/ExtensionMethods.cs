// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System.Collections.Generic;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Options;
    using MongoDB.Bson.Serialization.Serializers;

    using OBeautifulCode.Reflection;

    using Spritely.Recipes;

    /// <summary>
    /// Extension methods for use with Bson serialization.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Sets a serializer to serialize and deserialize an enumeration as a string.
        /// </summary>
        /// <typeparam name="T">Type of the enumeration.</typeparam>
        /// <param name="map">Member the extension is on.</param>
        /// <returns>Updated <see cref="BsonMemberMap"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static BsonMemberMap SetEnumStringSerializer<T>(this BsonMemberMap map)
            where T : struct
        {
            new { map }.Must().NotBeNull().OrThrowFirstFailure();

            map.MemberType.IsEnum.Named("memberTypeIsEnumeration").Must().BeTrue().OrThrowFirstFailure();

            return map.SetSerializer(new EnumSerializer<T>(BsonType.String));
        }

        /// <summary>
        /// Sets a serializer to serialize and deserialize an array (will automatically setup enumeration serialization.
        /// </summary>
        /// <typeparam name="T">Type of the array member.</typeparam>
        /// <param name="map">Member the extension is on.</param>
        /// <param name="elementSerializer">Optional serializer to use for the elements.</param>
        /// <returns>Updated <see cref="BsonMemberMap"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static BsonMemberMap SetArraySerializer<T>(this BsonMemberMap map, IBsonSerializer<T> elementSerializer = null)
        {
            new { map }.Must().NotBeNull().OrThrowFirstFailure();

            var type = typeof(T);
            var typeArrayType = type.MakeArrayType();
            map.MemberType.Named("memberTypeIsArrayOfSpecifiedType").Must().BeEqualTo(typeArrayType).OrThrowFirstFailure();

            var elementSerializerLocal = elementSerializer ?? MakeEnumStringIfEnumOtherwiseObjectSerializer<T>();
            var serializer = new ArraySerializer<T>(elementSerializerLocal);

            return map.SetSerializer(serializer);
        }

        /// <summary>
        /// Sets a serializer to serialize and deserialize a dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of the key of the dictionary.</typeparam>
        /// <typeparam name="TValue">Type of the value of the dictionary.</typeparam>
        /// <param name="map">Member the extension is on.</param>
        /// <returns>Updated <see cref="BsonMemberMap"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static BsonMemberMap SetDictionarySerializer<TKey, TValue>(this BsonMemberMap map)
        {
            new { map }.Must().NotBeNull().OrThrowFirstFailure();

            new { map.MemberType }.Must().NotBeNull().OrThrowFirstFailure();

            map.MemberType.IsSubclassOf(typeof(IDictionary<TKey, TValue>)).Named("memberMustBeDictionaryImplementationOfSpecifiedKeyAndValueTypes").Must().BeTrue().OrThrowFirstFailure();

            var keySerializer = MakeEnumStringIfEnumOtherwiseObjectSerializer<TKey>();
            var valueSerializer = MakeEnumStringIfEnumOtherwiseObjectSerializer<TValue>();

            var serializer = new DictionaryInterfaceImplementerSerializer<Dictionary<TKey, TValue>>(DictionaryRepresentation.ArrayOfDocuments, keySerializer, valueSerializer);

            return map.SetSerializer(serializer);
        }

        private static IBsonSerializer<T> MakeEnumStringIfEnumOtherwiseObjectSerializer<T>()
        {
            return typeof(T).IsEnum
                       ? (IBsonSerializer<T>)typeof(EnumSerializer<>).MakeGenericType(typeof(T)).Construct(BsonType.String)
                       : (IBsonSerializer<T>)new ObjectSerializer();
        }
    }
}