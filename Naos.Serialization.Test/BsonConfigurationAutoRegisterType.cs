// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonConfigurationAutoRegisterType.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization;

    using Naos.Serialization.Bson;

    public class BsonConfigurationAutoRegisterType<T> : BsonConfigurationBase
    {
        /// <inheritdoc cref="BsonConfigurationBase" />
        protected override void CustomConfiguration()
        {
            this.RegisterClassMapForTypeAndSubclassTypes<T>();
        }
    }

    public class BsonConfigurationAutoRegisterInherited : BsonConfigurationBase
    {
        /// <inheritdoc cref="BsonConfigurationBase" />
        protected override void CustomConfiguration()
        {
            this.RegisterClassMapForTypeAndSubclassTypes<TestWithInheritorExtraPropertyWrapper>();
            this.RegisterClassMapForTypeAndSubclassTypes<TestWithInheritor>();
            this.RegisterClassMapForTypeAndSubclassTypes<TestWithInheritorExtraProperty>();
        }
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

        /// <inheritdoc cref="BsonConfigurationBase" />
        protected override void CustomConfiguration()
        {
            throw new NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Is actually called by reflection.")]
        private BsonClassMap TestCode()
        {
            // this is just a hook to test this...
            return this.AutomaticallyBuildBsonClassMap(this.TypeToRegister, this.ConstrainedProperties);
        }
    }
}
