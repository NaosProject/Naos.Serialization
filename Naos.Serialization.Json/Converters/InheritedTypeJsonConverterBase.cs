// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InheritedTypeJsonConverterBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Newtonsoft.Json;

    using OBeautifulCode.Reflection.Recipes;

    /// <summary>
    /// Inspired by: http://StackOverflow.com/a/17247339/1442829
    /// --- This requires the base type it's used on to declare all of the types it might use... ---
    /// Use Bindable Attribute to match a derived class based on the class given to the serializer
    /// Selected class will be the first class to match all properties in the json object.
    /// </summary>
    internal abstract class InheritedTypeJsonConverterBase : JsonConverter
    {
        private readonly ConcurrentDictionary<Type, IReadOnlyCollection<Type>> allChildTypes =
            new ConcurrentDictionary<Type, IReadOnlyCollection<Type>>();

        /// <summary>
        /// The type token name constant.
        /// </summary>
        protected const string TypeTokenName = "$concreteType";

        /// <summary>
        /// Gets the child types of a specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The child types of the specified type.
        /// </returns>
        protected IEnumerable<Type> GetAssignableTypes(Type type)
        {
            var allTypes = AssemblyLoader.GetLoadedAssemblies().GetTypesFromAssemblies();

            if (!this.allChildTypes.ContainsKey(type))
            {
                var assignableTypes = allTypes
                    .Where(
                        typeToConsider =>
                            typeToConsider != type &&
                            typeToConsider.IsClass &&
                            (!typeToConsider.IsAnonymous()) &&
                            (!typeToConsider.IsGenericTypeDefinition) && // can't do an IsAssignableTo check on generic type definitions
                            typeToConsider.IsAssignableTo(type))
                    .ToList();

                this.allChildTypes.AddOrUpdate(type, assignableTypes, (t, cts) => assignableTypes);
            }

            var result = this.allChildTypes[type];

            return result;
        }

        /// <summary>
        /// Gets the <see cref="BindableAttribute"/> on an object.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>
        /// The <see cref="BindableAttribute"/> on an object.
        /// </returns>
        protected static BindableAttribute GetBindableAttribute(Type objectType)
        {
            // If multiple types in the hierarchy have a Bindable attribute, only one is returned.
            // If the type is Bindable then that attribute is returned.  Otherwise, the attribute on
            // the type that is closest to the current type, going up the hierarchy, is returned.
            // A single type cannot specify Bindable multiple times, the compiler throws with CS0579.
            var attribute = Attribute.GetCustomAttributes(objectType, typeof(BindableAttribute)).OfType<BindableAttribute>().SingleOrDefault();
            return attribute;
        }

        /// <summary>
        /// Determines if <see cref="BindingDirection.TwoWay"/> is set on a <see cref="BindableAttribute"/>.
        /// </summary>
        /// <param name="bindableAttribute">The attribute.</param>
        /// <returns>
        /// True if the <see cref="BindingDirection.TwoWay"/> is set on the specified <see cref="BindableAttribute"/>.
        /// </returns>
        protected static bool IsTwoWayBindable(BindableAttribute bindableAttribute)
        {
            var bindingDirectionIsTwoWay = bindableAttribute.Direction == BindingDirection.TwoWay;
            return bindingDirectionIsTwoWay;
        }
    }
}
