﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InheritedTypeReaderJsonConverter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// An <see cref="InheritedTypeJsonConverterBase"/> that handles reads/deserialization.
    /// </summary>
    internal class InheritedTypeReaderJsonConverter : InheritedTypeJsonConverterBase
    {
        private static readonly ThreadLocal<bool> ReadJsonCalled = new ThreadLocal<bool>(() => false);

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            // ReadJson determines which type (if two-way bindable) or type(s) (if not two-way bindable)
            // to deserialize into and then attempts to deserialize into those type(s) and pick a deserialized
            // object to return.  In that process Newtonsoft will ask this JSON converter if it can perform
            // the deserialization, since we are using the same serializer.  At that point, we want to
            // explicitly say "no" (versus relying on the code below this if statement to return false).
            // ReadJson is expecting the deserialization to skip over this Converter and go down the "normal"
            // path, but there's no way to tell it that since we need to use the same serializer which
            // has other important settings like using camel case property names.  In most cases CanConvert
            // would have returned false anyways, but consider deserializing into Type that has derivative
            // types (this type would have to be marked two-way bindable or else the json string would
            // deserialize into multiple candidate types and the code would throw)
            // ReadJson determines that the base type is the type we want to deserialize into, but
            // without the following check (that ReadJson was called), CanConvert would return true because
            // the type does have child types.  What's worse is that the second call to ReadJson would then
            // only consider the child types because the TypeTokenName will be stripped out of the
            // payload by ReadUsingTypeSpecifiedInJson and the code will go down the "candidate" types path.
            if (ReadJsonCalled.Value)
            {
                ReadJsonCalled.Value = false;

                return false;
            }

            var bindableAttribute = GetBindableAttribute(objectType);
            if (bindableAttribute == null)
            {
                return false;
            }

            // When de-serializing, objectType will be whatever type the caller wants to de-serialize into.
            // If the type has other types that can be assigned into it, then we want to use our implementation of ReadJson to pick the
            // right assignable type, otherwise there is no ambiguity about which type to create and json.net can just handle it.
            var childTypes = this.GetAssignableTypes(objectType);
            var result = childTypes.Any();

            return result;
        }

        /// <inheritdoc />
        /// <remarks>
        /// - Methodology
        ///   - Is the type Two-Way bindable and has the type of the object written into the json?  If yes then strip
        ///     out the type information and ask Json.net to deserialize into that type.  If not then...
        ///   - Get all child types of the specified type.
        ///   - Filter to child types where every 1st level JSON property is either a public (accessor is public)
        ///     property or a public field of the child type, using case-insensitive matching.  Child types passing
        ///     this filter are called 'candidates'.
        ///   - If there are no candidates, then throw.
        ///   - For all candidates, ask the serializer to deserialize the JSON as the candidate type.  Catch and
        ///     ignore exceptions when attempting to deserialize.  If only one candidate successfully deserializes
        ///     then return the deserialized object.
        ///   - If more than one candidate successfully deserializes, then filter to candidates whose public
        ///     properties and fields are all 1st level JSON properties, using case-insensitive matching. We call
        ///     this "strict matching."  If only one candidate has a strict match, return the corresponding
        ///     deserialized object.  Otherwise, throw.
        /// - Using the serializer to deserialize enables the method to support types with constructors,
        ///   which JSON.net does well out-of-the-box and which would be cumbersome to emulate.
        /// - This method does not consider > 1st level JSON properties to determine candidates.  In other words,
        ///   it is not matching on the fields/properties of the child's fields/properties (e.g. the match is done
        ///   on Dog.Owner, not Dog.Owner.OwnersAddress).  The issue is that that kind of matching would require
        ///   complex logic.  For example, Strings have properties such as Length which would need to be ignored
        ///   when reflecting.  Similarly, all fields/properties of value-types would need to be ignored.  There
        ///   are likely other corner-cases.  To keep things simple, if the 1st level properties match the type's
        ///   properties and fields, by name, we hand-off the problem to the serializer and let it throw if
        ///   there is some incompatibility deep in the field/property hierarchy.
        /// - This method does not consider the type of objects contained within JSON arrays to determine
        ///   candidates.  This is difficult because JSON arrays can contain a mix of types (just likes .net
        ///   objects) and every element would have to be deserialized and matched against the child's IEnumerable
        ///   type and undoubtedly complexity would arise from dealing with generics and the vast implementations
        ///   of IEnumerable.  Like the bullet above, we simply hand-off the problem to the serializer.
        /// - If properties or fields are removed from a child type after it has been serialized, then the JSON
        ///   will not deserialize properly because that child type will no longer be a candidate.  If, however,
        ///   properties or fields are added to the child type, then the child type will continue to be a
        ///   candidate for the serialized JSON.
        /// - If the user serializes private or internal fields/properties, then this method will not work because
        ///   it only looks for public fields/properties.  We cannot bank on the JSON having been serialized by
        ///   the same serializer passed to this method.  Even if we could, the serializer is so highly
        ///   configurable that it would be difficult to determine whether or which internal or private fields
        ///   or properties are serialized.
        /// - It's OK if the JSON is serialized with NullValueHandling.Ignore because the candidate filter tries
        ///   to find all JSON properties in child type's properties/fields, and not vice-versa.  However, depending
        ///   on how permissive the serializer's Contract Resolver is, those candidates may or may not be able to
        ///   be deserialized.  For example, if constructor parameters are required and a particular parameter is
        ///   excluded from the JSON, then that type cannot be deserialized.
        /// </remarks>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            new { reader }.Must().NotBeNull();
            new { serializer }.Must().NotBeNull();

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObject = JObject.Load(reader);
            var jsonProperties = new HashSet<string>(GetProperties(jsonObject), StringComparer.OrdinalIgnoreCase);

            // if two-way bindable then type should be written into the json
            // if it's not then fallback on the typical strategy
            var bindableAttribute = GetBindableAttribute(objectType);
            if (IsTwoWayBindable(bindableAttribute) && jsonProperties.Contains(TypeTokenName))
            {
                return ReadUsingTypeSpecifiedInJson(serializer, jsonObject);
            }

            var childTypes = this.GetAssignableTypes(objectType);
            var candidateChildTypes = GetCandidateChildTypes(childTypes, jsonProperties);
            var deserializedChildren = DeserializeCandidates(candidateChildTypes, serializer, jsonObject).ToList();

            if (deserializedChildren.Count == 0)
            {
                throw new JsonSerializationException(
                    string.Format(CultureInfo.InvariantCulture, "Unable to deserialize to type {0}, value: {1}", objectType, jsonObject));
            }

            CandidateChildType matchedChild = null;
            if (deserializedChildren.Count == 1)
            {
                matchedChild = deserializedChildren.Single();
            }
            else if (deserializedChildren.Count > 1)
            {
                matchedChild = SelectBestChildUsingStrictPropertyMatching(deserializedChildren, jsonObject, jsonProperties);
            }

            var result = matchedChild.DeserializedObject;

            return result;
        }

        private static object ReadUsingTypeSpecifiedInJson(JsonSerializer serializer, JObject jsonObject)
        {
            var concreteType = jsonObject[TypeTokenName].ToString();
            jsonObject.Remove(TypeTokenName);

            var objectType = Type.GetType(concreteType);
            var reader = jsonObject.CreateReader();

            ReadJsonCalled.Value = true;

            var result = serializer.Deserialize(reader, objectType);

            return result;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("This is a read-only converter.");
        }

        private class CandidateChildType
        {
            public Type Type { get; set; }

            public HashSet<string> PropertiesAndFields { get; set; }

            public object DeserializedObject { get; set; }
        }

        private static IReadOnlyCollection<string> GetProperties(JToken node)
        {
            var result = new List<string>();

            if (node.Type == JTokenType.Object)
            {
                result.AddRange(node.Children<JProperty>().Select(child => child.Name));
            }

            return result;
        }

        private static IEnumerable<CandidateChildType> GetCandidateChildTypes(IEnumerable<Type> childTypes, HashSet<string> jsonProperties)
        {
            var result = new List<CandidateChildType>();

            foreach (var childType in childTypes)
            {
                var childTypeProperties = childType.GetProperties().Select(t => t.Name).ToList();
                var childTypeFields = childType.GetFields().Select(t => t.Name).ToList();
                var childTypePropertiesAndFields = new HashSet<string>(
                    childTypeProperties.Concat(childTypeFields),
                    StringComparer.OrdinalIgnoreCase);
                if (jsonProperties.All(p => childTypePropertiesAndFields.Contains(p)))
                {
                    var candidateChildType = new CandidateChildType
                    {
                        Type = childType,
                        PropertiesAndFields = childTypePropertiesAndFields,
                    };
                    result.Add(candidateChildType);
                }
            }

            return result;
        }

        private static IReadOnlyCollection<CandidateChildType> DeserializeCandidates(IEnumerable<CandidateChildType> candidateChildTypes, JsonSerializer serializer, JObject jsonObject)
        {
            var result = new List<CandidateChildType>();

            foreach (var candidateChildType in candidateChildTypes)
            {
                object deserializedObject = null;

                try
                {
                    ReadJsonCalled.Value = true;

                    deserializedObject = serializer.Deserialize(jsonObject.CreateReader(), candidateChildType.Type);
                }
                catch (JsonException)
                {
                }
                catch (ArgumentException)
                {
                }

                if (deserializedObject != null)
                {
                    var deserializedCandidate = new CandidateChildType
                    {
                        Type = candidateChildType.Type,
                        PropertiesAndFields = candidateChildType.PropertiesAndFields,
                        DeserializedObject = deserializedObject,
                    };

                    result.Add(deserializedCandidate);
                }
            }

            return result;
        }

        private static CandidateChildType SelectBestChildUsingStrictPropertyMatching(IEnumerable<CandidateChildType> candidateChildTypes, JObject jsonObject, HashSet<string> jsonProperties)
        {
            candidateChildTypes = candidateChildTypes.ToList();
            var strictCandidates =
                candidateChildTypes.Where(cct => cct.PropertiesAndFields.All(pf => jsonProperties.Contains(pf)))
                    .ToList();

            if (strictCandidates.Count != 1)
            {
                var typesToReportInException = strictCandidates.Count == 0 ? candidateChildTypes : strictCandidates;
                var matchingTypes =
                    typesToReportInException.Select(_ => _.Type.FullName)
                        .Aggregate((running, current) => running + " | " + current);
                throw new JsonSerializationException(
                    string.Format(CultureInfo.InvariantCulture, "The json string can be deserialized into multiple types: {0}, value: {1}", matchingTypes, jsonObject));
            }

            var result = strictCandidates.Single();

            return result;
        }
    }
}