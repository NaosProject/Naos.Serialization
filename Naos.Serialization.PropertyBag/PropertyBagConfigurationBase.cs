// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBagConfigurationBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.PropertyBag
{
    using System;
    using System.Collections.Generic;

    using Naos.Serialization.Domain;

    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Base class to use for creating a <see cref="NaosPropertyBagSerializer" /> configuration.
    /// </summary>
    public abstract class PropertyBagConfigurationBase : SerializationConfigurationBase
    {
        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            /* no-op */
        }

        /// <summary>
        /// Gets the key value delimiter to use for string serialization of the property bag.
        /// </summary>
        public virtual string StringSerializationKeyValueDelimiter { get; private set; } = NaosDictionaryStringStringSerializer.DefaultKeyValueDelimiter;

        /// <summary>
        /// Gets the line delimiter to use for string serialization of the property bag.
        /// </summary>
        public virtual string StringSerializationLineDelimiter { get; private set; } = NaosDictionaryStringStringSerializer.DefaultLineDelimiter;

        /// <summary>
        /// Gets the null value encoding to use for string serialization of the property bag.
        /// </summary>
        public virtual string StringSerializationNullValueEncoding { get; private set; } = NaosDictionaryStringStringSerializer.DefaultNullValueEncoding;

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
            new { customTypeToSerializerMappings }.Must().NotBeNull();

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