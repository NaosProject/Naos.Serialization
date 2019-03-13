// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosBsonConventions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System.Linq;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Conventions;
    using MongoDB.Bson.Serialization.Serializers;

    /// <summary>
    /// Helper class for specifying broader conventions.
    /// </summary>
    public static class NaosBsonConventions
    {
        /// <summary>
        /// Name of the convention to store enumerations as strings.
        /// </summary>
        public const string EnumAsStringConventionName = "EnumAsStringConvention";

        /// <summary>
        /// Registers the convention pack to serialize most enumerations as strings but sometimes (like in dictionaries) you'll need to use the <see cref="EnumSerializer{TEnum}"/> directly.
        /// </summary>
        public static void RegisterEnumAsStringConventionIfNotRegistered()
        {
            if (ConventionRegistry.Lookup(typeof(ConventionPack)).Conventions.All(_ => _.GetType() != typeof(EnumRepresentationConvention)))
            {
                var enumAsStringPack = new ConventionPack { new EnumRepresentationConvention(BsonType.String) };
                ConventionRegistry.Register(EnumAsStringConventionName, enumAsStringPack, t => true);
            }
        }
    }
}