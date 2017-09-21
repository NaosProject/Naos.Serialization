// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonConfigurationBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Options;
    using MongoDB.Bson.Serialization.Serializers;
    using OBeautifulCode.Reflection;
    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Base class to use for creating
    /// </summary>
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
                        foreach (var dependantMapperType in this.DependentMapperTypes)
                        {
                            BsonConfigurationManager.Configure(dependantMapperType);
                        }

                        this.CustomConfiguration();

                        this.configured = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of <see cref="BsonConfigurationBase"/>'s that are needed for the current implemenation of <see cref="BsonConfigurationBase"/>.  Optionally overrideable, DEFAULT is empty collection.
        /// </summary>
        protected virtual IReadOnlyCollection<Type> DependentMapperTypes => new Type[0];

        /// <summary>
        /// Template method to override and specify custom logic.
        /// </summary>
        protected abstract void CustomConfiguration();

        /// <summary>
        /// Method to perform automatic type member mapping using specific internal conventions.
        /// </summary>
        /// <param name="type">Type to register.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassMapForTypeUsingMongoGeneric(Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            try
            {
                var genericRegisterClassMapMethod = RegisterClassMapGenericMethod.MakeGenericMethod(type);
                genericRegisterClassMapMethod.Invoke(null, null);
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
        protected void RegisterClassMapForType(Type type, IReadOnlyCollection<string> constrainToProperties = null)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            try
            {
                var bsonClassMap = this.AutomaticallyBuildBsonClassMap(type, constrainToProperties);

                BsonClassMap.RegisterClassMap(bsonClassMap);
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
        protected void RegisterClassMapForType<T>(IReadOnlyCollection<string> constrainToProperties = null)
        {
            this.RegisterClassMapForType(typeof(T), constrainToProperties);
        }

        /// <summary>
        /// Method to perform automatic type member mapping using specific internal conventions.
        /// </summary>
        /// <param name="types">Types to register.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassMapForType(IReadOnlyCollection<Type> types)
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
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassMapForTypeAndSubclassTypes(IReadOnlyCollection<Type> types)
        {
            new { types }.Must().NotBeNull().OrThrowFirstFailure();

            var allTypes = types.SelectMany(_ => this.GetSubclassTypes(_, includeSpecifiedTypeInReturnList: true)).Distinct().ToList();

            this.RegisterClassMapForType(allTypes);
        }

        /// <summary>
        /// Method to register the specified type and all derivative types in the same assembly.
        /// </summary>
        /// <param name="type">Type to register.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassMapForTypeAndSubclassTypes(Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            this.RegisterClassMapForTypeAndSubclassTypes(new[] { type });
        }

        /// <summary>
        /// Method to register the specified type and all derivative types in the same assembly.
        /// </summary>
        /// <typeparam name="T">Type to register.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Want to use this as a generic.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected void RegisterClassMapForTypeAndSubclassTypes<T>()
        {
            this.RegisterClassMapForTypeAndSubclassTypes(typeof(T));
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
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            var bsonClassMap = new BsonClassMap(type);

            var constraintsAreNullOrEmpty = constrainToProperties == null || constrainToProperties.Count == 0;

            var allMembers = GetMembersToAutomap(type);

            var members = allMembers.Where(_ => constraintsAreNullOrEmpty || constrainToProperties.Contains(_.Name)).ToList();

            if (!constraintsAreNullOrEmpty)
            {
                var allMemberNames = allMembers.Select(_ => _.Name).ToList();
                constrainToProperties.Any(_ => !allMemberNames.Contains(_)).Named("constrainedPropertyDoesNotExistOnType").Must().BeFalse().OrThrowFirstFailure();
            }

            foreach (var member in members)
            {
                var memberType = member.GetUnderlyingType();
                memberType.Named(Invariant($"{member.Name}-{nameof(MemberInfo.DeclaringType)}")).Must().NotBeNull().OrThrowFirstFailure();

                try
                {
                    var memberMap = MapMember(bsonClassMap, member);
                    var serializer = GetSerializer(memberType);
                    memberMap.SetSerializer(serializer);
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

        internal static IBsonSerializer GetSerializer(Type type)
        {
            IBsonSerializer result;
            if (type == typeof(string))
            {
                result = new StringSerializer();
            }
            else if (type.IsEnum)
            {
                result = typeof(EnumSerializer<>).MakeGenericType(type).Construct<IBsonSerializer>(BsonType.String);
            }
            else if (type == typeof(DateTime))
            {
                result = new NaosBsonDateTimeSerializer();
            }
            else if (type == typeof(DateTime?))
            {
                result = new NaosBsonNullableDateTimeSerializer();
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
            else if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var elementSerializer = GetSerializer(type);
                result = typeof(ArraySerializer<>).MakeGenericType(elementType).Construct<IBsonSerializer>(elementSerializer);
            }
            else
            {
                result = new ObjectSerializer();
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
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            var allMembers = type.GetMembers(BsonMemberAutomapSelectionBindingFlags)
                .Where(_ => _.MemberType == MemberTypes.Field || _.MemberType == MemberTypes.Property)
                .Where(_ => !_.CustomAttributes.Select(s => s.AttributeType).Contains(typeof(CompilerGeneratedAttribute))).ToList();
            return allMembers;
        }

        /// <summary>
        /// Get a list of the subclass types of the provided type and the provided type if <paramref name="includeSpecifiedTypeInReturnList"/> is true.
        /// </summary>
        /// <param name="classType">Type to find derivatives of.</param>
        /// <param name="includeSpecifiedTypeInReturnList">Optional value indicating whether or not to include the provided type in the return list; DEFAULT is true.</param>
        /// <returns>List of the subclass types of the provided type and the provided type if <paramref name="includeSpecifiedTypeInReturnList"/> is true.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Want to be used from derivatives using 'this.'")]
        protected IReadOnlyCollection<Type> GetSubclassTypes(Type classType, bool includeSpecifiedTypeInReturnList = false)
        {
            new { classType }.Must().NotBeNull().OrThrowFirstFailure();
            new { classType.IsClass }.Must().BeTrue().OrThrowFirstFailure();

            var derivativeTypes = classType.Assembly.GetTypes().Where(_ => _.IsSubclassOf(classType)).ToList();

            if (includeSpecifiedTypeInReturnList)
            {
                derivativeTypes.Add(classType);
            }

            return derivativeTypes.Distinct().ToList();
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