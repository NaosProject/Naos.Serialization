// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationConfigurationManager.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Collections.Generic;
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
            FetchOrCreateConfigurationInstance(typeof(T));
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
            var returnType = typeof(TReturn);
            type.IsAssignableTo(returnType).Named(Invariant($"typeMustBeSubclassOf{returnType}")).Must().BeTrue();
            type.HasParameterlessConstructor().Named("typeHasParameterLessConstructor").Must().BeTrue();

            var result = FetchOrCreateConfigurationInstance(type);

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

            FetchOrCreateConfigurationInstance(type);
         }

        private static Type GetInheritorOfSerializationBase(this Type configurationType)
        {
            var type = configurationType.BaseType;
            while (type != null && type.BaseType != null && type.BaseType != typeof(SerializationConfigurationBase))
            {
                type = type.BaseType;
            }

            return
                   type != null
                   && type.BaseType != null
                   && type.BaseType == typeof(SerializationConfigurationBase)
                ? type
                : null;
        }

        private static SerializationConfigurationBase FetchOrCreateConfigurationInstance(Type configurationType)
        {
            lock (SyncInstances)
            {
                if (!Instances.ContainsKey(configurationType))
                {
                    var instance = (SerializationConfigurationBase)configurationType.Construct();

                    var allDependentConfigTypes = instance.DependentConfigurationTypes.ToList();

                    if (!(instance is IDoNotNeedInternalDependencies))
                    {
                        allDependentConfigTypes.AddRange(instance.InternalDependentConfigurationTypes);
                    }

                    allDependentConfigTypes = allDependentConfigTypes.Distinct().ToList();

                    var configInheritor = configurationType.GetInheritorOfSerializationBase();

                    // TODO: test this throw.
                    // This protects against a JsonConfiguration listing dependent types that are BsonConfiguration derivatives, and vice-versa.
                    var rogueDependents = allDependentConfigTypes.Where(_ => _.GetInheritorOfSerializationBase() != configInheritor).ToList();
                    if (rogueDependents.Any())
                    {
                        throw new InvalidOperationException(Invariant($"Configuration {configurationType} has {nameof(instance.DependentConfigurationTypes)} ({string.Join(",", rogueDependents)}) that do not share the same first layer of inheritance {configInheritor}."));
                    }

                    var dependentConfigTypeToConfigMap = new Dictionary<Type, SerializationConfigurationBase>();

                    foreach (var dependentConfigType in allDependentConfigTypes)
                    {
                        var dependentConfigInstance = FetchOrCreateConfigurationInstance(dependentConfigType);

                        var dependentConfigDependentConfigurationTypeToInstanceMap = dependentConfigInstance.DependentConfigurationTypeToInstanceMap;

                        foreach (var dependentConfigDependentConfigType in dependentConfigDependentConfigurationTypeToInstanceMap.Keys)
                        {
                            if (!dependentConfigTypeToConfigMap.ContainsKey(dependentConfigDependentConfigType))
                            {
                                dependentConfigTypeToConfigMap.Add(dependentConfigDependentConfigType, dependentConfigDependentConfigurationTypeToInstanceMap[dependentConfigDependentConfigType]);
                            }
                        }

                        if (!dependentConfigTypeToConfigMap.ContainsKey(dependentConfigType))
                        {
                            dependentConfigTypeToConfigMap.Add(dependentConfigType, dependentConfigInstance);
                        }
                    }

                    instance.Configure(dependentConfigTypeToConfigMap);

                    Instances.Add(configurationType, instance);
                }

                var result = Instances[configurationType];

                return result;
            }
        }
    }
}