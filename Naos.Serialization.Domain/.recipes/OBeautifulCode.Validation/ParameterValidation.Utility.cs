﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValidation.Utility.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in OBeautifulCode.Validation source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Validation.Recipes
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using static System.FormattableString;

    /// <summary>
    /// Contains all validations that can be applied to a <see cref="Parameter"/>.
    /// </summary>
#if !OBeautifulCodeValidationRecipesProject
    internal
#else
    public
#endif
        static partial class ParameterValidation
    {
#pragma warning disable SA1201

        private static readonly CodeDomProvider CodeDomProvider = CodeDomProvider.CreateProvider("CSharp");

        private static readonly Regex TypeFriendlyNameGenericArgumentsRegex = new Regex("<.*?>", RegexOptions.Compiled);

        private static readonly MethodInfo GetDefaultValueOpenGenericMethodInfo = ((Func<object>)GetDefaultValue<object>).Method.GetGenericMethodDefinition();

        private static readonly ConcurrentDictionary<Type, MethodInfo> GetDefaultValueTypeToMethodInfoMap = new ConcurrentDictionary<Type, MethodInfo>();

        private static readonly MethodInfo EqualsUsingDefaultEqualityComparerOpenGenericMethodInfo = ((Func<object, object, bool>)EqualsUsingDefaultEqualityComparer).Method.GetGenericMethodDefinition();

        private static readonly ConcurrentDictionary<Type, MethodInfo> EqualsUsingDefaultEqualityComparerTypeToMethodInfoMap = new ConcurrentDictionary<Type, MethodInfo>();

        private static readonly MethodInfo CompareUsingDefaultComparerOpenGenericMethodInfo = ((Func<object, object, CompareOutcome>)CompareUsingDefaultComparer).Method.GetGenericMethodDefinition();

        private static readonly ConcurrentDictionary<Type, MethodInfo> CompareUsingDefaultComparerTypeToMethodInfoMap = new ConcurrentDictionary<Type, MethodInfo>();

        private static void Validate(
            this Parameter parameter,
            Validation validation)
        {
            ParameterValidator.ThrowImproperUseOfFrameworkIfDetected(parameter, ParameterShould.BeMusted);

            validation.ParameterName = parameter.Name;
            validation.IsElementInEnumerable = parameter.HasBeenEached;

            if (parameter.HasBeenEached)
            {
                // check that the parameter is an IEnumerable and not null
                if (!parameter.HasBeenValidated)
                {
                    var eachValidation = new Validation
                    {
                        ParameterName = parameter.Name,
                        ValidationName = nameof(ParameterValidator.Each),
                        Value = parameter.Value,
                        ValueType = parameter.ValueType,
                        IsElementInEnumerable = false,
                    };

                    ThrowIfNotOfType(eachValidation, MustBeEnumerableTypeValidations.Single());
                    NotBeNullInternal(eachValidation);
                }

                var valueAsEnumerable = (IEnumerable)parameter.Value;
                var enumerableType = GetEnumerableGenericType(parameter.ValueType);
                validation.ValueType = enumerableType;

                foreach (var typeValidation in validation.TypeValidations ?? new TypeValidation[] { })
                {
                    typeValidation.TypeValidationHandler(validation, typeValidation);
                }

                foreach (var element in valueAsEnumerable)
                {
                    validation.Value = element;

                    validation.ValueValidationHandler(validation);
                }
            }
            else
            {
                validation.Value = parameter.Value;
                validation.ValueType = parameter.ValueType;

                foreach (var typeValidation in validation.TypeValidations ?? new TypeValidation[] { })
                {
                    typeValidation.TypeValidationHandler(validation, typeValidation);
                }

                validation.ValueValidationHandler(validation);
            }

            parameter.HasBeenValidated = true;
        }

        private static Type GetEnumerableGenericType(
            Type enumerableType)
        {
            // note: this method has a very similar structure to IsAssignableTo()
            // in the future, look for a way to extract the commonality
            // adapted from: https://stackoverflow.com/a/17713382/356790
            Type result;
            if (enumerableType.IsArray)
            {
                // type is array, shortcut
                result = enumerableType.GetElementType();
            }
            else if (enumerableType.IsGenericType && (enumerableType.GetGenericTypeDefinition() == UnboundGenericEnumerableType))
            {
                // type is IEnumerable<T>
                result = enumerableType.GetGenericArguments()[0];
            }
            else
            {
                // type implements IEnumerable<T> or is a subclass (sub-sub-class, ...)
                // of a type that implements IEnumerable<T>
                // note that we are grabbing the first implementation.  it is possible, but
                // highly unlikely, for a type to have multiple implementations of IEnumerable<T>
                result = enumerableType
                    .GetInterfaces()
                    .Where(_ => _.IsGenericType && (_.GetGenericTypeDefinition() == UnboundGenericEnumerableType))
                    .Select(_ => _.GenericTypeArguments[0])
                    .FirstOrDefault();

                if (result == null)
                {
                    var baseType = enumerableType.BaseType;
                    result = baseType == null ? ObjectType : GetEnumerableGenericType(baseType);
                }
            }

            return result;
        }

        private static Type GetDictionaryGenericValueType(
            Type dictionaryType)
        {
            // note: this method has a very similar structure to IsAssignableTo()
            // in the future, look for a way to extract the commonality
            Type result;

            if (dictionaryType.IsGenericType && (dictionaryType.GetGenericTypeDefinition() == UnboundGenericDictionaryType))
            {
                // type is IDictionary<T,K>
                result = dictionaryType.GetGenericArguments()[1];
            }
            else if (dictionaryType.IsGenericType && (dictionaryType.GetGenericTypeDefinition() == UnboundGenericReadOnlyDictionaryType))
            {
                // type is IReadOnlyDictionary<T,K>
                result = dictionaryType.GetGenericArguments()[1];
            }
            else
            {
                // type implements IDictionary<T,K>/IReadOnlyDictionary<T,K> or is a subclass (sub-sub-class, ...)
                // of a type that implements those types
                // note that we are grabbing the first implementation.  it is possible, but
                // highly unlikely, for a type to have multiple implementations of IDictionary<T,K>
                result = dictionaryType
                    .GetInterfaces()
                    .Where(_ => _.IsGenericType && ((_.GetGenericTypeDefinition() == UnboundGenericDictionaryType) || (_.GetGenericTypeDefinition() == UnboundGenericReadOnlyDictionaryType)))
                    .Select(_ => _.GenericTypeArguments[1])
                    .FirstOrDefault();

                if (result == null)
                {
                    var baseType = dictionaryType.BaseType;
                    result = baseType == null ? ObjectType : GetEnumerableGenericType(baseType);
                }
            }

            return result;
        }

        private static bool IsAssignableTo(
            this Type type,
            Type otherType,
            bool treatUnboundGenericAsAssignableTo = false)
        {
            // A copy of this method exists in OBC.Reflection.
            // Any bug fixes made here should also be applied to OBC.Reflection.
            // OBC.Reflection cannot take a reference to OBC.Validation because it creates a circular reference
            // since OBC.Validation itself depends on OBC.Reflection.
            // We considered converting all usages of OBC.Validation in OBC.Reflection to vanilla if..then..throw
            // but decided against because it was going to be too much work and we like the way OBC.Validation reads (e.g. Must().NotBeNull()) in OBC.Reflection.
            // The other option was to create a third package that OBC.Validation and OBC.Reflection could both depend on, but
            // that didn't feel right because this method naturally fits with TypeHelper.
            // note that the parameter checks in OBC.Reflection were replaced with the following, single check:
            if (type.IsGenericTypeDefinition)
            {
                ParameterValidator.ThrowImproperUseOfFramework(Invariant($"The parameter type is an unbounded generic type."));
            }

            // type is equal to the other type
            if (type == otherType)
            {
                return true;
            }

            // type is assignable to the other type
            if (otherType.IsAssignableFrom(type))
            {
                return true;
            }

            // type is generic and other type is an unbounded generic type
            if (treatUnboundGenericAsAssignableTo && otherType.IsGenericTypeDefinition)
            {
                // type's unbounded generic version is the other type
                if (type.IsGenericType && type.GetGenericTypeDefinition() == otherType)
                {
                    return true;
                }

                // type implements an interface who's unbounded generic version is the other type
                if (type.GetInterfaces().Any(_ => _.IsGenericType && (_.GetGenericTypeDefinition() == otherType)))
                {
                    return true;
                }

                var baseType = type.BaseType;
                if (baseType == null)
                {
                    return false;
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                var result = baseType.IsAssignableTo(otherType, treatUnboundGenericAsAssignableTo);
                return result;
            }

            return false;
        }

        private static void ThrowIfMalformedRange(
            ValidationParameter[] validationParameters)
        {
            // the public BeInRange/NotBeInRange is generic and guarantees that minimum and maximum are of the same type
            var rangeIsMalformed = CompareUsingDefaultComparer(validationParameters[0].ValueType, validationParameters[0].Value, validationParameters[1].Value) == CompareOutcome.Value1GreaterThanValue2;
            if (rangeIsMalformed)
            {
                var malformedRangeExceptionMessage = string.Format(CultureInfo.InvariantCulture, MalformedRangeExceptionMessage, validationParameters[0].Name, validationParameters[1].Name, validationParameters[0].Value?.ToString() ?? NullValueToString, validationParameters[1].Value?.ToString() ?? NullValueToString);
                ParameterValidator.ThrowImproperUseOfFramework(malformedRangeExceptionMessage);
            }
        }

        private static string GetFriendlyTypeName(
            this Type type)
        {
            // adapted from: https://stackoverflow.com/a/6402967/356790
            var result = CodeDomProvider.GetTypeOutput(new CodeTypeReference(type.FullName?.Replace(type.Namespace + ".", string.Empty)));

            // if type is an unbounded generic type, then the result will something like List<> or IReadOnlyDictionary<, >
            // whereas we would prefer List<T> or IReadOnlyDictionary<T,K>
            if (type.IsGenericTypeDefinition)
            {
                var genericArgumentNames = string.Join(",", type.GetGenericArguments().Select(x => x.Name));
                result = TypeFriendlyNameGenericArgumentsRegex.Replace(result, "<" + genericArgumentNames + ">");
            }

            return result;
        }

        private static string BuildArgumentExceptionMessage(
            Validation validation,
            string exceptionMessageSuffix,
            Include include = Include.None,
            Type genericTypeOverride = null)
        {
            if (validation.ApplyBecause == ApplyBecause.InLieuOfDefaultMessage)
            {
                // we force to empty string if null because otherwise when the exception is
                // constructed the framework chooses some generic message like 'An exception of type ArgumentException was thrown'
                return validation.Because ?? string.Empty;
            }

            var parameterNameQualifier = validation.ParameterName == null ? string.Empty : Invariant($" '{validation.ParameterName}'");
            var enumerableQualifier = validation.IsElementInEnumerable ? " contains an element that" : string.Empty;
            var genericTypeQualifier = include.HasFlag(Include.GenericType) ? ", where T: " + (genericTypeOverride?.GetFriendlyTypeName() ?? validation.ValueType.GetFriendlyTypeName()) : string.Empty;
            var failingValueQualifier = include.HasFlag(Include.FailingValue) ? (validation.IsElementInEnumerable ? "  Element value" : "  Parameter value") + Invariant($" is '{validation.Value?.ToString() ?? NullValueToString}'.") : string.Empty;
            var validationParameterQualifiers = validation.ValidationParameters == null || !validation.ValidationParameters.Any() ? string.Empty : string.Join(string.Empty, validation.ValidationParameters.Select(_ => _.ToExceptionMessageComponent()));
            var result = Invariant($"Parameter{parameterNameQualifier}{enumerableQualifier} {exceptionMessageSuffix}{genericTypeQualifier}.{failingValueQualifier}{validationParameterQualifiers}");

            if (validation.ApplyBecause == ApplyBecause.PrefixedToDefaultMessage)
            {
                if (!string.IsNullOrWhiteSpace(validation.Because))
                {
                    result = validation.Because + "  " + result;
                }
            }
            else if (validation.ApplyBecause == ApplyBecause.SuffixedToDefaultMessage)
            {
                if (!string.IsNullOrWhiteSpace(validation.Because))
                {
                    result = result + "  " + validation.Because;
                }
            }
            else
            {
                throw new NotSupportedException(Invariant($"This {nameof(ApplyBecause)} is not supported: {validation.ApplyBecause}"));
            }

            return result;
        }

        private static string ToExceptionMessageComponent(
            this ValidationParameter validationParameter)
        {
            var result = Invariant($"  Specified '{validationParameter.Name}' is");
            if (validationParameter.ValueToStringFunc == null)
            {
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (validationParameter.Value == null)
                {
                    result = Invariant($"{result} '{NullValueToString}'");
                }
                else
                {
                    result = Invariant($"{result} '{validationParameter.Value}'");
                }
            }
            else
            {
                result = Invariant($"{result} {validationParameter.ValueToStringFunc()}");
            }

            result = Invariant($"{result}.");

            return result;
        }

        private static Exception AddData(
            this Exception exception,
            IDictionary data)
        {
            if (data != null)
            {
                // because the caller is creating a new exception, we know that Data is empty
                // and we don't have to check for key conflicts (same key exists in both exception.Data and in data)
                foreach (var dataKey in data.Keys)
                {
                    exception.Data[dataKey] = data[dataKey];
                }
            }

            return exception;
        }

        private static T GetDefaultValue<T>()
        {
            var result = default(T);
            return result;
        }

        private static object GetDefaultValue(
            Type type)
        {
            if (!GetDefaultValueTypeToMethodInfoMap.ContainsKey(type))
            {
                GetDefaultValueTypeToMethodInfoMap.TryAdd(type, GetDefaultValueOpenGenericMethodInfo.MakeGenericMethod(type));
            }

            var result = GetDefaultValueTypeToMethodInfoMap[type].Invoke(null, null);
            return result;
        }

        private static bool EqualsUsingDefaultEqualityComparer<T>(
            T value1,
            T value2)
        {
            var result = EqualityComparer<T>.Default.Equals(value1, value2);
            return result;
        }

        private static bool EqualUsingDefaultEqualityComparer(
            Type type,
            object value1,
            object value2)
        {
            if (!EqualsUsingDefaultEqualityComparerTypeToMethodInfoMap.ContainsKey(type))
            {
                EqualsUsingDefaultEqualityComparerTypeToMethodInfoMap.TryAdd(type, EqualsUsingDefaultEqualityComparerOpenGenericMethodInfo.MakeGenericMethod(type));
            }

            var result = (bool)EqualsUsingDefaultEqualityComparerTypeToMethodInfoMap[type].Invoke(null, new[] { value1, value2 });
            return result;
        }

        private static CompareOutcome CompareUsingDefaultComparer<T>(
            T x,
            T y)
        {
            var comparison = Comparer<T>.Default.Compare(x, y);
            CompareOutcome result;
            if (comparison < 0)
            {
                result = CompareOutcome.Value1LessThanValue2;
            }
            else if (comparison == 0)
            {
                result = CompareOutcome.Value1EqualsValue2;
            }
            else
            {
                result = CompareOutcome.Value1GreaterThanValue2;
            }

            return result;
        }

        private static CompareOutcome CompareUsingDefaultComparer(
            Type type,
            object value1,
            object value2)
        {
            if (!CompareUsingDefaultComparerTypeToMethodInfoMap.ContainsKey(type))
            {
                CompareUsingDefaultComparerTypeToMethodInfoMap.TryAdd(type, CompareUsingDefaultComparerOpenGenericMethodInfo.MakeGenericMethod(type));
            }

            // note that the call is ultimately, via reflection, to Compare(T, T)
            // as such, reflection will throw an ArgumentException if the types of value1 and value2 are
            // not "convertible" to the specified type.  It's a pretty complicated heuristic:
            // https://stackoverflow.com/questions/34433043/check-whether-propertyinfo-setvalue-will-throw-an-argumentexception
            // Instead of relying on this heuristic, we just check upfront that value2's type == the specified type
            // (value1's type will always be the specified type).  This constrains our capabilities - for example, we
            // can't compare an integer to a decimal.  That said, we feel like this is a good constraint in a parameter
            // validation framework.  We'd rather be forced to make the types align than get a false negative
            // (a validation passes when it should fail).

            // otherwise, if reflection is able to call Compare(T, T), then ArgumentException can be thrown if
            // Type T does not implement either the System.IComparable<T> generic interface or the System.IComparable interface
            // However we already check for this upfront in ThrowIfNotComparable
            var result = (CompareOutcome)CompareUsingDefaultComparerTypeToMethodInfoMap[type].Invoke(null, new[] { value1, value2 });
            return result;
        }

        private enum CompareOutcome
        {
            Value1LessThanValue2,

            Value1EqualsValue2,

            Value1GreaterThanValue2,
        }

        private class Validation
        {
            public string ValidationName { get; set; }

            public string Because { get; set; }

            public ApplyBecause ApplyBecause { get; set; }

            public ValueValidationHandler ValueValidationHandler { get; set; }

            public ValidationParameter[] ValidationParameters { get; set; }

            public string ParameterName { get; set; }

            public object Value { get; set; }

            public Type ValueType { get; set; }

            public bool IsElementInEnumerable { get; set; }

            public IReadOnlyCollection<TypeValidation> TypeValidations { get; set; }

            public IDictionary Data { get; set; }
        }

        private class ValidationParameter
        {
            public string Name { get; set; }

            public object Value { get; set; }

            public Type ValueType { get; set; }

            public Func<string> ValueToStringFunc { get; set; }
        }

        [Flags]
        private enum Include
        {
            None = 0,

            FailingValue = 1,

            GenericType = 2,
        }

#pragma warning restore SA1201
    }
}
