// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InheritedTypeReaderJsonConverter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// An <see cref="InheritedTypeJsonConverterBase"/> that handles reads/deserialization.
    /// </summary>
    internal class InheritedTypeReaderJsonConverter : InheritedTypeJsonConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InheritedTypeReaderJsonConverter"/> class.
        /// </summary>
        /// <param name="typesToHandle">Types that when encountered should trigger usage of the converter.</param>
        public InheritedTypeReaderJsonConverter(IReadOnlyCollection<Type> typesToHandle)
            : base(typesToHandle)
        {
        }

        private readonly ConcurrentDictionary<Type, IReadOnlyCollection<Type>> assignableTypesCache =
            new ConcurrentDictionary<Type, IReadOnlyCollection<Type>>();

        private bool readJsonCalled = false;

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            // ReadJson determines which type (if payload is annotated with a concrete type token) or type(s)
            // (if evaluating candidates) to deserialize into and then attempts to deserialize into those type(s)
            // and pick a deserialized object to return.  In that process Newtonsoft will ask this JSON converter
            // if it can perform the deserialization, since we are using the same serializer.
            // At that point, we want to explicitly say "no".  ReadJson is expecting the deserialization
            // to skip over this Converter and go down the "normal" path, but there's no way to tell it to do that.
            // Since we control the "front door" (i.e. NaosJsonSerializer), we can new-up this converter on each
            // call to Deserialize.   This guarantees that an instance is dedicated to a single Deserialize operation
            // (which may trigger multiple successive calls to the instance depending on how deep the object graph is).
            // As such, when this flag is raised, it is guaranteed to have been raised by a prior call to ReadJson for
            // the same overall (thru the front-door) deserialization operation.
            if (this.readJsonCalled)
            {
                this.readJsonCalled = false;

                return false;
            }

            var result = this.ShouldBeHandledByThisConverter(objectType);

            return result;
        }

        /// <inheritdoc />
        /// <remarks>
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
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            new { reader }.Must().NotBeNull();
            new { serializer }.Must().NotBeNull();

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObject = JObject.Load(reader);
            var jsonProperties = GetProperties(jsonObject);

            var annotatedConcreteType = TryRemoveAnnotatedConcreteType(jsonObject, jsonProperties);

            if (annotatedConcreteType != null)
            {
                var annotatedConcreteTypeResult = this.Deserialize(jsonObject, annotatedConcreteType, serializer);

                return annotatedConcreteTypeResult;
            }

            if (objectType == typeof(object))
            {
                throw new JsonSerializationException(Invariant($"Type has too many assignable types (requires annotated concrete type).  target type: {objectType}, json payload {jsonObject}."));
            }

            var assignableTypes = this.GetAssignableTypes(objectType);

            if (!assignableTypes.Any())
            {
                object assignableTypeAbsentResult;
                if (objectType.IsClass)
                {
                    assignableTypeAbsentResult = this.Deserialize(jsonObject, objectType, serializer);
                }
                else
                {
                    throw new JsonSerializationException(Invariant($"Could not find a class that's assignable to target type.  target type: {objectType}, json payload {jsonObject}."));
                }

                return assignableTypeAbsentResult;
            }

            var candidates = GetCandidatesUsingMemberMatching(jsonProperties, assignableTypes);

            var deserializedCandidates = this.TryDeserializeCandidates(jsonObject, candidates, serializer);

            if (!deserializedCandidates.Any())
            {
                throw new JsonSerializationException(Invariant($"The json payload could not be deserialized into any of the candidate types.  target type: {objectType}, json payload: {jsonObject}, deserialized candidate types: {string.Join(",", deserializedCandidates.Select(_ => _.Type.ToString()))}."));
            }

            if (deserializedCandidates.Count == 1)
            {
                var singleCandidateResult = deserializedCandidates.Single().DeserializedObject;

                return singleCandidateResult;
            }

            var strictCandidates = FilterCandidateUsingStrictMemberMatching(jsonProperties, deserializedCandidates);

            if (!strictCandidates.Any())
            {
                throw new JsonSerializationException(Invariant($"None of the deserialized candidates survived strict member matching.  target type: {objectType}, json payload: {jsonObject}, deserialized candidate types: {string.Join(",", deserializedCandidates.Select(_ => _.Type.ToString()))}."));
            }

            if (strictCandidates.Count == 1)
            {
                var strictCandidateResult = strictCandidates.Single().DeserializedObject;

                return strictCandidateResult;
            }

            throw new JsonSerializationException(Invariant($"Multiple deserialized candidates survived strict member matching.  target type: {objectType}, json payload: {jsonObject}, strict deserialized candidate types: {string.Join(",", strictCandidates.Select(_ => _.Type.ToString()))}."));
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("This is a read-only converter.");
        }

        private static Type TryRemoveAnnotatedConcreteType(
            JObject jsonObject,
            HashSet<string> jsonProperties)
        {
            Type result;
            if (jsonProperties.Contains(ConcreteTypeTokenName))
            {
                var concreteType = jsonObject[ConcreteTypeTokenName].ToString();

                jsonObject.Remove(ConcreteTypeTokenName);

                result = Type.GetType(concreteType);
            }
            else
            {
                result = null;
            }

            return result;
        }

        private IReadOnlyCollection<Type> GetAssignableTypes(
            Type type)
        {
            var allTypes = AssemblyLoader.GetLoadedAssemblies().GetTypesFromAssemblies();

            if (!this.assignableTypesCache.ContainsKey(type))
            {
                var assignableTypes = allTypes
                    .Where(_ => _.IsNonAnonymousClosedClassType() && _ != type && _.IsAssignableTo(type))
                    .ToList();

                this.assignableTypesCache.AddOrUpdate(type, assignableTypes, (t, cts) => assignableTypes);
            }

            var result = this.assignableTypesCache[type];

            return result;
        }

        private object Deserialize(
            JObject jsonObject,
            Type type,
            JsonSerializer serializer)
        {
            var reader = jsonObject.CreateReader();

            this.readJsonCalled = true;

            var result = serializer.Deserialize(reader, type);

            return result;
        }

        private class Candidate
        {
            public Type Type { get; set; }

            public HashSet<string> Members { get; set; }

            public object DeserializedObject { get; set; }
        }

        private static HashSet<string> GetProperties(
            JToken node)
        {
            var result = new HashSet<string>();

            if (node.Type == JTokenType.Object)
            {
                node.Children<JProperty>().Select(child => child.Name).ToList().ForEach(_ => result.Add(_));
            }

            return result;
        }

        private static IReadOnlyCollection<Candidate> GetCandidatesUsingMemberMatching(
            HashSet<string> jsonProperties,
            IReadOnlyCollection<Type> assignableTypes)
        {
            // Filter to assignable types where every 1st level JSON property is either a public (accessor is public)
            // property or a public field of the assignable type, using case-insensitive matching.  Child types passing
            // this filter are called 'candidates'.
            var result = new List<Candidate>();

            foreach (var assignableType in assignableTypes)
            {
                var typeProperties = assignableType.GetProperties().Select(t => t.Name).ToList();
                var typeFields = assignableType.GetFields().Select(t => t.Name).ToList();

                var typeMembers = new HashSet<string>(new string[0].Concat(typeProperties).Concat(typeFields), StringComparer.CurrentCultureIgnoreCase);

                if (jsonProperties.All(_ => typeMembers.Contains(_)))
                {
                    var candidateTargetType = new Candidate
                    {
                        Type = assignableType,
                        Members = typeMembers,
                    };

                    result.Add(candidateTargetType);
                }
            }

            return result;
        }

        private IReadOnlyCollection<Candidate> TryDeserializeCandidates(
            JObject jsonObject,
            IReadOnlyCollection<Candidate> candidates,
            JsonSerializer serializer)
        {
            var result = new List<Candidate>();

            foreach (var candidate in candidates)
            {
                object deserializedObject = null;

                try
                {
                    deserializedObject = this.Deserialize(jsonObject, candidate.Type, serializer);
                }
#pragma warning disable 168 // Want to keep variable here for use in debugger.
                catch (Exception ex)
#pragma warning restore 168
                {
                }

                if (deserializedObject != null)
                {
                    // This is just a clone of the candidate with the deserialized object added.
                    var deserializedCandidate = new Candidate
                    {
                        Type = candidate.Type,
                        Members = candidate.Members,
                        DeserializedObject = deserializedObject,
                    };

                    result.Add(deserializedCandidate);
                }
            }

            return result;
        }

        private static IReadOnlyCollection<Candidate> FilterCandidateUsingStrictMemberMatching(
            HashSet<string> jsonProperties,
            IReadOnlyCollection<Candidate> deserializedCandidates)
        {
            // Filter to candidates whose public properties and fields are all 1st level JSON properties,
            // using case-insensitive matching. We call this "strict matching."
            var result = deserializedCandidates.Where(_ => _.Members.All(m => jsonProperties.Contains(m, StringComparer.CurrentCultureIgnoreCase))).ToList();

            return result;
        }
    }
}