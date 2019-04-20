// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationConfigurationManager.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Manager class to govern configuration of <see cref="SerializationConfigurationBase"/> implementations.
    /// </summary>
    public static class SerializationConfigurationManager
    {
        private static readonly object SyncInstances = new object();

        private static readonly Dictionary<Type, SerializationConfigurationBase> Instances = new Dictionary<Type, SerializationConfigurationBase>();

        /// <summary>
        /// Registers the class maps for the specified <see cref="SerializationConfigurationBase"/> type.
        /// </summary>
        /// <typeparam name="T">Type of derivative of <see cref="SerializationConfigurationBase"/> to use.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static void Configure<T>()
            where T : SerializationConfigurationBase, new()
        {
            // TODO: is there a race condition here? should we lock while calling configure...
            GetConfiguredType(typeof(T));
        }

        /// <summary>
        /// Registers the class maps for the specified <see cref="SerializationConfigurationBase"/> type.
        /// </summary>
        /// <typeparam name="T">Type of derivative of <see cref="SerializationConfigurationBase"/> to configured and return as.</typeparam>
        /// <returns>Configured instance.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static T ConfigureWithReturn<T>()
            where T : SerializationConfigurationBase
        {
            var result = ConfigureWithReturn<T>(typeof(T));
            return result;
        }

        /// <summary>
        /// Registers the class maps for the specified <see cref="SerializationConfigurationBase"/> type.
        /// </summary>
        /// <param name="type">Type of derivative of <see cref="SerializationConfigurationBase"/> to use.</param>
        /// <typeparam name="TReturn">Type of derivative of <see cref="SerializationConfigurationBase"/> to return as.</typeparam>
        /// <returns>Configured instance.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static TReturn ConfigureWithReturn<TReturn>(Type type)
            where TReturn : SerializationConfigurationBase
        {
            new { type }.Must().NotBeNull();
            type.IsAssignableTo(typeof(TReturn)).Named(Invariant($"typeMustBeSubclassOf{nameof(TReturn)}")).Must().BeTrue();
            type.HasParameterlessConstructor().Named("typeHasParameterLessConstructor").Must().BeTrue();

            var result = GetConfiguredType(type);
            return (TReturn)result;
        }

        /// <summary>
        /// Registers the class maps for the specified <see cref="SerializationConfigurationBase"/> type.
        /// </summary>
        /// <param name="type">Type of derivative of <see cref="SerializationConfigurationBase"/> to use.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static void Configure(Type type)
        {
            new { type }.Must().NotBeNull();
            type.IsSubclassOf(typeof(SerializationConfigurationBase)).Named(Invariant($"typeMustBeSubclassOf{nameof(SerializationConfigurationBase)}")).Must().BeTrue();
            type.HasParameterlessConstructor().Named("typeHasParameterLessConstructor").Must().BeTrue();

            GetConfiguredType(type);
         }

        private static SerializationConfigurationBase GetConfiguredType(Type configurationType, Dictionary<Type, SerializationConfigurationBase> dependentConfigMap = null)
        {
            var instanceWasCreated = FetchOrCreateConfigurationInstance(configurationType, out var instance);

            new { instance }.Must().NotBeNull();

            if (!instanceWasCreated)
            {
                return instance;
            }

            if (dependentConfigMap == null)
            {
                dependentConfigMap = new Dictionary<Type, SerializationConfigurationBase>();
            }

            foreach (var dependentType in instance.DependentConfigurationTypes)
            {
                var dependentInstance = GetConfiguredType(dependentType, dependentConfigMap);

                if (!dependentConfigMap.ContainsKey(dependentType))
                {
                    dependentConfigMap.Add(dependentType, dependentInstance);
                }
            }

            var dependentConfigMapShallowClone = dependentConfigMap.ToDictionary(k => k.Key, v => v.Value);
            instance.Configure(dependentConfigMapShallowClone);

            return instance;
        }

        private static bool FetchOrCreateConfigurationInstance(
            Type type,
            out SerializationConfigurationBase outputResult,
            Func<SerializationConfigurationBase> creatorFunc = null)
        {
            var result = false;
            var localCreatorFunc = creatorFunc ?? (() => (SerializationConfigurationBase)type.Construct());

            lock (SyncInstances)
            {
                if (!Instances.ContainsKey(type))
                {
                    Instances.Add(type, localCreatorFunc());
                    result = true;
                }

                outputResult = Instances[type];
                return result;
            }
        }
    }
}