// --------------------------------------------------------------------------------------------------------------------
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
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Base class to use for creating <see cref="NaosBsonSerializer" /> configuration.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class is not excessively coupled for the nature of the problem.")]
    public abstract class BsonConfigurationBase : SerializationConfigurationBase
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

        /// <summary>
        /// All registered types (static and used by all configurations to accomodate the fact that you have dependent configs that are only run once).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is immutable.")]
        protected static readonly Tracker<Type> RegisteredTypesTracker = new Tracker<Type>((a, b) => a == b);

        /// <summary>
        /// Gets a map of <see cref="Type"/> to the <see cref="IBsonSerializer"/> to register.
        /// </summary>
        protected virtual IReadOnlyDictionary<Type, IBsonSerializer> TypeToCustomSerializerMap => new Dictionary<Type, IBsonSerializer>();

        /// <summary>
        /// Gets all the types from this and any other <see cref="SerializationConfigurationBase"/> derivative that have been registered.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want this accessible via the object.")]
        public IReadOnlyCollection<Type> AllRegisteredTypes => RegisteredTypesTracker.GetAllTrackedObjects();

        /// <summary>
        /// Gets all the types in their wrapped form with telemetry from this and any other <see cref="SerializationConfigurationBase"/> derivative that have been registered.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want this accessible via the object.")]
        public IReadOnlyCollection<Tracker<Type>.TrackedObjectContainer> AllTrackedTypeContainers => RegisteredTypesTracker.GetAllTrackedObjectContainers();

        /// <inheritdoc />
        protected override void InternalConfigure()
        {
            foreach (var typeToCustomSerializer in this.TypeToCustomSerializerMap ?? new Dictionary<Type, IBsonSerializer>())
            {
                this.RegisterCustomSerializer(typeToCustomSerializer.Key, typeToCustomSerializer.Value);
            }
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
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.' to make sure type is tracked correctly.")]
        protected void RegisterClassMapForTypeUsingMongoGeneric(Type type)
        {
            new { type }.Must().NotBeNull();

            try
            {
                // this should be tracked since it's side band to the normal way types are identified and therefore needs to maintain consistent list of registered types.
                void TrackedOperation(Type localType)
                {
                    var genericRegisterClassMapMethod = RegisterClassMapGenericMethod.MakeGenericMethod(localType);
                    genericRegisterClassMapMethod.Invoke(null, null);
                }

                RegisteredTypesTracker.RunTrackedOperation(type, TrackedOperation, this.TypeTrackerCollisionStrategy, this.GetType());
            }
            catch (Exception ex)
            {
                throw new BsonConfigurationException(Invariant($"Failed to run {nameof(BsonClassMap.RegisterClassMap)} on {type.FullName}"), ex);
            }
        }

        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull();

            foreach (var type in types)
            {
                this.RegisterTypeWithPropertyConstraints(type);
            }
        }

        /// <summary>
        /// Method to perform automatic type member mapping using specific internal conventions.
        /// </summary>
        /// <param name="type">Type to register.</param>
        /// <param name="constrainToProperties">Optional list of properties to constrain type members to (null or 0 will mean all).</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Like this structure.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterTypeWithPropertyConstraints(Type type, IReadOnlyCollection<string> constrainToProperties = null)
        {
            new { type }.Must().NotBeNull();

            try
            {
                if (type.IsClass)
                {
                    void TrackedOperation(Type localType)
                    {
                        var bsonClassMap = this.AutomaticallyBuildBsonClassMap(type, constrainToProperties);
                        BsonClassMap.RegisterClassMap(bsonClassMap);
                    }

                    RegisteredTypesTracker.RunTrackedOperation(type, TrackedOperation, this.TypeTrackerCollisionStrategy, this.GetType());
                }
            }
            catch (Exception ex)
            {
                throw new BsonConfigurationException(Invariant($"Failed to run {nameof(BsonClassMap.RegisterClassMap)} on {type.FullName}"), ex);
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
        protected BsonClassMap AutomaticallyBuildBsonClassMap(Type type, IReadOnlyCollection<string> constrainToProperties = null)
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

                    var serializer = GetAppropriateSerializer(memberType, defaultToObjectSerializer: false);
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
        internal static IBsonSerializer GetAppropriateSerializer(Type type, bool defaultToObjectSerializer = true)
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
                var elementSerializer = GetAppropriateSerializer(elementType, defaultToObjectSerializer: false);
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
                var keySerializer = GetAppropriateSerializer(keyType);
                var valueSerializer = GetAppropriateSerializer(valueType);
                result = typeof(NaosDictionarySerializer<,,>).MakeGenericType(type, keyType, valueType).Construct<IBsonSerializer>(DictionaryRepresentation.ArrayOfDocuments, keySerializer, valueSerializer);
            }
            else if (type.IsGenericType && NullNaosCollectionSerializer.IsSupportedUnboundedGenericCollectionType(type.GetGenericTypeDefinition()))
            {
                var arguments = type.GetGenericArguments();
                var elementType = arguments[0];
                var elementSerializer = GetAppropriateSerializer(elementType, defaultToObjectSerializer: false);
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
    }

    /// <summary>
    /// Generic implementation of <see cref="BsonConfigurationBase" /> that will auto register with discovery using type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Type to auto register with discovery.</typeparam>
    public sealed class GenericBsonConfiguration<T> : BsonConfigurationBase
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegisterWithDiscovery => new[] { typeof(T) };
    }

    /// <summary>
    /// Generic implementation of <see cref="BsonConfigurationBase" /> that will auto register with discovery using type <typeparamref name="T1" />, <typeparamref name="T2" />.
    /// </summary>
    /// <typeparam name="T1">Type one to auto register with discovery.</typeparam>
    /// <typeparam name="T2">Type two to auto register with discovery.</typeparam>
    public sealed class GenericBsonConfiguration<T1, T2> : BsonConfigurationBase
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegisterWithDiscovery => new[] { typeof(T1), typeof(T2) };
    }

    /// <summary>
    /// Null implementation of <see cref="BsonConfigurationBase"/>.
    /// </summary>
    public sealed class NullBsonConfiguration : BsonConfigurationBase
    {
        /// <inheritdoc />
        protected override void RegisterTypes(IReadOnlyCollection<Type> types)
        {
            /* no-op */
        }
    }
}