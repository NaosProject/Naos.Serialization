// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonClassMapperBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using MongoDB.Bson.Serialization;

    using Spritely.Recipes;

    /// <summary>
    /// Base class to use for creating
    /// </summary>
    public abstract class BsonClassMapperBase
    {
        private static readonly MethodInfo RegisterClassMapGenericMethod = typeof(BsonClassMap).GetMethods().Single(_ => (_.Name == nameof(BsonClassMap.RegisterClassMap)) && (!_.GetParameters().Any()) && _.IsGenericMethod);

        private static readonly object SyncRegister = new object();

        private static bool registered;

        /// <summary>
        /// Class to manage class maps necessary for the CoScore Storage Model.
        /// </summary>
        public void RegisterClassMaps()
        {
            if (!registered)
            {
                lock (SyncRegister)
                {
                    if (!registered)
                    {
                        if (this.ShouldRegisterEnumConvention)
                        {
                            NaosBsonConventions.RegisterEnumAsStringConventionIfNotRegistered();
                        }

                        foreach (var dependantMapperType in this.DependentMapperTypes)
                        {
                            BsonClassMapManager.RegisterClassMaps(dependantMapperType);
                        }

                        this.RegisterCustomClassMappings();

                        registered = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not to run <see cref="NaosBsonConventions.RegisterEnumAsStringConventionIfNotRegistered"/>.  Optionally overrideable, DEFAULT is true.
        /// </summary>
        protected virtual bool ShouldRegisterEnumConvention => true;

        /// <summary>
        /// Gets a list of <see cref="BsonClassMapperBase"/>'s that are needed for the current implemenation of <see cref="BsonClassMapperBase"/>.  Optionally overrideable, DEFAULT is empty collection.
        /// </summary>
        protected virtual IReadOnlyCollection<Type> DependentMapperTypes => new Type[0];

        /// <summary>
        /// Template method to override and specify custom class maps.
        /// </summary>
        protected abstract void RegisterCustomClassMappings();

        /// <summary>
        /// Method to use relection and call <see cref="BsonClassMap.RegisterClassMap{TClass}()"/> using the <see cref="Type"/> as a parameter.
        /// </summary>
        /// <param name="type">Type to register.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassMap(Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            var genericRegisterClassMapMethod = RegisterClassMapGenericMethod.MakeGenericMethod(type);
            genericRegisterClassMapMethod.Invoke(null, null);
        }

        /// <summary>
        /// Method to use relection and call <see cref="BsonClassMap.RegisterClassMap{TClass}()"/> using the <see cref="Type"/> as a parameter.
        /// </summary>
        /// <param name="types">Types to register.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassMap(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull().OrThrowFirstFailure();

            foreach (var type in types)
            {
                this.RegisterClassMap(type);
            }
        }

        /// <summary>
        /// Method to register the specified type and all derivative types in the same assembly.
        /// </summary>
        /// <param name="types">Types to register.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterTypesAndTheirSubclasses(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull().OrThrowFirstFailure();

            IReadOnlyCollection<Type> GetTypeAndDerivativeTypes(Type type)
            {
                var derivativeTypes = type.Assembly.GetTypes().Where(_ => _.IsSubclassOf(type)).ToList();
                derivativeTypes.Add(type);
                return derivativeTypes;
            }

            foreach (var type in types)
            {
                var allTypes = GetTypeAndDerivativeTypes(type);
                this.RegisterClassMap(allTypes);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Just for example purposes, move to recipe eventually.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Just for example purposes, move to recipe eventually.")]
        private void ExampleSpecificRegistration()
        {
            // show dictionaries too
            // show dictionary with enum key
            BsonClassMap.RegisterClassMap<SomeType>(
                cm =>
                    {
                        cm.AutoMap();
                        cm.MapMember(c => c.Enumeration).SetEnumStringSerializer<SomeEnum>();
                    });
        }

        private class SomeType
        {
            public SomeEnum Enumeration { get; set; }
        }

        private enum SomeEnum
        {
        }
    }

    /// <summary>
    /// Null implementation of <see cref="BsonClassMapperBase"/>.
    /// </summary>
    public class NullBsonClassMapper : BsonClassMapperBase
    {
        /// <inheritdoc cref="BsonClassMapperBase"/>
        protected override void RegisterCustomClassMappings()
        {
            /* no-op */
        }
    }
}