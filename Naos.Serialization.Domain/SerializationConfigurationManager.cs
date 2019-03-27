// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationConfigurationManager.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Collections.Generic;

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
            var instance = FetchOrCreateConfigurationInstance<T>();
            instance.Configure();
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
            type.IsSubclassOf(typeof(TReturn)).Named(Invariant($"typeMustBeSubclassOf{nameof(TReturn)}")).Must().BeTrue();
            type.HasParameterlessConstructor().Named("typeHasParameterLessConstructor").Must().BeTrue();

            var instance = FetchOrCreateConfigurationInstance(type, () => (SerializationConfigurationBase)type.Construct());
            instance.Configure();
            return (TReturn)instance;
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

            var instance = FetchOrCreateConfigurationInstance(type, () => (SerializationConfigurationBase)type.Construct());
            instance.Configure();
        }

        /// <summary>
        /// Gets the singleton instance of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of derivative of <see cref="SerializationConfigurationBase"/> to use.</typeparam>
        /// <returns>Singleton instance of the specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        private static SerializationConfigurationBase FetchOrCreateConfigurationInstance<T>()
            where T : SerializationConfigurationBase, new()
        {
            return FetchOrCreateConfigurationInstance(typeof(T), () => new T());
        }

        private static SerializationConfigurationBase FetchOrCreateConfigurationInstance(Type type, Func<SerializationConfigurationBase> creatorFunc)
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