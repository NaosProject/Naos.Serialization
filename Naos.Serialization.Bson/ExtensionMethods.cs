// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Options;
    using MongoDB.Bson.Serialization.Serializers;

    using OBeautifulCode.Reflection;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Extension methods for use with Bson serialization.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Sets a serializer to serialize and deserialize an enumeration as a string.
        /// </summary>
        /// <param name="map">Member the extension is on.</param>
        /// <returns>Updated <see cref="BsonMemberMap"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static BsonMemberMap SetEnumStringSerializer(this BsonMemberMap map)
        {
            new { map }.Must().NotBeNull().OrThrowFirstFailure();

            map.MemberType.IsEnum.Named("memberTypeIsEnumeration").Must().BeTrue().OrThrowFirstFailure();

            var serializer = MakeEnumStringSerializerFromType(map.MemberType);

            return map.SetSerializer(serializer);
        }

        /// <summary>
        /// Sets a serializer to serialize and deserialize an array (will automatically setup enumeration serialization).
        /// </summary>
        /// <param name="map">Member the extension is on.</param>
        /// <returns>Updated <see cref="BsonMemberMap"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static BsonMemberMap SetEnumArraySerializer(this BsonMemberMap map)
        {
            new { map }.Must().NotBeNull().OrThrowFirstFailure();

            map.MemberType.IsArray.Named("memberTypeIsArray").Must().BeTrue().OrThrowFirstFailure();

            var elementType = map.MemberType.GetElementType();
            new { elementType }.Must().NotBeNull().OrThrowFirstFailure();
            elementType.IsEnum.Named("itemTypeIsEnum").Must().BeTrue().OrThrowFirstFailure();

            var elementSerializer = MakeEnumStringSerializerFromType(elementType);
            var serializer = typeof(ArraySerializer<>).MakeGenericType(elementType).Construct(elementSerializer);

            return map.SetSerializer((IBsonSerializer)serializer);
        }

        /// <summary>
        /// Sets a serializer to serialize and deserialize a <see cref="DateTime"/> in and out using default string behavior.
        /// </summary>
        /// <param name="map">Member the extension is on.</param>
        /// <returns>Updated <see cref="BsonMemberMap"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static BsonMemberMap SetDateTimeStringSerializer(this BsonMemberMap map)
        {
            new { map }.Must().NotBeNull().OrThrowFirstFailure();

            new { map.MemberType }.Must().BeEqualTo(typeof(DateTime)).OrThrowFirstFailure();

            var serializer = new NaosBsonDateTimeSerializer();

            return map.SetSerializer(serializer);
        }

        /// <summary>
        /// Sets a serializer to serialize and deserialize a <see cref="Nullable{DateTime}"/> in and out using default string behavior.
        /// </summary>
        /// <param name="map">Member the extension is on.</param>
        /// <returns>Updated <see cref="BsonMemberMap"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static BsonMemberMap SetNullableDateTimeStringSerializer(this BsonMemberMap map)
        {
            new { map }.Must().NotBeNull().OrThrowFirstFailure();

            new { map.MemberType }.Must().BeEqualTo(typeof(DateTime?)).OrThrowFirstFailure();

            var serializer = new NaosBsonNullableDateTimeSerializer();

            return map.SetSerializer(serializer);
        }

        /// <summary>
        /// Sets a serializer to serialize and deserialize a dictionary.
        /// </summary>
        /// <param name="map">Member the extension is on.</param>
        /// <returns>Updated <see cref="BsonMemberMap"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static BsonMemberMap SetDictionarySerializer(this BsonMemberMap map)
        {
            new { map }.Must().NotBeNull().OrThrowFirstFailure();

            new { map.MemberType }.Must().NotBeNull().OrThrowFirstFailure();

            var arguments = map.MemberType.GetGenericArguments();
            arguments.Named("memberGenericArguments").Must().NotBeNull().OrThrowFirstFailure();
            arguments.Length.Named("memberGenericArgumentsCount").Must().BeEqualTo(2).OrThrowFirstFailure();

            var keyType = arguments[0];
            var valueType = arguments[1];

            NullNaosDictionarySerializer.IsSupportedUnboundedGenericDictionaryType(map.MemberType.GetGenericTypeDefinition()).Named("isSupportedDictionaryType").Must().BeTrue().OrThrow();

            var keySerializer = keyType.IsEnum || keyType == typeof(string) ? MakeEnumStringIfEnumOtherwiseObjectSerializer(keyType) : throw new BsonConfigurationException(Invariant($"Can only use a string or enumeration as a key in a dictionary or Mongo complains; member type: {map.MemberType}, key type: {keyType}, value type: {valueType}"));
            var valueSerializer = MakeEnumStringIfEnumOtherwiseObjectSerializer(valueType);

            var serializer = typeof(NaosDictionarySerializer<,,>).MakeGenericType(map.MemberType, keyType, valueType).Construct(DictionaryRepresentation.ArrayOfDocuments, keySerializer, valueSerializer);
            return map.SetSerializer((IBsonSerializer)serializer);
        }

        /// <summary>
        /// Gets the underlying type of a <see cref="MemberInfo"/>.
        /// </summary>
        /// <param name="member"><see cref="MemberInfo"/> to check.</param>
        /// <returns>Type of the member.</returns>
        internal static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default: throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
            }
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> to the <see cref="DateTimeKind"/> of <see cref="DateTimeKind.Unspecified"/>.
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/> to convert.</param>
        /// <returns>Converted <see cref="DateTime"/>.</returns>
        public static DateTime ToUnspecified(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, DateTimeKind.Unspecified);
        }

        private static IBsonSerializer MakeEnumStringIfEnumOtherwiseObjectSerializer(Type type)
        {
            return type.IsEnum
                       ? (IBsonSerializer)typeof(EnumSerializer<>).MakeGenericType(type).Construct(BsonType.String)
                       : new ObjectSerializer();
        }

        private static IBsonSerializer MakeEnumStringSerializerFromType(Type type)
        {
            type.IsEnum.Named("typeIsEnumeration").Must().BeTrue().OrThrowFirstFailure();

            return (IBsonSerializer)typeof(EnumSerializer<>).MakeGenericType(type).Construct(BsonType.String);
        }
    }
}