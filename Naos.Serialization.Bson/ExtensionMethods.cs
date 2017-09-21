// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.Reflection;

    using MongoDB.Bson.Serialization;

    using Spritely.Recipes;

    /// <summary>
    /// Extension methods for use with Bson serialization.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Sets the automatically-choosen serializer.
        /// </summary>
        /// <param name="map">Member the extension is on.</param>
        /// <returns>Updated <see cref="BsonMemberMap"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static BsonMemberMap SetNaosSerializer(this BsonMemberMap map)
        {
            new { map }.Must().NotBeNull().OrThrowFirstFailure();

            var serializer = BsonConfigurationBase.GetSerializer(map.MemberType);

            return map.SetSerializer(serializer);
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
    }
}