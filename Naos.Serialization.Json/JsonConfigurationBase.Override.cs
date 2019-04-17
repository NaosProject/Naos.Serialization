// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigurationBase.Override.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Naos.Serialization.Domain;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Base class to use for creating <see cref="NaosJsonSerializer" /> configuration.
    /// </summary>
    public abstract partial class JsonConfigurationBase
    {
        /// <inheritdoc />
        protected override void InternalConfigure()
        {
            var nullRegisteredConverterMap =
                new Dictionary<SerializationDirection, IReadOnlyCollection<RegisteredJsonConverter>>
                {
                    { SerializationDirection.Serialize, new RegisteredJsonConverter[0] },
                    { SerializationDirection.Deserialize, new RegisteredJsonConverter[0] },
                };

            var dependentConfigTypes = new HashSet<Type>(this.DependentConfigurationTypes);
            while (dependentConfigTypes.Any())
            {
                var type = dependentConfigTypes.First();
                dependentConfigTypes.Remove(type);

                JsonConfigurationBase config = (JsonConfigurationBase)this.DependentConfigurationTypeToInstanceMap[type];

                dependentConfigTypes.AddRange(config.DependentConfigurationTypes);

                this.RegisteredSerializingConverters.AddRange(config.RegisteredSerializingConverters);
                this.TypesWithConverters.AddRange(config.RegisteredSerializingConverters.SelectMany(_ => _.HandledTypes));

                this.RegisteredDeserializingConverters.AddRange(config.RegisteredDeserializingConverters);
                this.TypesWithConverters.AddRange(config.RegisteredDeserializingConverters.SelectMany(_ => _.HandledTypes));
            }

            var registrationDetails = new RegistrationDetails(this.GetType());
            var serializingConverters = ((this.ConvertersToPushOnStack ?? nullRegisteredConverterMap)[SerializationDirection.Serialize] ?? new RegisteredJsonConverter[0]).ToList();
            this.RegisteredSerializingConverters.AddRange(serializingConverters);
            var serializingConverterTypes = serializingConverters.SelectMany(_ => _.HandledTypes).ToList();
            this.TypesWithConverters.AddRange(serializingConverterTypes);
            serializingConverterTypes.ForEach(_ => this.MutableRegisteredTypeToDetailsMap.Add(_, registrationDetails));

            var deserializingConverters = ((this.ConvertersToPushOnStack ?? nullRegisteredConverterMap)[SerializationDirection.Deserialize] ?? new RegisteredJsonConverter[0]).ToList();
            this.RegisteredDeserializingConverters.AddRange(deserializingConverters);
            var deserializingConverterTypes = deserializingConverters.SelectMany(_ => _.HandledTypes).ToList();
            this.TypesWithConverters.AddRange(deserializingConverterTypes);
            deserializingConverterTypes.ForEach(_ => this.MutableRegisteredTypeToDetailsMap.Add(_, registrationDetails));
        }

        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull();

            var discoveredRegistrationDetails = new RegistrationDetails(this.GetType());
            this.MutableRegisteredTypeToDetailsMap.AddRange(types.ToDictionary(k => k, v => discoveredRegistrationDetails));

            var inheritedTypeConverterTypes = types.Where(t =>
                !InheritedTypeConverterBlackList.Contains(t) &&
                (t.IsAbstract || t.IsInterface || types.Any(a => a != t && (t.IsAssignableTo(a) || a.IsAssignableTo(t))))).Distinct().ToList();

            // TODO: what info do we want to capture here? should we give registration details?
            this.InheritedTypesToHandle.AddRange(inheritedTypeConverterTypes.Except(this.TypesWithConverters));
        }
    }
}