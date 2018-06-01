// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigurationManager.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Generic;

    using Naos.Serialization.Domain.Extensions;

    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Manager class to govern configuration of <see cref="JsonConfigurationBase"/> implementations.
    /// </summary>
    public static class JsonConfigurationManager
    {
        private static readonly object SyncInstances = new object();

        private static readonly Dictionary<Type, JsonConfigurationBase> Instances = new Dictionary<Type, JsonConfigurationBase>();

        /// <summary>
        /// Configures and returns the specified <see cref="JsonConfigurationBase"/> type.
        /// </summary>
        /// <typeparam name="T">Type of derivative of <see cref="JsonConfigurationBase"/> to use.</typeparam>
        /// <returns>The configured <see cref="JsonConfigurationBase" /> instance.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static JsonConfigurationBase Configure<T>()
            where T : JsonConfigurationBase, new()
        {
            var instance = Instance<T>();
            instance.Configure();
            return instance;
        }

        /// <summary>
        /// Configures and returns the specified <see cref="JsonConfigurationBase"/> type.
        /// </summary>
        /// <param name="type">Type of derivative of <see cref="JsonConfigurationBase"/> to use.</param>
        /// <returns>The configured <see cref="JsonConfigurationBase" /> instance.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static JsonConfigurationBase Configure(Type type)
        {
            new { type }.Must().NotBeNull();
            type.IsSubclassOf(typeof(JsonConfigurationBase)).Named(Invariant($"typeMustBeSubclassOf{nameof(JsonConfigurationBase)}")).Must().BeTrue();
            type.HasParameterlessConstructor().Named("typeHasParameterLessConstructor").Must().BeTrue();

            var instance = Instance(type, () => (JsonConfigurationBase)type.Construct());
            instance.Configure();
            return instance;
        }

        /// <summary>
        /// Gets the singleton instance of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of derivative of <see cref="JsonConfigurationBase"/> to use.</typeparam>
        /// <returns>Singleton instance of the specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        private static JsonConfigurationBase Instance<T>()
            where T : JsonConfigurationBase, new()
        {
            return Instance(typeof(T), () => new T());
        }

        private static JsonConfigurationBase Instance(Type type, Func<JsonConfigurationBase> creatorFunc)
        {
            lock (SyncInstances)
            {
                if (!Instances.ContainsKey(type))
                {
                    Instances.Add(type, creatorFunc());
                }

                return Instances[type];
            }
        }
    }
}