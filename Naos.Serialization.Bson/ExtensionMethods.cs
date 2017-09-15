// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;

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

            return map.SetSerializer(new EnumSerializer<T>(BsonType.String));
        }
    }
}