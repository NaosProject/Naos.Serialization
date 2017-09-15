// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonClassMapManager.cs" company="Naos">
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
    /// Example of how to create a BsonClassMapManager.
    /// </summary>
    public static class BsonClassMapManager
    {
        private static readonly object SyncReadInstances = new object();
        private static readonly object SyncWriteInstances = new object();

        private static readonly Dictionary<Type, BsonClassMapperBase> Instances = new Dictionary<Type, BsonClassMapperBase>();

        /// <summary>
        /// Registers the class maps for the specified <see cref="BsonClassMapperBase"/> type.
        /// </summary>
        /// <typeparam name="T">Type of derivative of <see cref="BsonClassMapperBase"/> to use.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static void RegisterClassMaps<T>()
            where T : BsonClassMapperBase, new()
        {
            var instance = Instance<T>();
            instance.RegisterClassMaps();
        }

        /// <summary>
        /// Registers the class maps for the specified <see cref="BsonClassMapperBase"/> type.
        /// </summary>
        /// <param name="type">Type of derivative of <see cref="BsonClassMapperBase"/> to use.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static void RegisterClassMaps(Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();
            type.IsSubclassOf(typeof(BsonClassMapperBase)).Named(Invariant($"typeMustBeSubclassOf{nameof(BsonClassMapperBase)}")).Must().BeTrue().OrThrowFirstFailure();

            var instance = Instance(type, () => (BsonClassMapperBase)type.Construct());
            instance.RegisterClassMaps();
        }

        /// <summary>
        /// Gets the singleton instance of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of derivative of <see cref="BsonClassMapperBase"/> to use.</typeparam>
        /// <returns>Singleton instance of the specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static BsonClassMapperBase Instance<T>()
            where T : BsonClassMapperBase, new()
        {
            return Instance(typeof(T), () => new T());
        }

        private static BsonClassMapperBase Instance(Type type, Func<BsonClassMapperBase> creatorFunc)
        {
            lock (SyncReadInstances)
            {
                BsonClassMapperBase ret;
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