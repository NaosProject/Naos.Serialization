// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationConfigurationBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.TypeRepresentation;
    using OBeautifulCode.Validation.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Common configuration base across all kinds of serialization.
    /// </summary>
    public abstract class SerializationConfigurationBase
    {
        /// <summary>
        /// Binding flags used in <see cref="DiscoverAllContainedTypes"/> to reflect on a type.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Name is correct.")]
        public const BindingFlags DiscoveryBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        private readonly object syncConfigure = new object();
        private bool configured;

        private static readonly IReadOnlyCollection<Type> InternallyRequiredTypes = new[]
        {
            typeof(TypeDescription),
            typeof(SerializationDescription),
            typeof(DescribedSerialization),
        };

        /// <summary>
        /// Run configuration logic.
        /// </summary>
        public virtual void Configure()
        {
            if (!this.configured)
            {
                lock (this.syncConfigure)
                {
                    if (!this.configured)
                    {
                        // Basic assertions.
                        (this.ClassTypesToRegisterAlongWithInheritors ?? new List<Type>()).Select(_ => _.IsClass).Named(Invariant($"{nameof(this.ClassTypesToRegisterAlongWithInheritors)}.Select(_ => _.{nameof(Type.IsClass)})")).Must().Each().BeTrue();
                        (this.InterfaceTypesToRegisterImplementationOf ?? new List<Type>()).Select(_ => _.IsInterface).Named(Invariant($"{nameof(this.InterfaceTypesToRegisterImplementationOf)}.Select(_ => _.{nameof(Type.IsInterface)})")).Must().Each().BeTrue();

                        // Run optional initial config.
                        this.InitialConfiguration();

                        // Run all dependent configurations.
                        foreach (var dependentConfigurationType in this.DependentConfigurationTypes ?? new List<Type>())
                        {
                            SerializationConfigurationManager.Configure(dependentConfigurationType);
                        }

                        this.RegisterTypes(this.ClassTypesToRegister);

                        var discoveredTypes = DiscoverAllContainedTypes(this.TypesToAutoRegisterWithDiscovery);

                        var typesToAutoRegister = new Type[0]
                            .Concat(InternallyRequiredTypes)
                            .Concat(this.TypesToAutoRegister)
                            .Concat(this.InterfaceTypesToRegisterImplementationOf)
                            .Concat(this.ClassTypesToRegisterAlongWithInheritors)
                            .Concat(discoveredTypes)
                            .ToList();

                        var typesToAutoRegisterWithAssignables = DiscoverAllAssignableTypes(typesToAutoRegister);

                        this.RegisterTypes(typesToAutoRegisterWithAssignables);

                        this.InternalConfigure();

                        this.FinalConfiguration();

                        this.configured = true;
                    }
                }
            }
        }

        private static IReadOnlyCollection<Type> DiscoverAllContainedTypes(IReadOnlyCollection<Type> types)
        {
            var typeFullNameToTypeObjectMap = new Dictionary<string, Type>();
            var typeQueue = new Queue<Type>(types);
            Type type;

            bool FilterToUsableTypes(MemberInfo memberInfo) => !memberInfo.CustomAttributes.Select(s => s.AttributeType).Contains(typeof(CompilerGeneratedAttribute));

            while (typeQueue.Any() && (type = typeQueue.Dequeue()) != null)
            {
                if (!typeFullNameToTypeObjectMap.ContainsKey(type.FullName))
                {
                    typeFullNameToTypeObjectMap.Add(type.FullName, type);
                }

                var allCurrentTypeMembers = type.GetMembers(DiscoveryBindingFlags).Where(FilterToUsableTypes).ToList();
                var newTypes = allCurrentTypeMembers.Select(_ => _.DeclaringType).Distinct().ToList();
                newTypes.Where(_ => !typeFullNameToTypeObjectMap.ContainsKey(_.FullName)).ToList().ForEach(_ => typeQueue.Enqueue(_));
            }

            var result = typeFullNameToTypeObjectMap.Values.Where(_ => _.IsAssignableType()).ToList();
            return result;
        }

        /// <summary>
        /// Register the specified type and class types that are assignable to those specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        private static IReadOnlyCollection<Type> DiscoverAllAssignableTypes(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull();

            var allTypesToConsiderForRegistration = GetAllTypesToConsiderForRegistration();
            var classTypesToRegister = allTypesToConsiderForRegistration.Where(typeToConsider =>
                        typeToConsider.IsClass &&
                        (!typeToConsider.IsAnonymous()) &&
                        (!typeToConsider.IsGenericTypeDefinition) && // can't do an IsAssignableTo check on generic type definitions
                        types.Any(typeToAutoRegister => typeToConsider.IsAssignableTo(typeToAutoRegister)))
                .ToList();

            return classTypesToRegister;
        }

        /// <summary>
        /// Discover all types that should considered for registration when looking for derivative types.
        /// </summary>
        /// <returns>All types that should be considered for registration.</returns>
        protected static IReadOnlyCollection<Type> GetAllTypesToConsiderForRegistration()
        {
            return AssemblyLoader.GetLoadedAssemblies().GetTypesFromAssemblies();
        }

        /// <summary>
        /// Gets a list of <see cref="SerializationConfigurationBase"/>'s that are needed for the current implementation of <see cref="SerializationConfigurationBase"/>.  Optionally overrideable, DEFAULT is empty collection.
        /// </summary>
        protected virtual IReadOnlyCollection<Type> DependentConfigurationTypes => new Type[0];

        /// <summary>
        /// Gets a list of <see cref="Type"/>s to auto-register.
        /// Auto-registration is a convenient way to register types; it accounts for interface implementations and class inheritance when performing the registration.
        /// For interface types, all implementations will be also be registered.  For classes, all inheritors will also be registered.  These additional types do not need to be specified.
        /// </summary>
        protected virtual IReadOnlyCollection<Type> TypesToAutoRegister => new Type[0];

        /// <summary>
        /// Gets a list of <see cref="Type"/>s to auto-register with discovery.
        /// Will take these specified types and recursively detect all model objects used then treat them all as if they were specified on <see cref="TypesToAutoRegister" /> directly.
        /// </summary>
        protected virtual IReadOnlyCollection<Type> TypesToAutoRegisterWithDiscovery => new Type[0];

        /// <summary>
        /// Gets an optional <see cref="TrackerCollisionStrategy" /> to use when a <see cref="Type" /> is already registered; DEFAULT is <see cref="TrackerCollisionStrategy.Skip" />.
        /// </summary>
        protected virtual TrackerCollisionStrategy TypeTrackerCollisionStrategy => TrackerCollisionStrategy.Skip;

        /// <summary>
        /// Gets a list of class <see cref="Type"/>s to register.
        /// </summary>
        protected virtual IReadOnlyCollection<Type> ClassTypesToRegister => new Type[0];

        /// <summary>
        /// Gets a list of parent class <see cref="Type"/>s to register.  These classes and all of their inheritors will be registered.  The inheritors do not need to be specified.
        /// </summary>
        protected virtual IReadOnlyCollection<Type> ClassTypesToRegisterAlongWithInheritors => new Type[0];

        /// <summary>
        /// Gets a list of interface <see cref="Type"/>s whose implementations should be registered.
        /// </summary>
        protected virtual IReadOnlyCollection<Type> InterfaceTypesToRegisterImplementationOf => new Type[0];

        /// <summary>
        /// Optional template method to be used by a specific configuration implementation at the beginning of the configuration logic.
        /// </summary>
        protected virtual void InitialConfiguration()
        {
            /* no-op - just for additional custom logic */
        }

        /// <summary>
        /// Optional to each serializer, a template method for any specific logic for the serialization implementation.
        /// </summary>
        protected virtual void InternalConfigure()
        {
            /* no-op - just for additional custom logic */
        }

        /// <summary>
        /// Optional template method to be used by a specific configuration implementation at the end of the configuration logic.
        /// </summary>
        protected virtual void FinalConfiguration()
        {
            /* no-op - just for additional custom logic */
        }

        /// <summary>
        /// Register type using specific internal conventions.
        /// </summary>
        /// <param name="type">Type to register.</param>
        protected void RegisterType(Type type)
        {
            this.RegisterTypes(new[] { type });
        }

        /// <summary>
        /// Register type using specific internal conventions.
        /// </summary>
        /// <typeparam name="T">Type to register.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Want to use this as a generic.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Like this structure.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterType<T>()
        {
            this.RegisterTypes(new[] { typeof(T) });
        }

        /// <summary>
        /// Register types using specific internal conventions.
        /// </summary>
        /// <param name="types">Types to register.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected abstract void RegisterTypes(IReadOnlyCollection<Type> types);
    }

    /// <summary>
    /// Null implementation of <see cref="SerializationConfigurationBase"/>.
    /// </summary>
    public sealed class NullSerializationConfiguration : SerializationConfigurationBase
    {
        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            /* no-op */
        }
    }
}