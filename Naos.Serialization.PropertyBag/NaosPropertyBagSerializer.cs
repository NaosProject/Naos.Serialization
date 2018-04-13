// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosPropertyBagSerializer.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.PropertyBag
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Naos.Serialization.Domain;
    using Naos.Serialization.Domain.Extensions;

    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Reflection.Recipes;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Serializer for moving in and out of a <see cref="Dictionary{TKey,TValue} "/> for string, string.
    /// </summary>
    public class NaosPropertyBagSerializer : ISerializeAndDeserialize, IPropertyBagSerializeAndDeserialize
    {
        /// <summary>
        /// Encoding to use for conversion in and out of bytes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is not mutable.")]
        public static readonly Encoding SerializationEncoding = Encoding.UTF8;

        /// <summary>
        /// Initializes a new instance of the <see cref="NaosPropertyBagSerializer"/> class.
        /// </summary>
        /// <param name="serializationKind">Type of serialization to use.</param>
        /// <param name="configurationType">Type of configuration to use.</param>
        public NaosPropertyBagSerializer(SerializationKind serializationKind = SerializationKind.Default, Type configurationType = null)
        {
            new { serializationKind }.Must().BeEqualTo(SerializationKind.Default).OrThrowFirstFailure();
            (configurationType == null).Named(Invariant($"Param-{nameof(configurationType)} must be null but was: {configurationType}")).Must().BeTrue().OrThrowFirstFailure();

            this.SerializationKind = serializationKind;
            this.ConfigurationType = configurationType;
        }

        /// <inheritdoc cref="IHaveSerializationKind" />
        public SerializationKind SerializationKind { get; private set; }

        /// <inheritdoc cref="IHaveConfigurationType" />
        public Type ConfigurationType { get; private set; }

        /// <summary>
        /// Converts string into a byte array.
        /// </summary>
        /// <param name="stringRepresentation">String representation.</param>
        /// <returns>Byte array.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Name/spelling is correct.")]
        public static byte[] ConvertStringToByteArray(string stringRepresentation)
        {
            var ret = SerializationEncoding.GetBytes(stringRepresentation);
            return ret;
        }

        /// <summary>
        /// Converts string representation byte array into a string.
        /// </summary>
        /// <param name="stringRepresentationAsBytes">String representation as bytes.</param>
        /// <returns>JSON string.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "I like this name...")]
        public static string ConvertByteArrayToString(byte[] stringRepresentationAsBytes)
        {
            var ret = SerializationEncoding.GetString(stringRepresentationAsBytes);
            return ret;
        }

        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public byte[] SerializeToBytes(object objectToSerialize)
        {
            var stringRepresentation = this.SerializeToString(objectToSerialize);
            var stringRepresentationBytes = ConvertStringToByteArray(stringRepresentation);
            return stringRepresentationBytes;
        }

        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public T Deserialize<T>(byte[] serializedBytes)
        {
            var ret = this.Deserialize(serializedBytes, typeof(T));
            return (T)ret;
        }

        /// <inheritdoc cref="IBinarySerializeAndDeserialize"/>
        public object Deserialize(byte[] serializedBytes, Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            var stringRepresentation = ConvertByteArrayToString(serializedBytes);
            return this.Deserialize(stringRepresentation, type);
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public string SerializeToString(object objectToSerialize)
        {
            var serializedObject = this.SerializeToPropertyBag(objectToSerialize);
            var ret = NaosDictionaryStringStringSerializer.SerializeDictionaryToString(serializedObject);

            return ret;
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public T Deserialize<T>(string serializedString)
        {
            var dictionary = NaosDictionaryStringStringSerializer.DeserializeToDictionary(serializedString);
            var ret = this.Deserialize<T>(dictionary);

            return ret;
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public object Deserialize(string serializedString, Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            var dictionary = NaosDictionaryStringStringSerializer.DeserializeToDictionary(serializedString);
            var ret = this.Deserialize(dictionary, type);

            return ret;
        }

        /// <inheritdoc cref="IPropertyBagSerializeAndDeserialize"/>
        public IReadOnlyDictionary<string, string> SerializeToPropertyBag(object objectToSerialize)
        {
            if (objectToSerialize == null)
            {
                return null;
            }

            var specificType = objectToSerialize.GetType();
            var bindingFlags = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public;
            var propertyNames = specificType.GetProperties(bindingFlags);
            var properties = propertyNames.ToDictionary(
                k => k.Name,
                v =>
                    {
                        var pi = specificType.GetProperty(v.Name, bindingFlags);
                        pi.Named(Invariant($"Could not find {nameof(PropertyInfo)} on type: {specificType} by name: {v.Name}")).Must().NotBeNull().OrThrowFirstFailure();

                        var propertyValue = pi?.GetValue(objectToSerialize);
                        var serializerType = pi.GetCustomAttribute<NaosStringSerializerAttribute>()?.SerializerType
                                             ?? propertyValue?.GetType().GetAttribute<NaosStringSerializerAttribute>()?.SerializerType;

                        var elementSerializerType = pi.GetCustomAttribute<NaosElementStringSerializerAttribute>()?.ElementSerializerType;

                        return propertyValue == null ? null : MakeStringFromPropertyValue(propertyValue, serializerType, elementSerializerType);
                    });

            properties.Add(nameof(objectToSerialize.ToString), objectToSerialize.ToString());
            properties.Add(nameof(objectToSerialize.GetType), specificType.FullName);

            return properties;
        }

        /// <inheritdoc cref="IPropertyBagSerializeAndDeserialize"/>
        public T Deserialize<T>(IReadOnlyDictionary<string, string> serializedPropertyBag)
        {
            var ret = this.Deserialize(serializedPropertyBag, typeof(T));

            return (T)ret;
        }

        /// <inheritdoc cref="IPropertyBagSerializeAndDeserialize"/>
        public object Deserialize(IReadOnlyDictionary<string, string> serializedPropertyBag, Type type)
        {
            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            if (serializedPropertyBag == null)
            {
                return null;
            }

            var ret = type.Construct();
            if (!serializedPropertyBag.Any())
            {
                return ret;
            }

            if (serializedPropertyBag.ContainsKey(nameof(this.GetType)))
            {
                var specificTypeFullName = serializedPropertyBag[nameof(this.GetType)];
                var loadedTypeMatches = GetAllLoadedTypes().Where(_ => _.FullName == specificTypeFullName).ToList();
                if (loadedTypeMatches.Any())
                {
                    if (loadedTypeMatches.Count > 1)
                    {
                        throw new ArgumentException(Invariant($"Found multiple loaded matches for ({specificTypeFullName}); {string.Join(",", loadedTypeMatches)}"));
                    }
                    else
                    {
                        ret = loadedTypeMatches.Single().Construct();
                    }
                }
            }

            FillProperties(ret, serializedPropertyBag);
            return ret;
        }

        private static void FillProperties(object objectToFill, IReadOnlyDictionary<string, string> properties)
        {
            var specificType = objectToFill.GetType();
            var bindingFlags = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public;
            foreach (var property in properties)
            {
                if (property.Key == nameof(ToString) || property.Key == nameof(GetType))
                {
                    // reserved and not assigned to properties
                    continue;
                }

                var pi = specificType.GetProperty(property.Key, bindingFlags);
                pi.Named(Invariant($"Could not find {nameof(PropertyInfo)} on type: {specificType} by name: {property.Key}")).Must().NotBeNull().OrThrowFirstFailure();

                var targetType = pi.PropertyType;
                var serializerType = pi.GetCustomAttribute<NaosStringSerializerAttribute>()?.SerializerType
                                     ?? targetType.GetAttribute<NaosStringSerializerAttribute>()?.SerializerType;
                var elementSerializerType = pi.GetCustomAttribute<NaosElementStringSerializerAttribute>()?.ElementSerializerType;

                var targetValue = property.Value == null ? null : MakeObjectFromString(property.Value, targetType, serializerType, elementSerializerType);
                pi.SetValue(objectToFill, targetValue);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Name/spelling is correct.")]
        private static object MakeObjectFromString(string serializedString, Type type, Type serializerType, Type elementSerializerType)
        {
            new { serializedString }.Must().NotBeNull().OrThrowFirstFailure();

            if (serializerType != null)
            {
                var ret = serializerType.Construct<IStringSerializeAndDeserialize>().Deserialize(serializedString, type);
                return ret;
            }
            else if (type.IsEnum)
            {
                return Enum.Parse(type, serializedString);
            }
            else if (type.IsArray)
            {
                var arrayItemType = type.GetElementType() ?? throw new ArgumentException(Invariant($"Found array type that cannot extract element type: {type}"));
                var asList = (IList)MakeObjectFromString(
                    serializedString,
                    typeof(List<>).MakeGenericType(arrayItemType),
                    null,
                    elementSerializerType);
                var asArrayList = new ArrayList(asList);
                return asArrayList.ToArray(arrayItemType);
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return NaosDateTimeStringSerializer.DeserializeToDateTime(serializedString);
            }
            else if (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type))
            {
                var itemType = type.GetGenericArguments().SingleOrDefault() ?? throw new ArgumentException(Invariant($"Found {typeof(IEnumerable)} type that cannot extract element type: {type}"));
                var stringValues = serializedString.Split(',');
                IList values = (IList)typeof(List<>).MakeGenericType(itemType).Construct();
                foreach (var stringValue in stringValues)
                {
                    var itemValue = MakeObjectFromString(
                        stringValue,
                        itemType,
                        elementSerializerType ?? itemType.GetAttribute<NaosStringSerializerAttribute>()?.SerializerType,
                        null);
                    values.Add(itemValue);
                }

                return values;
            }
            else
            {
                var bindingFlags = BindingFlags.Static | BindingFlags.Public;
                var parseMethod = type.GetMethods(bindingFlags).SingleOrDefault(
                    _ =>
                        {
                            var parameters = _.GetParameters();
                            return _.Name == "Parse" && parameters.Length == 1 && parameters.Single().ParameterType == typeof(string);
                        });

                if (parseMethod == null)
                {
                    // nothing we can do here so return the string and hope...
                    return serializedString;
                }
                else
                {
                    var ret = parseMethod.Invoke(null, new object[] { serializedString });
                    return ret;
                }
            }
        }

        private static string MakeStringFromPropertyValue(object propertyValue, Type serializerType, Type elementSerializerType)
        {
            new { propertyValue }.Must().NotBeNull().OrThrowFirstFailure();

            if (serializerType != null)
            {
                var ret = serializerType.Construct<IStringSerializeAndDeserialize>().SerializeToString(propertyValue);
                return ret;
            }
            else if (propertyValue is DateTime)
            {
                return NaosDateTimeStringSerializer.SerializeDateTimeToString((DateTime)propertyValue);
            }
            else if (propertyValue.GetType() != typeof(string) && propertyValue is IEnumerable propertyValueAsEnumerable)
            {
                var values = new List<string>();
                foreach (var item in propertyValueAsEnumerable)
                {
                    var serializedItem = MakeStringFromPropertyValue(
                        item,
                        elementSerializerType ?? item.GetType().GetAttribute<NaosStringSerializerAttribute>()?.SerializerType,
                        null);
                    values.Add(serializedItem);
                }

                return values.ToCsv();
            }
            else if (propertyValue is ISerializeToString propertyValueAsSerializeToString)
            {
                return propertyValueAsSerializeToString.SerializeToString();
            }
            else
            {
                return (string)propertyValue.ToString();
            }
        }

        private static IReadOnlyCollection<Type> GetAllLoadedTypes()
        {
            var loadedAssemblies = AssemblyLoader.GetLoadedAssemblies().Distinct().ToList();
            var allTypes = new List<Type>();
            var reflectionTypeLoadExceptions = new List<ReflectionTypeLoadException>();
            foreach (var assembly in loadedAssemblies)
            {
                try
                {
                    allTypes.AddRange(new[] { assembly }.GetTypesFromAssemblies());
                }
                catch (TypeLoadException ex) when (ex.InnerException?.GetType() == typeof(ReflectionTypeLoadException))
                {
                    var reflectionTypeLoadException = (ReflectionTypeLoadException)ex.InnerException;
                    allTypes.AddRange(reflectionTypeLoadException.Types);
                    reflectionTypeLoadExceptions.Add(reflectionTypeLoadException);
                }
            }

            return allTypes;
        }
    }
}
