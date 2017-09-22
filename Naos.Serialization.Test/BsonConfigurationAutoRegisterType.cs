// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonConfigurationAutoRegisterType.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using MongoDB.Bson.Serialization;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;

    public class BsonConfigurationAutoRegisterType<T> : BsonConfigurationBase
    {
        protected override IReadOnlyCollection<Type> TypesToAutoRegister => new[] { typeof(T) };
    }

    public class BsonConfigurationTestAutoConstrainedType : BsonConfigurationBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Only used in testing.")]
        public BsonConfigurationTestAutoConstrainedType(Type type, IReadOnlyCollection<string> constrainedProperties = null)
        {
            this.TypeToRegister = type;
            this.ConstrainedProperties = constrainedProperties;
        }

        public Type TypeToRegister { get; set; }

        public IReadOnlyCollection<string> ConstrainedProperties { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Is actually called by reflection.")]
        private BsonClassMap TestCode()
        {
            // this is just a hook to test this...
            return this.AutomaticallyBuildBsonClassMap(this.TypeToRegister, this.ConstrainedProperties);
        }
    }

    public class CustomThrowsConfig : BsonConfigurationBase
    {
        /// <summary>
        /// Gets the exception message being thrown.
        /// </summary>
        public const string ExceptionMessage = "Expected to be thrown.";

        protected override void CustomConfiguration()
        {
            throw new ArgumentException(ExceptionMessage);
        }
    }

    public class TestConfigWithSettableFields : BsonConfigurationBase
    {
#pragma warning disable SA1401 // Fields should be private
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        public IReadOnlyCollection<Type> SettableClassTypesToRegister = new Type[0];

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        public IReadOnlyCollection<Type> SettableTypesToAutoRegister = new Type[0];

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        public IReadOnlyCollection<Type> SettableClassTypesToRegisterAlongWithInheritors = new Type[0];

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        public IReadOnlyCollection<Type> SettableInterfaceTypesToRegisterImplementationOf = new Type[0];

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        public IReadOnlyCollection<Type> SettableDependentConfigurationTypes = new Type[0];

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        public TrackerCollisionStrategy SettableTypeTrackerCollisionStrategy = TrackerCollisionStrategy.Skip;

#pragma warning restore SA1401 // Fields should be private

        protected override IReadOnlyCollection<Type> ClassTypesToRegister => this.SettableClassTypesToRegister;

        protected override IReadOnlyCollection<Type> TypesToAutoRegister => this.SettableTypesToAutoRegister;

        protected override IReadOnlyCollection<Type> ClassTypesToRegisterAlongWithInheritors => this.SettableClassTypesToRegisterAlongWithInheritors;

        protected override IReadOnlyCollection<Type> InterfaceTypesToRegisterImplementationOf => this.SettableInterfaceTypesToRegisterImplementationOf;

        protected override IReadOnlyCollection<Type> DependentConfigurationTypes => this.SettableDependentConfigurationTypes;

        protected override TrackerCollisionStrategy TypeTrackerCollisionStrategy => this.SettableTypeTrackerCollisionStrategy;
    }
}
