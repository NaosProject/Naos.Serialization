// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CamelStrictConstructorContractResolver.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Resolves member mappings for a type using camel casing property names.
    /// Also requires that constructor parameters are defined in the json string
    /// when deserializing, for types with non-default constructors.
    /// </summary>
    /// <remarks>
    /// See. <a href="https://stackoverflow.com/questions/37416233/json-net-should-not-use-default-values-for-constructor-parameters-should-use-de"/>
    /// </remarks>
    internal class CamelStrictConstructorContractResolver
        : CamelCasePropertyNamesContractResolver
    {
        private static readonly CamelStrictConstructorContractResolver ContractResolverInstance
            = new CamelStrictConstructorContractResolver();

        private CamelStrictConstructorContractResolver()
        {
            // this will cause dictionary keys to be lowercased if set to true (or you can set to true and set a dictionary key resolver.
            this.NamingStrategy.ProcessDictionaryKeys = false;
        }

        /// <summary>
        /// Gets an instance of the contract resolver.
        /// </summary>
        /// <remarks>
        /// As of 7.0.1, Json.NET suggests using a static instance for "stateless" contract resolvers, for performance reasons.
        /// <a href="http://www.newtonsoft.com/json/help/html/ContractResolver.htm"/>
        /// <a href="http://www.newtonsoft.com/json/help/html/M_Newtonsoft_Json_Serialization_DefaultContractResolver__ctor_1.htm"/>
        /// "Use the parameter-less constructor and cache instances of the contract resolver within your application for optimal performance."
        /// Also. <a href="https://stackoverflow.com/questions/33557737/does-json-net-cache-types-serialization-information"/>
        /// </remarks>
        public static CamelStrictConstructorContractResolver Instance
        {
            get
            {
                return ContractResolverInstance;
            }
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This method is largely a copy-paste of the method it overrides, and that method does not validate arguments.")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "This method is largely a copy-paste of the method it overrides, and that method does not validate arguments.")]
        protected override IList<JsonProperty> CreateConstructorParameters(ConstructorInfo constructor, JsonPropertyCollection memberProperties)
        {
            var constructorParameters = constructor.GetParameters();

            JsonPropertyCollection parameterCollection = new JsonPropertyCollection(constructor.DeclaringType);

            foreach (ParameterInfo parameterInfo in constructorParameters)
            {
                var matchingMemberProperty = (parameterInfo.Name != null) ? memberProperties.GetClosestMatchProperty(parameterInfo.Name) : null;

                // Constructor type must be assignable from property type.
                // Note that this is the only difference between this method and the method it overrides in DefaultContractResolver.
                // In DefaultContractResolver, the types must match exactly.
                if (matchingMemberProperty != null)
                {
                    var memberType = matchingMemberProperty.PropertyType;
                    var memberTypeIsGeneric = memberType.IsGenericType;
                    var memberGenericArguments = memberType.GetGenericArguments();
                    var parameterTypeIsArray = parameterInfo.ParameterType.IsArray;
                    var parameterElementType = parameterInfo.ParameterType.GetElementType();
                    if (parameterTypeIsArray
                        && memberTypeIsGeneric
                        && memberGenericArguments.Length == 1
                        && memberType.IsAssignableTo(typeof(IEnumerable<>).MakeGenericType(parameterElementType)))
                    {
                        // NO-OP - this allows for the constructor parameter to be a "params" array while still using a collection property as the source.
                    }
                    else if (memberType.IsAssignableTo(parameterInfo.ParameterType))
                    {
                        // NO-OP - vanilla assignable type to constructor check.
                    }
                    else
                    {
                        // no way to do this so null out and the let the next step error with a clean message.
                        matchingMemberProperty = null;
                    }
                }

                if (matchingMemberProperty != null || parameterInfo.Name != null)
                {
                    var property = this.CreatePropertyFromConstructorParameterWithConstructorInfo(matchingMemberProperty, parameterInfo, constructor);

                    if (property != null)
                    {
                        parameterCollection.AddProperty(property);
                    }
                }
            }

            return parameterCollection;
        }

        /// <inheritdoc />
        protected override JsonProperty CreatePropertyFromConstructorParameter(JsonProperty matchingMemberProperty, ParameterInfo parameterInfo)
        {
            return this.CreatePropertyFromConstructorParameterWithConstructorInfo(matchingMemberProperty, parameterInfo);
        }

        private JsonProperty CreatePropertyFromConstructorParameterWithConstructorInfo(JsonProperty matchingMemberProperty, ParameterInfo parameterInfo, ConstructorInfo constructor = null)
        {
            if (matchingMemberProperty == null)
            {
                var constructorMessage = constructor == null
                    ? string.Empty
                    : constructor.DeclaringType.FullName;

                var parameterMessage = parameterInfo == null
                    ? "all parameters"
                    : string.Format(CultureInfo.InvariantCulture, "parameter '{0}'", parameterInfo.Name);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Constructor for '{0}' requires {1}; but it was not found in the json",
                    constructorMessage,
                    parameterMessage);

                throw new JsonSerializationException(message);
            }

            var property = base.CreatePropertyFromConstructorParameter(matchingMemberProperty, parameterInfo);

            if (property != null)
            {
                var required = matchingMemberProperty.Required;
                if (required == Required.Default)
                {
                    if (matchingMemberProperty.PropertyType != null &&
                        matchingMemberProperty.PropertyType.IsValueType &&
                        Nullable.GetUnderlyingType(matchingMemberProperty.PropertyType) == null)
                    {
                        required = Required.Always;
                    }
                    else
                    {
                        // this does NOT mean that the parameter is not required
                        // the property must be defined in JSON, but can be null
                        required = Required.AllowNull;
                    }

                    property.Required = matchingMemberProperty.Required = required;
                }
            }

            return property;
        }
    }
}
