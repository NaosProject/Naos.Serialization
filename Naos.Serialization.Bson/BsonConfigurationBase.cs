﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonConfigurationBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Options;
    using MongoDB.Bson.Serialization.Serializers;

    using Naos.Serialization.Domain;

    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.TypeRepresentation;
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Base class to use for creating <see cref="NaosBsonSerializer" /> configuration.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class is not excessively coupled for the nature of the problem.")]
    public abstract class BsonConfigurationBase
    {
        /// <summary>
        /// Binding flags used in <see cref="GetMembersToAutomap"/> to reflect on a type.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Name is correct.")]
        public const BindingFlags BsonMemberAutomapSelectionBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        /// <summary>
        /// Default member name to use as the ID.
        /// </summary>
        public const string DefaultIdMemberName = "Id";

        private const string RegisterClassMapMethodName = nameof(BsonClassMap.RegisterClassMap);

        private static readonly MethodInfo RegisterClassMapGenericMethod = typeof(BsonClassMap).GetMethods().Single(_ => (_.Name == RegisterClassMapMethodName) && (!_.GetParameters().Any()) && _.IsGenericMethod);

        private static readonly Tracker<Type> TypeTracker = new Tracker<Type>((first, second) =>
                {
                    if (ReferenceEquals(first, second))
                    {
                        return true;
                    }

                    if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
                    {
                        return false;
                    }

                    return first == second;
                });

        private readonly object syncConfigure = new object();

        private bool configured;

        /// <summary>
        /// Run configuration logic.
        /// </summary>
        public void Configure()
        {
            if (!this.configured)
            {
                lock (this.syncConfigure)
                {
                    if (!this.configured)
                    {
                        new { this.TypeToCustomSerializerMap }.Must().NotBeNull();
                        new { this.DependentConfigurationTypes }.Must().NotBeNull();
                        new { this.TypesToAutoRegister }.Must().NotBeNull();
                        new { this.ClassTypesToRegister }.Must().NotBeNull();
                        new { this.ClassTypesToRegisterAlongWithInheritors }.Must().NotBeNull();
                        new { this.InterfaceTypesToRegisterImplementationOf }.Must().NotBeNull();

                        this.ClassTypesToRegisterAlongWithInheritors.Select(_ => _.IsClass).Named(Invariant($"{nameof(this.ClassTypesToRegisterAlongWithInheritors)}.Select(_ => _.{nameof(Type.IsClass)})")).Must().Each().BeTrue();
                        this.InterfaceTypesToRegisterImplementationOf.Select(_ => _.IsInterface).Named(Invariant($"{nameof(this.InterfaceTypesToRegisterImplementationOf)}.Select(_ => _.{nameof(Type.IsInterface)})")).Must().Each().BeTrue();

                        foreach (var dependentConfigurationType in this.DependentConfigurationTypes)
                        {
                            BsonConfigurationManager.Configure(dependentConfigurationType);
                        }

                        foreach (var typeToCustomSerializer in this.TypeToCustomSerializerMap)
                        {
                            this.RegisterCustomSerializer(typeToCustomSerializer.Key, typeToCustomSerializer.Value);
                        }

                        this.RegisterClassTypes(this.ClassTypesToRegister);

                        var typesToAutoRegister = new Type[0]
                            .Concat(this.TypesToAutoRegister)
                            .Concat(this.InterfaceTypesToRegisterImplementationOf)
                            .Concat(this.ClassTypesToRegisterAlongWithInheritors)
                            .ToList();

                        this.RegisterAssignableClassTypes(typesToAutoRegister);

                        this.InternalConfiguration();

                        this.CustomConfiguration();

                        this.configured = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of <see cref="BsonConfigurationBase"/>'s that are needed for the current implementation of <see cref="BsonConfigurationBase"/>.  Optionally overrideable, DEFAULT is empty collection.
        /// </summary>
        protected virtual IReadOnlyCollection<Type> DependentConfigurationTypes => new Type[0];

        /// <summary>
        /// Gets a list of <see cref="Type"/>s to auto-register.
        /// Auto-registration is a convenient way to register types; it accounts for interface implementations and class inheritance when performing the registration.
        /// For interface types, all implementations will be also be registered.  For classes, all inheritors will also be registered.  These additional types do not need to be specified.
        /// </summary>
        protected virtual IReadOnlyCollection<Type> TypesToAutoRegister => new Type[0];

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
        /// Gets a map of <see cref="Type"/> to the <see cref="IBsonSerializer"/> to register.
        /// </summary>
        protected virtual IReadOnlyDictionary<Type, IBsonSerializer> TypeToCustomSerializerMap => new Dictionary<Type, IBsonSerializer>();

        /// <summary>
        /// Gets an optional <see cref="TrackerCollisionStrategy" /> to use when a <see cref="Type" /> is already registered; DEFAULT is <see cref="TrackerCollisionStrategy.Skip" />.
        /// </summary>
        protected virtual TrackerCollisionStrategy TypeTrackerCollisionStrategy => TrackerCollisionStrategy.Skip;

        /// <summary>
        /// Optional template method to override and specify custom logic, usually used for specific direct <see cref="MongoDB.Bson"/> calls.
        /// </summary>
        protected virtual void CustomConfiguration()
        {
            /* no-op - just for additional custom logic */
        }

        private void InternalConfiguration()
        {
            this.RegisterClassType<TypeDescription>();
            this.RegisterClassType<SerializationDescription>();
            this.RegisterClassType<DescribedSerialization>();
        }

        /// <summary>
        /// Method to register new custom serializers.
        /// </summary>
        /// <param name="typeToUseSerializerFor">Type that should use provided serializer.</param>
        /// <param name="customSerializer">Serializer implementation to use.</param>
        protected void RegisterCustomSerializer(Type typeToUseSerializerFor, IBsonSerializer customSerializer)
        {
            BsonSerializer.RegisterSerializer(typeToUseSerializerFor, customSerializer);
        }

        /// <summary>
        /// Method to call <see cref="BsonClassMap.RegisterClassMap{TClass}()"/> using a <see cref="Type"/> parameter instead of the generic.
        /// </summary>
        /// <param name="type">Type to register.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassMapForTypeUsingMongoGeneric(Type type)
        {
            new { type }.Must().NotBeNull();

            try
            {
                void TrackedOperation(Type typeToOperateOn)
                {
                    var genericRegisterClassMapMethod = RegisterClassMapGenericMethod.MakeGenericMethod(typeToOperateOn);
                    genericRegisterClassMapMethod.Invoke(null, null);
                }

                TypeTracker.RunTrackedOperation(type, TrackedOperation, this.TypeTrackerCollisionStrategy, this.GetType());
            }
            catch (Exception ex)
            {
                throw new BsonConfigurationException(Invariant($"Failed to run {nameof(BsonClassMap.RegisterClassMap)} on {type.FullName}"), ex);
            }
        }

        /// <summary>
        /// Method to perform automatic type member mapping using specific internal conventions.
        /// </summary>
        /// <param name="type">Type to register.</param>
        /// <param name="constrainToProperties">Optional list of properties to constrain type members to (null or 0 will mean all).</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Like this structure.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassType(Type type, IReadOnlyCollection<string> constrainToProperties = null)
        {
            new { type }.Must().NotBeNull();
            type.IsClass.Named(Invariant($"{nameof(type)}.{nameof(Type.IsClass)}")).Must().BeTrue();

            try
            {
                void TrackedOperation(Type typeToOperateOn)
                {
                    var bsonClassMap = this.AutomaticallyBuildBsonClassMap(typeToOperateOn, constrainToProperties);
                    BsonClassMap.RegisterClassMap(bsonClassMap);
                }

                TypeTracker.RunTrackedOperation(type, TrackedOperation, this.TypeTrackerCollisionStrategy, this.GetType());
            }
            catch (Exception ex)
            {
                throw new BsonConfigurationException(Invariant($"Failed to run {RegisterClassMapMethodName} on {type.FullName}"), ex);
            }
        }

        /// <summary>
        /// Method to perform automatic type member mapping using specific internal conventions.
        /// </summary>
        /// <typeparam name="T">Type to register.</typeparam>
        /// <param name="constrainToProperties">Optional list of properties to constrain type members to (null or 0 will mean all).</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Want to use this as a generic.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Like this structure.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassType<T>(IReadOnlyCollection<string> constrainToProperties = null)
        {
            this.RegisterClassType(typeof(T), constrainToProperties);
        }

        /// <summary>
        /// Method to run <see cref="RegisterClassType"/> on each provided type.
        /// </summary>
        /// <param name="types">Types to register.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassTypes(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull();

            foreach (var type in types)
            {
                this.RegisterClassType(type);
            }
        }

        /// <summary>
        /// Method to register the specified type and class types that are assignable to those specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterAssignableClassTypes(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull();

            var allTypesToConsiderForRegistration = AssemblyLoader.GetLoadedAssemblies().GetTypesFromAssemblies();

            var classTypesToRegister = allTypesToConsiderForRegistration
                .Where(
                    typeToConsider =>
                        typeToConsider.IsClass &&
                        (!typeToConsider.IsAnonymous()) &&
                        (!typeToConsider.IsGenericTypeDefinition) && // can't do an IsAssignableTo check on generic type definitions
                        types.Any(typeToAutoRegister => typeToConsider.IsAssignableTo(typeToAutoRegister)))
                .ToList();

            if (classTypesToRegister.Any())
            {
                this.RegisterClassTypes(classTypesToRegister);
            }
        }

        /// <summary>
        /// Configures a <see cref="BsonClassMap"/> automatically based on the members of the provided type.
        /// </summary>
        /// <param name="type">Type to register.</param>
        /// <param name="constrainToProperties">Optional list of properties to constrain type members to (null or 0 will mean all).</param>
        /// <returns>Configured <see cref="BsonClassMap"/>.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Like this structure.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected BsonClassMap AutomaticallyBuildBsonClassMap(Type type, IReadOnlyCollection<string> constrainToProperties)
        {
            new { type }.Must().NotBeNull();

            var bsonClassMap = new BsonClassMap(type);

            var constraintsAreNullOrEmpty = constrainToProperties == null || constrainToProperties.Count == 0;

            var allMembers = GetMembersToAutomap(type);

            var members = allMembers.Where(_ => constraintsAreNullOrEmpty || constrainToProperties.Contains(_.Name)).ToList();

            if (!constraintsAreNullOrEmpty)
            {
                var allMemberNames = allMembers.Select(_ => _.Name).ToList();
                constrainToProperties.Any(_ => !allMemberNames.Contains(_)).Named("constrainedPropertyDoesNotExistOnType").Must().BeFalse();
            }

            foreach (var member in members)
            {
                var memberType = member.GetUnderlyingType();
                memberType.Named(Invariant($"{member.Name}-{nameof(MemberInfo.DeclaringType)}")).Must().NotBeNull();

                try
                {
                    var memberMap = MapMember(bsonClassMap, member);

                    var serializer = GetSerializer(memberType, defaultToObjectSerializer: false);
                    if (serializer != null)
                    {
                        memberMap.SetSerializer(serializer);
                    }
                }
                catch (Exception ex)
                {
                    throw new BsonConfigurationException(Invariant($"Error automatically mapping; type: {type}, member: {member}"), ex);
                }
            }

            return bsonClassMap;
        }

        private static BsonMemberMap MapMember(BsonClassMap bsonClassMap, MemberInfo member)
        {
            if (DefaultIdMemberName.Equals(member.Name, StringComparison.OrdinalIgnoreCase))
            {
                // TODO: add logic to make sure ID is of acceptable type here...
                return bsonClassMap.MapIdMember(member);
            }
            else
            {
                return bsonClassMap.MapMember(member);
            }
        }

        /// <summary>
        /// Gets the serializer to use for a given type.
        /// </summary>
        /// <param name="type">The type to serialize.</param>
        /// <param name="defaultToObjectSerializer">Optional.  If true (DEFAULT), then returns <see cref="ObjectSerializer"/> when the serializer cannot be determined.  Otherwise, returns null.</param>
        /// <returns>
        /// The serializer to use for the specified type.
        /// </returns>
        internal static IBsonSerializer GetSerializer(Type type, bool defaultToObjectSerializer = true)
        {
            IBsonSerializer result;
            if (type == typeof(string))
            {
                result = new StringSerializer();
            }
            else if (type == typeof(Guid))
            {
                result = new GuidSerializer();
            }
            else if (type.IsEnum)
            {
                result = typeof(NaosBsonEnumStringSerializer<>).MakeGenericType(type).Construct<IBsonSerializer>();
            }
            else if (type == typeof(DateTime))
            {
                result = new NaosBsonDateTimeSerializer();
            }
            else if (type == typeof(DateTime?))
            {
                result = new NaosBsonNullableDateTimeSerializer();
            }
            else if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var elementSerializer = GetSerializer(elementType, defaultToObjectSerializer: false);
                result = elementSerializer == null
                    ? typeof(ArraySerializer<>).MakeGenericType(elementType).Construct<IBsonSerializer>()
                    : typeof(ArraySerializer<>).MakeGenericType(elementType)
                        .Construct<IBsonSerializer>(elementSerializer);
            }
            else if (type.IsGenericType && NullNaosDictionarySerializer.IsSupportedUnboundedGenericDictionaryType(type.GetGenericTypeDefinition()))
            {
                var arguments = type.GetGenericArguments();
                var keyType = arguments[0];
                var valueType = arguments[1];
                var keySerializer = GetSerializer(keyType);
                var valueSerializer = GetSerializer(valueType);
                result = typeof(NaosDictionarySerializer<,,>).MakeGenericType(type, keyType, valueType).Construct<IBsonSerializer>(DictionaryRepresentation.ArrayOfDocuments, keySerializer, valueSerializer);
            }
            else if (type.IsGenericType && NullNaosCollectionSerializer.IsSupportedUnboundedGenericCollectionType(type.GetGenericTypeDefinition()))
            {
                var arguments = type.GetGenericArguments();
                var elementType = arguments[0];
                var elementSerializer = GetSerializer(elementType, defaultToObjectSerializer: false);
                result = typeof(NaosCollectionSerializer<,>).MakeGenericType(type, elementType).Construct<IBsonSerializer>(elementSerializer);
            }
            else
            {
                result = defaultToObjectSerializer ? new ObjectSerializer() : null;
            }

            return result;
        }

        /// <summary>
        /// Gets the <see cref="MemberInfo"/>'s to use for auto mapping.
        /// </summary>
        /// <param name="type">Type to interrogate.</param>
        /// <returns>Collection of members to map.</returns>
        public static IReadOnlyCollection<MemberInfo> GetMembersToAutomap(Type type)
        {
            new { type }.Must().NotBeNull();

            bool FilterCompilerGenerated(MemberInfo memberInfo) => !memberInfo.CustomAttributes.Select(s => s.AttributeType).Contains(typeof(CompilerGeneratedAttribute));

            var allMembers = type.GetMembers(BsonMemberAutomapSelectionBindingFlags).Where(FilterCompilerGenerated).ToList();

            var fields = allMembers
                .Where(_ => _.MemberType == MemberTypes.Field)
                .Cast<FieldInfo>()
                .Where(_ => !_.IsInitOnly)
                .ToList();

            const bool returnIfSetMethodIsNotPublic = true;
            var properties = allMembers
                .Where(_ => _.MemberType == MemberTypes.Property)
                .Cast<PropertyInfo>()
                .Where(_ => _.CanWrite || _.GetSetMethod(returnIfSetMethodIsNotPublic) != null)
                .ToList();

            return new MemberInfo[0].Concat(fields).Concat(properties).ToList();
        }

        /// <summary>
        /// Gets all the types from this and any other <see cref="BsonConfigurationBase"/> derivative that have been registered.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want this accessible via the object.")]
        public IReadOnlyCollection<Type> AllRegisteredTypes => TypeTracker.GetAllTrackedObjects().Select(_ => _.TrackedObject).ToList();

        /// <summary>
        /// Gets all the types in their wrapped form with telemetry from this and any other <see cref="BsonConfigurationBase"/> derivative that have been registered.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want this accessible via the object.")]
        public IReadOnlyCollection<Tracker<Type>.TrackedObjectContainer> AllTrackedTypeContainers => TypeTracker.GetAllTrackedObjects();
    }

    /// <summary>
    /// Null implementation of <see cref="BsonConfigurationBase"/>.
    /// </summary>
    public sealed class NullBsonConfiguration : BsonConfigurationBase
    {
        /// <inheritdoc />
        protected override void CustomConfiguration()
        {
            /* no-op */
        }
    }
}