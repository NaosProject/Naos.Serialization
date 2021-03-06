// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Naos.Serialization.Json.InheritedTypeReaderJsonConverter.#TryDeserializeCandidates(Newtonsoft.Json.Linq.JObject,System.Collections.Generic.IReadOnlyCollection`1<Naos.Serialization.Json.InheritedTypeReaderJsonConverter+Candidate>,Newtonsoft.Json.JsonSerializer)", Justification = "Any exception is a false and is intended as no other way to find out if it will deserialize.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "ex", Scope = "member", Target = "Naos.Serialization.Json.InheritedTypeReaderJsonConverter.#TryDeserializeCandidates(Newtonsoft.Json.Linq.JObject,System.Collections.Generic.IReadOnlyCollection`1<Naos.Serialization.Json.InheritedTypeReaderJsonConverter+Candidate>,Newtonsoft.Json.JsonSerializer)", Justification = "Keeping for debugger watch.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Want the protected fields to be fields.", Scope = "member", Target = "~F:Naos.Serialization.Json.DictionaryJsonConverterBase.typesThatSerializeToString")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "Naos.Serialization.Json.JsonConfigurationBase.#.cctor()", Justification = "Keeping this way.")]

/* This file is used by Code Analysis to maintain SuppressMessage
attributes that are applied to this project.
Project-level suppressions either have no target or are given
a specific target and scoped to a namespace, type, member, etc.

To add a suppression to this file, right-click the message in the
Code Analysis results, point to "Suppress Message", and click
 "In Suppression File".

You do not need to add suppressions to this file manually.
*/
