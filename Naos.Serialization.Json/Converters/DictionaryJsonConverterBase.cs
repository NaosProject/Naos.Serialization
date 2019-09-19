// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryJsonConverterBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Newtonsoft.Json;

    using OBeautifulCode.Reflection.Recipes;

    /// <summary>
    /// Custom dictionary converter to do the right thing.
    /// Supports:
    /// - <see cref="IDictionary{TKey, TValue}"/>
    /// - <see cref="Dictionary{TKey, TValue}"/>
    /// - <see cref="IReadOnlyDictionary{TKey, TValue}" />
    /// - <see cref="ReadOnlyDictionary{TKey, TValue}" />
    /// - <see cref="ConcurrentDictionary{TKey, TValue}" />.
    /// </summary>
    internal abstract class DictionaryJsonConverterBase : JsonConverter
    {
        /// <summary>
        /// Types that are serialized as strings in JSON.
        /// </summary>
        protected readonly IReadOnlyCollection<Type> typesThatSerializeToString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryJsonConverterBase"/> class.
        /// </summary>
        /// <param name="typesThatSerializeToString">Types that convert to a string when serialized.</param>
        protected DictionaryJsonConverterBase(IReadOnlyCollection<Type> typesThatSerializeToString)
        {
            this.typesThatSerializeToString = typesThatSerializeToString ?? new Type[0];
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            bool result;

            if (objectType == null)
            {
                result = false;
            }
            else
            {
                // Note that we are NOT checking whether the type is assignable to a dictionary type,
                // we are specifically checking that the type is a System dictionary type.
                // If the consumer is deriving from a dictionary type, they should create a custom converter.
                if (objectType.IsSystemDictionaryType())
                {
                    var keyType = objectType.GetGenericArguments().First();

                    result = this.ShouldConsiderKeyType(keyType);
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Convert the wrapped dictionary into the correct return type.
        /// </summary>
        /// <param name="returnType">Type to convert to.</param>
        /// <param name="wrappedDictionary">Wrapped dictionary to convert.</param>
        /// <param name="genericArguments">Generic arguments.</param>
        /// <returns>Converted dictionary to proper return object if necessary.</returns>
        protected static object ConvertResultAsNecessary(Type returnType, object wrappedDictionary, Type[] genericArguments)
        {
            object result;
            var unboundedGenericReturnType = returnType.GetGenericTypeDefinition();
            if ((unboundedGenericReturnType == typeof(IDictionary<,>)) || (unboundedGenericReturnType == typeof(Dictionary<,>)))
            {
                // nothing to do, the dictionary is already of the expected return type
                result = wrappedDictionary;
            }
            else if ((unboundedGenericReturnType == typeof(ReadOnlyDictionary<,>)) ||
                     (unboundedGenericReturnType == typeof(IReadOnlyDictionary<,>)))
            {
                result = typeof(ReadOnlyDictionary<,>).MakeGenericType(genericArguments).Construct(wrappedDictionary);
            }
            else if (unboundedGenericReturnType == typeof(ConcurrentDictionary<,>))
            {
                result = typeof(ConcurrentDictionary<,>).MakeGenericType(genericArguments).Construct(wrappedDictionary);
            }
            else
            {
                throw new InvalidOperationException("The following type was not expected: " + returnType);
            }

            return result;
        }

        /// <summary>
        /// Should consider this <see cref="Type"/> of key for this converter.
        /// </summary>
        /// <param name="keyType"><see cref="Type"/> of key.</param>
        /// <returns>A value indicating whether or not to consider the <see cref="Type"/> of key.</returns>
        protected abstract bool ShouldConsiderKeyType(Type keyType);
    }
}
