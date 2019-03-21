// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InheritedTypeJsonConverterBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    /// <summary>
    /// A converter that handles inherited types.
    /// </summary>
    internal abstract class InheritedTypeJsonConverterBase : JsonConverter
    {
        /// <summary>
        /// The concrete type token name constant.
        /// </summary>
        protected const string ConcreteTypeTokenName = "$concreteType";

        /// <summary>
        /// Determines if the specified type should be handled by this converter.
        /// </summary>
        /// <param name="objectType">The type.</param>
        /// <returns>
        /// true if the specified type should be handled by this converter; false otherwise.
        /// </returns>
        protected bool ShouldBeHandledByThisConverter(
            Type objectType)
        {
            if (objectType == typeof(object))
            {
                return true;
            }

            var ancestry = new List<Type>();
            var currentType = objectType;
            while (currentType.BaseType != null)
            {
                ancestry.Add(currentType.BaseType);
                currentType = currentType.BaseType;
            }

            if (ancestry.Any(_ => _ == typeof(ValueType)))
            {
                return false;
            }

            if (ancestry.Any(_ => _.IsAbstract))
            {
                return true;
            }

            return false;
        }
    }
}
