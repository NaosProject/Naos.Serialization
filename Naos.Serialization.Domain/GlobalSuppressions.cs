// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Naos.Serialization.Domain", Justification = "Layer is important for sharing models and logic between different serialization options.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:Constants should appear before fields", Scope = "member", Target = "~F:Naos.Serialization.Domain.NaosDateTimeStringSerializer.MatchLocalRegexPattern", Justification = "Want it near the code using it.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Naos.Serialization.Domain.Extensions", Justification = "Want extensions in a different namespace since they are on object.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Want the protected fields to be fields.", Scope = "member", Target = "~F:Naos.Serialization.Domain.Tracker`1.NullTrackedOperation")]

/* This file is used by Code Analysis to maintain SuppressMessage
attributes that are applied to this project.
Project-level suppressions either have no target or are given
a specific target and scoped to a namespace, type, member, etc.

To add a suppression to this file, right-click the message in the
Code Analysis results, point to "Suppress Message", and click
 "In Suppression File".

You do not need to add suppressions to this file manually.
*/
