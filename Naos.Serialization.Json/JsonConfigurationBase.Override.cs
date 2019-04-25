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
            var dependentConfigTypes = new List<Type>(this.DependentConfigurationTypes.Reverse());
            while (dependentConfigTypes.Any())
            {
                var type = dependentConfigTypes.Last();
                dependentConfigTypes.RemoveAt(dependentConfigTypes.Count - 1);

                var dependentConfig = (JsonConfigurationBase)this.DependentConfigurationTypeToInstanceMap[type];
                dependentConfigTypes.AddRange(dependentConfig.DependentConfigurationTypes);

                this.ProcessConverter(dependentConfig.RegisteredConverters, false);
                this.ProcessInheritedTypeConverterTypes(dependentConfig.RegisteredTypeToDetailsMap.Keys.ToList());
            }

            var converters = (this.ConvertersToRegister ?? new RegisteredJsonConverter[0]).ToList();
            var handledTypes = this.ProcessConverter(converters);
            var registrationDetails = new RegistrationDetails(this.GetType());

            foreach (var handledType in handledTypes)
            {
                this.MutableRegisteredTypeToDetailsMap.Add(handledType, registrationDetails);
            }
        }

        private IReadOnlyCollection<Type> ProcessConverter(IList<RegisteredJsonConverter> registeredConverters, bool checkForAlreadyRegistered = true)
        {
            var handledTypes = registeredConverters.SelectMany(_ => _.HandledTypes).ToList();

            if (checkForAlreadyRegistered && this.RegisteredTypeToDetailsMap.Keys.Intersect(handledTypes).Any())
            {
                throw new DuplicateRegistrationException(
                    Invariant($"Trying to register one or more types via {nameof(this.ConvertersToRegister)} processing, but one is already registered."),
                    handledTypes);
            }

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

            this.ProcessInheritedTypeConverterTypes(types);
        }

        private void ProcessInheritedTypeConverterTypes(IReadOnlyCollection<Type> types)
        {
            var inheritedTypeConverterTypes = types.Where(t =>
                !InheritedTypeConverterBlackList.Contains(t) &&
                (t.IsAbstract || t.IsInterface || types.Any(a => a != t && (t.IsAssignableTo(a) || a.IsAssignableTo(t))))).Distinct().ToList();

            // TODO: what info do we want to capture here? should we give registration details?
            this.InheritedTypesToHandle.AddRange(inheritedTypeConverterTypes.Except(this.TypesWithConverters));
        }
    }
}