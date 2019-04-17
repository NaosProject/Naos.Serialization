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
        protected sealed override void InternalConfigure()
        {
            // TODO: depdendentA <- dependentC
            //       depdendentB <- dependentC
            // Both A & B will publish the converters from C

            // A <- B <- C.ConverterB
            // C.ConverterB
            // B.ConverterB

            var dependentConfigTypes = new List<Type>(this.DependentConfigurationTypes.Reverse());
            while (dependentConfigTypes.Any())
            {
                var type = dependentConfigTypes.Last();
                dependentConfigTypes.RemoveAt(dependentConfigTypes.Count);

                var dependentConfig = (JsonConfigurationBase)this.DependentConfigurationTypeToInstanceMap[type];
                dependentConfigTypes.AddRange(dependentConfig.DependentConfigurationTypes);

                this.ProcessConverter(dependentConfig.RegisteredConverters);
            }

            var converters = (this.ConvertersToRegister ?? new RegisteredJsonConverter[0]).ToList();
            var handledTypes = this.ProcessConverter(converters);
            var registrationDetails = new RegistrationDetails(this.GetType());

            foreach (var handledType in handledTypes)
            {
                this.MutableRegisteredTypeToDetailsMap.Add(handledType, registrationDetails);
            }
        }

        private IReadOnlyCollection<Type> ProcessConverter(IList<RegisteredJsonConverter> registeredConverters)
        {
            var handledTypes = registeredConverters.SelectMany(_ => _.HandledTypes).ToList();

            if (this.RegisteredTypeToDetailsMap.Keys.Intersect(handledTypes).Any())
            {
                throw new DuplicateRegistrationException(
                    Invariant(
                        $"Trying to register one or more types via {nameof(this.ConvertersToRegister)} processing, but one is already registered."),
                    handledTypes);
            }

            // TODO: how do we check the scenario above to ensure we do not double register converters
            this.RegisteredConverters.AddRange(registeredConverters);
            this.TypesWithConverters.AddRange(handledTypes);
            this.TypesWithStringConverters.AddRange(
                registeredConverters
                    .Where(_ => _.OutputKind == RegisteredJsonConverterOutputKind.String)
                    .SelectMany(_ => _.HandledTypes).Distinct());

            return handledTypes;
        }

        /// <inheritdoc />
        protected sealed override void RegisterTypes(IReadOnlyCollection<Type> types)
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