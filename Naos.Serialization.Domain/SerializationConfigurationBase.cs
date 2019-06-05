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
    using OBeautifulCode.Type;
    using OBeautifulCode.Validation.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Common configuration base across all kinds of serialization.
    /// </summary>
    public abstract class SerializationConfigurationBase
    {
        /// <summary>
        /// The string representation of null.
        /// </summary>
        public const string NullSerializedStringValue = "null";

        /// <summary>
        /// Binding flags used in <see cref="DiscoverAllContainedAssignableTypes"/> to reflect on a type.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Name is correct.")]
        public const BindingFlags DiscoveryBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        /// <summary>
        /// Types that will be added by default to the <see cref="RegisterTypes" />.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is immutable and want a field.")]
        public static readonly IReadOnlyCollection<Type> InternallyRequiredTypes = new[]
        {
            typeof(TypeDescription),
            typeof(SerializationDescription),
            typeof(DescribedSerialization),
            typeof(DynamicTypePlaceholder),
        };

        private readonly object syncConfigure = new object();
        private bool configured;

        private static readonly IReadOnlyCollection<Type> DiscoverAssignableTypesBlackList =
            new[]
            {
                typeof(string),
                typeof(object),
                typeof(ValueType),
                typeof(Enum),
                typeof(Array),
            };

        /// <summary>
        /// Gets the version of <see cref="RegisteredTypeToDetailsMap" />.
        /// </summary>
        protected Dictionary<Type, RegistrationDetails> MutableRegisteredTypeToDetailsMap { get; } = new Dictionary<Type, RegistrationDetails>();

        /// <summary>
        /// Gets a map of the the dependent configuration type (and any ancestors) to their configured instance.
        /// </summary>
        public IReadOnlyDictionary<Type, SerializationConfigurationBase> DependentConfigurationTypeToInstanceMap { get; private set; }

        /// <summary>
        /// Gets a map of registration details keyed on type registered.
        /// </summary>
        public IReadOnlyDictionary<Type, RegistrationDetails> RegisteredTypeToDetailsMap => this.MutableRegisteredTypeToDetailsMap;

        /// <summary>
        /// Run configuration logic.
        /// </summary>
        /// <param name="dependentConfigurationTypeToInstanceMap">Map of dependent configuration type to configured instance.</param>
        public virtual void Configure(IReadOnlyDictionary<Type, SerializationConfigurationBase> dependentConfigurationTypeToInstanceMap)
        {
            if (!this.configured)
            {
                lock (this.syncConfigure)
                {
                    if (!this.configured)
                    {
                        // Configure dependency map.
                        this.DependentConfigurationTypeToInstanceMap = dependentConfigurationTypeToInstanceMap;
                        foreach (var dependentConfigurationTypeToInstance in dependentConfigurationTypeToInstanceMap)
                        {
                            var dependentConfigType = dependentConfigurationTypeToInstance.Key;
                            var dependentConfigInstance = dependentConfigurationTypeToInstance.Value;

                            var registrationDetails = new RegistrationDetails(dependentConfigType);

                            var dependentConfigRegisteredTypes = dependentConfigInstance
                                .RegisteredTypeToDetailsMap
                                .Where(_ => _.Value.RegisteringType == dependentConfigType).ToList();

                            foreach (var dependentConfigRegisteredType in dependentConfigRegisteredTypes)
                            {
                                this.MutableRegisteredTypeToDetailsMap.Add(
                                    dependentConfigRegisteredType.Key,
                                    registrationDetails);
                            }
                        }

                        // Save locals to work with.
                        var localClassTypesToRegister = this.ClassTypesToRegister ?? new List<Type>();
                        var localInterfaceTypesToRegisterImplementationOf = this.InterfaceTypesToRegisterImplementationOf ?? new List<Type>();
                        var localTypesToAutoRegisterWithDiscovery = this.TypesToAutoRegisterWithDiscovery ?? new List<Type>();
                        var localTypesToAutoRegister = this.TypesToAutoRegister ?? new List<Type>();
                        var localClassTypesToRegisterAlongWithInheritors = this.ClassTypesToRegisterAlongWithInheritors ?? new List<Type>();

                        // Basic assertions.
                        this.DependentConfigurationTypeToInstanceMap.Must().NotBeNull();
                        localInterfaceTypesToRegisterImplementationOf.Must().NotContainAnyNullElements();
                        localTypesToAutoRegisterWithDiscovery.Must().NotContainAnyNullElements();
                        localTypesToAutoRegister.Must().NotContainAnyNullElements();
                        localClassTypesToRegisterAlongWithInheritors.Must().NotContainAnyNullElements();

                        localClassTypesToRegisterAlongWithInheritors.Select(_ => _.IsClass).Named(Invariant($"{nameof(this.ClassTypesToRegisterAlongWithInheritors)}.Select(_ => _.{nameof(Type.IsClass)})")).Must().Each().BeTrue();
                        localInterfaceTypesToRegisterImplementationOf.Select(_ => _.IsInterface).Named(Invariant($"{nameof(this.InterfaceTypesToRegisterImplementationOf)}.Select(_ => _.{nameof(Type.IsInterface)})")).Must().Each().BeTrue();

                        // Run optional initial config for the last inheritor.
                        this.InitialConfiguration();

                        // Run inheritor specific setup logic (like custom third party serializers/converters/resolvers).
                        this.InternalConfigure();

                        // Accumulate all possible types from configuration.
                        var discoveredTypes = this.DiscoverAllContainedAssignableTypes(localTypesToAutoRegisterWithDiscovery);

                        var typesToAutoRegister = new Type[0]
                            .Concat(localClassTypesToRegister)
                            .Concat(localTypesToAutoRegister)
                            .Concat(localInterfaceTypesToRegisterImplementationOf)
                            .Concat(localClassTypesToRegisterAlongWithInheritors)
                            .Concat(localTypesToAutoRegisterWithDiscovery)
                            .Concat(discoveredTypes)
                            .ToList();

                        var typesToAutoRegisterAlreadyHandledByDependentConfiguration = typesToAutoRegister.Intersect(this.RegisteredTypeToDetailsMap.Keys).ToList();
                        if (typesToAutoRegisterAlreadyHandledByDependentConfiguration.Any())
                        {
                            // TODO: what info do we want to capture here?
                            throw new DuplicateRegistrationException("Attempted to register types that are already registered", typesToAutoRegisterAlreadyHandledByDependentConfiguration);
                        }

                        var allTypesToRegister =
                            DiscoverAllAssignableTypes(typesToAutoRegister)
                                .Concat(discoveredTypes)
                                .Distinct()
                                .ToList();

                        // Register all types with inheritors to do additional configuration.
                        this.RegisterTypes(allTypesToRegister);

                        // Run optional final config for the last inheritor.
                        this.FinalConfiguration();

                        this.configured = true;
                    }
                }
            }
        }

        private IReadOnlyCollection<Type> DiscoverAllContainedAssignableTypes(IReadOnlyCollection<Type> types)
        {
            var typeHashSet = new HashSet<Type>();
            var typeSeen = new HashSet<Type>();
            var typesToInspect = new HashSet<Type>(types);

            bool FilterToUsableTypes(MemberInfo memberInfo) => !memberInfo.CustomAttributes.Select(s => s.AttributeType).Contains(typeof(CompilerGeneratedAttribute));

            while (typesToInspect.Any())
            {
                var type = typesToInspect.First();
                typesToInspect.Remove(type);

                if (typeSeen.Contains(type) || this.RegisteredTypeToDetailsMap.ContainsKey(type))
                {
                    continue;
                }

                typeSeen.Add(type);

                if (type.IsGenericType)
                {
                    type.GetGenericArguments().ToList().ForEach(_ => typesToInspect.Add(_));
                }

                if (type.IsArray)
                {
                    typesToInspect.Add(type.GetElementType());
                    continue;
                }

                if (type.Namespace?.StartsWith(nameof(System), StringComparison.Ordinal) ?? true)
                {
                    continue;
                }

                typeHashSet.Add(type);

                var assignableTypesToAdd = DiscoverAllAssignableTypes(new[] { type });

                var memberTypesToAdd = type.GetMembers(DiscoveryBindingFlags).Where(FilterToUsableTypes).SelectMany(
                    _ =>
                    {
                        if (_ is PropertyInfo propertyInfo)
                        {
                            return new[] { propertyInfo.PropertyType };
                        }
                        else if (_ is FieldInfo fieldInfo)
                        {
                            return new[] { fieldInfo.FieldType };
                        }
                        else
                        {
                            return new Type[0];
                        }
                    }).Where(_ => !_.IsGenericParameter).ToList();

                var newTypes = assignableTypesToAdd.Concat(memberTypesToAdd).ToList();

                newTypes.Distinct().ToList().ForEach(_ => typesToInspect.Add(_));
            }

            var result = typeHashSet.Where(_ => _.IsAssignableType()).ToList();
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
                        (!DiscoverAssignableTypesBlackList.Contains(typeToConsider)) &&
                        (!typeToConsider
                            .IsGenericTypeDefinition) && // can't do an IsAssignableTo check on generic type definitions
                        types.Any(typeToAutoRegister => typeToConsider.IsAssignableTo(typeToAutoRegister) || typeToAutoRegister.IsAssignableTo(typeToConsider)))
                .Concat(types.Where(_ => _.IsInterface)) // add interfaces back as they were explicitly provided.
                .ToList();

            return classTypesToRegister;
        }

        /// <summary>
        /// Discover all types that should considered for registration when looking for derivative types.
        /// </summary>
        /// <returns>All types that should be considered for registration.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Want this to be a method since it's running logic.")]
        protected static IReadOnlyCollection<Type> GetAllTypesToConsiderForRegistration()
        {
            return AssemblyLoader.GetLoadedAssemblies().GetTypesFromAssemblies();
        }

        /// <summary>
        /// Gets a list of <see cref="SerializationConfigurationBase"/>'s that are needed for the current implementation of <see cref="SerializationConfigurationBase"/>.  Optionally overrideable, DEFAULT is empty collection.
        /// </summary>
        public virtual IReadOnlyCollection<Type> DependentConfigurationTypes => new Type[0];

        /// <summary>
        /// Gets the dependent configurations that encompasses any necessary internal types, this should be used by first level inheritor and sealed.
        /// </summary>
        /// <returns>Configurations necessary to accomodate internal types.</returns>
        public abstract IReadOnlyCollection<Type> InternalDependentConfigurationTypes { get; }

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
    /// Generic implementation of <see cref="SerializationConfigurationBase" /> that will depend on config using type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Type to auto register with discovery.</typeparam>
    public sealed class GenericDependencyConfiguration<T> : SerializationConfigurationBase
        where T : SerializationConfigurationBase
    {
        /// <inheritdoc />
        public override IReadOnlyCollection<Type> DependentConfigurationTypes => new[] { typeof(T) };

        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            var registrationDetails = new RegistrationDetails(this.GetType());
            foreach (var type in types ?? new Type[0])
            {
                this.MutableRegisteredTypeToDetailsMap.Add(type, registrationDetails);
            }
        }

        /// <inheritdoc />
        public sealed override IReadOnlyCollection<Type> InternalDependentConfigurationTypes => new Type[0];
    }

    /// <summary>
    /// Generic implementation of <see cref="SerializationConfigurationBase" /> that will depend on config using type <typeparamref name="T1" />, <typeparamref name="T2" />.
    /// </summary>
    /// <typeparam name="T1">Type one to auto register with discovery.</typeparam>
    /// <typeparam name="T2">Type two to auto register with discovery.</typeparam>
    public sealed class GenericDependencyConfiguration<T1, T2> : SerializationConfigurationBase
        where T1 : SerializationConfigurationBase
        where T2 : SerializationConfigurationBase
    {
        /// <inheritdoc />
        public override IReadOnlyCollection<Type> DependentConfigurationTypes => new[] { typeof(T1), typeof(T2) };

        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            var registrationDetails = new RegistrationDetails(this.GetType());
            foreach (var type in types ?? new Type[0])
            {
                this.MutableRegisteredTypeToDetailsMap.Add(type, registrationDetails);
            }
        }

        /// <inheritdoc />
        public sealed override IReadOnlyCollection<Type> InternalDependentConfigurationTypes => new Type[0];
    }

    /// <summary>
    /// Null implementation of <see cref="SerializationConfigurationBase"/>.
    /// </summary>
    public sealed class NullSerializationConfiguration : SerializationConfigurationBase, IImplementNullObjectPattern
    {
        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            var registrationDetails = new RegistrationDetails(this.GetType());
            foreach (var type in types ?? new Type[0])
            {
                this.MutableRegisteredTypeToDetailsMap.Add(type, registrationDetails);
            }
        }

        /// <inheritdoc />
        public sealed override IReadOnlyCollection<Type> InternalDependentConfigurationTypes => new Type[0];
    }

    /// <summary>
    /// Internal implementation of <see cref="SerializationConfigurationBase" /> that will auto register necessary internal types.
    /// </summary>
    public sealed class InternalNullDiscoveryConfiguration : SerializationConfigurationBase, IDoNotNeedInternalDependencies, IImplementNullObjectPattern
    {
        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            var registrationDetails = new RegistrationDetails(this.GetType());
            foreach (var type in types ?? new Type[0])
            {
                this.MutableRegisteredTypeToDetailsMap.Add(type, registrationDetails);
            }
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegisterWithDiscovery => InternallyRequiredTypes;

        /// <inheritdoc />
        public override IReadOnlyCollection<Type> InternalDependentConfigurationTypes => new Type[0];
    }

    /// <summary>
    /// Generic implementation of <see cref="SerializationConfigurationBase" /> that will perform discovery using type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Type to use for discovery.</typeparam>
    public sealed class NullDiscoverySerializationConfiguration<T> : SerializationConfigurationBase, IImplementNullObjectPattern
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegisterWithDiscovery => new[] { typeof(T) };

        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            var registrationDetails = new RegistrationDetails(this.GetType());
            foreach (var type in types ?? new Type[0])
            {
                this.MutableRegisteredTypeToDetailsMap.Add(type, registrationDetails);
            }
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<Type> InternalDependentConfigurationTypes => new[] { typeof(InternalNullDiscoveryConfiguration) };
    }

    /// <summary>
    /// Generic implementation of <see cref="SerializationConfigurationBase" /> that will perform discovery using type <typeparamref name="T1" />, <typeparamref name="T2" />.
    /// </summary>
    /// <typeparam name="T1">Type one to use for discovery.</typeparam>
    /// <typeparam name="T2">Type two to use for discovery.</typeparam>
    public sealed class NullDiscoverySerializationConfiguration<T1, T2> : SerializationConfigurationBase, IImplementNullObjectPattern
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegisterWithDiscovery => new[] { typeof(T1), typeof(T2) };

        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            var registrationDetails = new RegistrationDetails(this.GetType());
            foreach (var type in types ?? new Type[0])
            {
                this.MutableRegisteredTypeToDetailsMap.Add(type, registrationDetails);
            }
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<Type> InternalDependentConfigurationTypes => new Type[0];
    }

    /// <summary>
    /// Interface to declare that it does not require the internal dependency configuration.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Prefer an interface for this instead of an attribute.")]
    public interface IDoNotNeedInternalDependencies
    {
    }
}