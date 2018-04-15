// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBagConfigurationBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.PropertyBag
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Naos.Serialization.Domain;

    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.TypeRepresentation;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Base class to use for creating
    /// </summary>
    public abstract class PropertyBagConfigurationBase
    {
        /// <summary>
        /// Gets a list of <see cref="PropertyBagConfigurationBase"/>'s that are needed for the current implemenation of <see cref="PropertyBagConfigurationBase"/>.  Optionally overrideable, DEFAULT is empty collection.
        /// </summary>
        protected virtual IReadOnlyCollection<Type> DependentConfigurationTypes => new Type[0];

        /// <summary>
        /// Gets the key value delimiter to use for string serialization of the property bag.
        /// </summary>
        public virtual string StringSerializationKeyValueDelimiter { get; private set; } = NaosDictionaryStringStringSerializer.DefaultKeyValueDelimiter;

        /// <summary>
        /// Gets the line delimiter to use for string serialization of the property bag.
        /// </summary>
        public virtual string StringSerializationLineDelimiter { get; private set; } = NaosDictionaryStringStringSerializer.DefaultLineDelimiter;

        /// <summary>
        /// Gets the strategy for collisions in type to serializer registrations.
        /// </summary>
        public virtual TypeSerializationRegistrationCollisionStrategy CollisionStrategy { get; private set; } = TypeSerializationRegistrationCollisionStrategy.Throw;

        /// <summary>
        /// Build a map of types to serializers to consider for property serialization.
        /// </summary>
        /// <returns>Map of types to serializes to consider for property serialization.</returns>
        public IReadOnlyDictionary<Type, IStringSerializeAndDeserialize> BuildTypeToSerializerMap()
        {
            var ret = new Dictionary<Type, IStringSerializeAndDeserialize>();
            var customTypeToSerializerMappings = this.CustomTypeToSerializerMappings();
            new { customTypeToSerializerMappings }.Must().NotBeNull().OrThrowFirstFailure();

            foreach (var customMapping in customTypeToSerializerMappings)
            {
                ret.Add(customMapping.Key, customMapping.Value);
            }

            foreach (var dependentConfigurationType in this.DependentConfigurationTypes)
            {
                var configuration = dependentConfigurationType.Construct<PropertyBagConfigurationBase>();
                var mappingsFromDependent = configuration.BuildTypeToSerializerMap();
                foreach (var mappingFromDependent in mappingsFromDependent)
                {
                    var collision = ret.ContainsKey(mappingFromDependent.Key);
                    if (collision && this.CollisionStrategy == TypeSerializationRegistrationCollisionStrategy.Throw)
                    {
                        throw new PropertyBagConfigurationException(Invariant($"Dependent configuration type {dependentConfigurationType} tried to add mapping for {mappingFromDependent.Key} but it was already added by {nameof(this.CustomTypeToSerializerMappings)} or a prior dependent with strategy set to {this.CollisionStrategy}"));
                    }
                    else if (collision && this.CollisionStrategy == TypeSerializationRegistrationCollisionStrategy.FirstWins)
                    {
                        /* no-op */
                    }
                    else if (collision && this.CollisionStrategy == TypeSerializationRegistrationCollisionStrategy.LastWins)
                    {
                        ret[mappingFromDependent.Key] = mappingFromDependent.Value;
                    }
                    else
                    {
                        ret.Add(mappingFromDependent.Key, mappingFromDependent.Value);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Build a map of types to serializers to consider for property serialization.
        /// </summary>
        /// <returns>Map of types to serializes to consider for property serialization.</returns>
        protected virtual IReadOnlyDictionary<Type, IStringSerializeAndDeserialize> CustomTypeToSerializerMappings()
        {
            return new Dictionary<Type, IStringSerializeAndDeserialize>();
        }
    }

    /// <summary>
    /// Null implementation of <see cref="PropertyBagConfigurationBase"/>.
    /// </summary>
    public sealed class NullPropertyBagConfiguration : PropertyBagConfigurationBase
    {
    }

    /// <summary>
    /// Strategy on dealing with collisions in the <see cref="PropertyBagConfigurationBase" /> logic.
    /// </summary>
    public enum TypeSerializationRegistrationCollisionStrategy
    {
        /// <summary>
        /// Throw an exception.
        /// </summary>
        Throw,

        /// <summary>
        /// First one registered is the one used; internal custom followed by in order run through dependents.
        /// </summary>
        FirstWins,

        /// <summary>
        /// Last one registered is the one used; internal custom followed by in order run through dependents.
        /// </summary>
        LastWins,
    }
}