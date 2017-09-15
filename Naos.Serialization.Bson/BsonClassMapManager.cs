// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonClassMapManager.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Example of how to create a BsonClassMapManager.
    /// </summary>
    public static class BsonClassMapManager
    {
        private static readonly object SyncReadInstances = new object();
        private static readonly object SyncWriteInstances = new object();

        private static readonly Dictionary<Type, BsonClassMapperBase> Instances = new Dictionary<Type, BsonClassMapperBase>();

        /// <summary>
        /// Gets the singleton instance of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of derivative of <see cref="BsonClassMapperBase"/> to use.</typeparam>
        /// <returns>Singleton instance of the specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prefer to use in the generic sense.")]
        public static BsonClassMapperBase Instance<T>()
            where T : BsonClassMapperBase, new()
        {
            var type = typeof(T);
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
                            ret = new T();
                            Instances.Add(type, ret);

                            return ret;
                        }
                    }
                }
            }
        }
    }
}