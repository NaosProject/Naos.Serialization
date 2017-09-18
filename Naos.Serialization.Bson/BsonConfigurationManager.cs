// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonConfigurationManager.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.Collections.Generic;

    using OBeautifulCode.Reflection;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Manager class to govern configuration of <see cref="BsonConfigurationBase"/> implementations.
    /// </summary>
    public static class BsonConfigurationManager
    {
        private static readonly object SyncReadInstances = new object();
        private static readonly object SyncWriteInstances = new object();

        private static readonly Dictionary<Type, BsonConfigurationBase> Instances = new Dictionary<Type, BsonConfigurationBase>();

        /// <summary>
        /// Registers the class maps for the specified <see cref="BsonConfigurationBase"/> type.
        /// </summary>
        /// <typeparam name="T">Type of derivative of <see cref="BsonConfigurationBase"/> to use.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static void Configure<T>()
            where T : BsonConfigurationBase, new()
        {
            var instance = Instance<T>();
            instance.Configure();
        }

        /// <summary>
        /// Registers the class maps for the specified <see cref="BsonConfigurationBase"/> type.
        /// </summary>
        /// <param name="type">Type of derivative of <see cref="BsonConfigurationBase"/> to use.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static void Configure(Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();
            type.IsSubclassOf(typeof(BsonConfigurationBase)).Named(Invariant($"typeMustBeSubclassOf{nameof(BsonConfigurationBase)}")).Must().BeTrue().OrThrowFirstFailure();

            var instance = Instance(type, () => (BsonConfigurationBase)type.Construct());
            instance.Configure();
        }

        /// <summary>
        /// Gets the singleton instance of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of derivative of <see cref="BsonConfigurationBase"/> to use.</typeparam>
        /// <returns>Singleton instance of the specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        private static BsonConfigurationBase Instance<T>()
            where T : BsonConfigurationBase, new()
        {
            return Instance(typeof(T), () => new T());
        }

        private static BsonConfigurationBase Instance(Type type, Func<BsonConfigurationBase> creatorFunc)
        {
            lock (SyncReadInstances)
            {
                BsonConfigurationBase ret;
                var exists = Instances.TryGetValue(type, out ret);
                if (exists)
                {
                    return ret;
                }
                else
                {
                    lock (SyncWriteInstances)
                    {
                        var existsNow = Instances.TryGetValue(type, out ret);
                        if (existsNow)
                        {
                            return ret;
                        }
                        else
                        {
                            ret = creatorFunc();
                            Instances.Add(type, ret);

                            return ret;
                        }
                    }
                }
            }
        }
    }
}