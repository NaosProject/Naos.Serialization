// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBagConfigurationBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.PropertyBag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.Serialization.Domain;
    using OBeautifulCode.Collection.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Base class to use for creating a <see cref="NaosPropertyBagSerializer" /> configuration.
    /// </summary>
    public abstract class PropertyBagConfigurationBase : SerializationConfigurationBase
    {
        /// <summary>
        /// Gets the map of type to specific serializer.
        /// </summary>
        private Dictionary<Type, IStringSerializeAndDeserialize> TypeToSerializerMap { get; } = new Dictionary<Type, IStringSerializeAndDeserialize>();

        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            var registrationDetails = new RegistrationDetails(this.GetType());
            foreach (var type in types ?? new Type[0])
            {
                this.MutableRegisteredTypeToDetailsMap.Add(type, registrationDetails);
            }
        }

        /// <inheritdoc />
        public sealed override IReadOnlyCollection<Type> InternalDependentConfigurationTypes => new[] { typeof(InternalPropertyBagConfiguration) };

        /// <summary>
        /// Gets the registered serializer set to use.
        /// </summary>
        protected IList<RegisteredStringSerializer> RegisteredSerializers { get; } = new List<RegisteredStringSerializer>();

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

        /// <inheritdoc />
        protected sealed override void InternalConfigure()
        {
            var dependentConfigTypes = new List<Type>(this.DependentConfigurationTypes.Reverse());
            while (dependentConfigTypes.Any())
            {
                var type = dependentConfigTypes.Last();
                dependentConfigTypes.RemoveAt(dependentConfigTypes.Count - 1);

                var dependentConfig = (PropertyBagConfigurationBase)this.DependentConfigurationTypeToInstanceMap[type];
                dependentConfigTypes.AddRange(dependentConfig.DependentConfigurationTypes);

                this.ProcessSerializer(dependentConfig.RegisteredSerializers, false);
            }

            var serializers = (this.SerializersToRegister ?? new RegisteredStringSerializer[0]).ToList();
            var handledTypes = this.ProcessSerializer(serializers);
            var registrationDetails = new RegistrationDetails(this.GetType());

            foreach (var handledType in handledTypes)
            {
                this.MutableRegisteredTypeToDetailsMap.Add(handledType, registrationDetails);
            }
        }

        private IReadOnlyCollection<Type> ProcessSerializer(IList<RegisteredStringSerializer> registeredSerializers, bool checkForAlreadyRegistered = true)
        {
            var handledTypes = registeredSerializers.SelectMany(_ => _.HandledTypes).ToList();

            if (checkForAlreadyRegistered && this.RegisteredTypeToDetailsMap.Keys.Intersect(handledTypes).Any())
            {
                throw new DuplicateRegistrationException(
                    Invariant($"Trying to register one or more types via {nameof(this.SerializersToRegister)} processing, but one is already registered."),
                    handledTypes);
            }

            this.RegisteredSerializers.AddRange(registeredSerializers);

            foreach (var registeredSerializer in registeredSerializers)
            {
                foreach (var handledType in registeredSerializer.HandledTypes)
                {
                    if (this.TypeToSerializerMap.ContainsKey(handledType))
                    {
                        if (checkForAlreadyRegistered)
                        {
                            throw new DuplicateRegistrationException(
                                Invariant($"Type {handledType} is already registered."),
                                new[] { handledType });
                        }
                    }
                    else
                    {
                        this.TypeToSerializerMap.Add(handledType, registeredSerializer.SerializerBuilderFunction());
                    }
                }
            }

            return handledTypes;
        }

        /// <summary>
        /// Gets the optional serializers to add.
        /// </summary>
        protected virtual IReadOnlyCollection<RegisteredStringSerializer> SerializersToRegister => new List<RegisteredStringSerializer>();

        /// <summary>
        /// Builds a map of type to serializer.
        /// </summary>
        /// <returns>Map of type to specific serializer.</returns>
        public IReadOnlyDictionary<Type, IStringSerializeAndDeserialize> BuildConfiguredTypeToSerializerMap()
        {
            return this.TypeToSerializerMap;
        }
    }

    /// <summary>
    /// Internal implementation of <see cref="PropertyBagConfigurationBase" /> that will auto register necessary internal types.
    /// </summary>
    public sealed class InternalPropertyBagConfiguration : PropertyBagConfigurationBase, IDoNotNeedInternalDependencies
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegisterWithDiscovery => InternallyRequiredTypes;
    }

    /// <summary>
    /// Null implementation of <see cref="PropertyBagConfigurationBase"/>.
    /// </summary>
    public sealed class NullPropertyBagConfiguration : PropertyBagConfigurationBase, IImplementNullObjectPattern
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