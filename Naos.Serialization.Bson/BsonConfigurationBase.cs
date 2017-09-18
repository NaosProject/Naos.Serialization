// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonConfigurationBase.cs" company="Naos">
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
    public abstract class BsonConfigurationBase
    {
        private static readonly MethodInfo RegisterClassMapGenericMethod = typeof(BsonClassMap).GetMethods().Single(_ => (_.Name == nameof(BsonClassMap.RegisterClassMap)) && (!_.GetParameters().Any()) && _.IsGenericMethod);

        private static readonly object SyncConfigure = new object();

        private static bool configured;

        /// <summary>
        /// Run configuration logic.
        /// </summary>
        public void Configure()
        {
            if (!configured)
            {
                lock (SyncConfigure)
                {
                    if (!configured)
                    {
                        if (this.ShouldRegisterEnumConvention)
                        {
                            NaosBsonConventions.RegisterEnumAsStringConventionIfNotRegistered();
                        }

                        foreach (var dependantMapperType in this.DependentMapperTypes)
                        {
                            BsonConfigurationManager.Configure(dependantMapperType);
                        }

                        this.CustomConfiguration();

                        configured = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not to run <see cref="NaosBsonConventions.RegisterEnumAsStringConventionIfNotRegistered"/>.  Optionally overrideable, DEFAULT is true.
        /// </summary>
        protected virtual bool ShouldRegisterEnumConvention => true;

        /// <summary>
        /// Gets a list of <see cref="BsonConfigurationBase"/>'s that are needed for the current implemenation of <see cref="BsonConfigurationBase"/>.  Optionally overrideable, DEFAULT is empty collection.
        /// </summary>
        protected virtual IReadOnlyCollection<Type> DependentMapperTypes => new Type[0];

        /// <summary>
        /// Template method to override and specify custom logic.
        /// </summary>
        protected abstract void CustomConfiguration();

        /// <summary>
        /// Method to use relection and call <see cref="BsonClassMap.RegisterClassMap{TClass}()"/> using the <see cref="Type"/> as a parameter.
        /// </summary>
        /// <param name="type">Type to register.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassMapForType(Type type)
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
        protected void RegisterClassMapForTypes(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull().OrThrowFirstFailure();

            foreach (var type in types)
            {
                this.RegisterClassMapForType(type);
            }
        }

        /// <summary>
        /// Method to register the specified type and all derivative types in the same assembly.
        /// </summary>
        /// <param name="types">Types to register.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassMapForClassTypesAndTheirSubclasses(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull().OrThrowFirstFailure();

            foreach (var type in types)
            {
                var allTypes = this.GetSubclassTypes(type);
                this.RegisterClassMapForTypes(allTypes);
            }
        }

        /// <summary>
        /// Method to register the specified type and all derivative types in the same assembly.
        /// </summary>
        /// <param name="types">Types to register.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassMapForInterfaceTypesAndTheirImplementations(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull().OrThrowFirstFailure();

            foreach (var type in types)
            {
                var allTypes = this.GetSubclassTypes(type);
                this.RegisterClassMapForTypes(allTypes);
            }
        }

        /// <summary>
        /// Get a list of the subclass types of the provided type and the provided type if <paramref name="includeSpecifiedTypeInReturnList"/> is true.
        /// </summary>
        /// <param name="classType">Type to find derivatives of.</param>
        /// <param name="includeSpecifiedTypeInReturnList">Optional value indicating whether or not to include the provided type in the return list; DEFAULT is true.</param>
        /// <returns>List of the subclass types of the provided type and the provided type if <paramref name="includeSpecifiedTypeInReturnList"/> is true.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected IReadOnlyCollection<Type> GetSubclassTypes(Type classType, bool includeSpecifiedTypeInReturnList = true)
        {
            new { classType }.Must().NotBeNull().OrThrowFirstFailure();
            new { classType.IsClass }.Must().BeTrue().OrThrowFirstFailure();

            var derivativeTypes = classType.Assembly.GetTypes().Where(_ => _.IsSubclassOf(classType)).ToList();

            if (includeSpecifiedTypeInReturnList)
            {
                derivativeTypes.Add(classType);
            }

            return derivativeTypes;
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
    /// Null implementation of <see cref="BsonConfigurationBase"/>.
    /// </summary>
    public sealed class NullBsonConfiguration : BsonConfigurationBase
    {
        /// <inheritdoc cref="BsonConfigurationBase"/>
        protected override void CustomConfiguration()
        {
            /* no-op */
        }
    }
}