// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Naos.Serialization.Test", Justification = "Keeping this layer on purpose.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Naos.Serialization.Recipes", Justification = "Recipes go in a separate namespace.")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Need fields for testing serialization behavior.", Scope = "member", Target = "~F:Naos.Serialization.Test.Animal.Name")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Need fields for testing serialization behavior.", Scope = "member", Target = "~F:Naos.Serialization.Test.Cat.NumberOfLives")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Need fields for testing serialization behavior.", Scope = "member", Target = "~F:Naos.Serialization.Test.JsonConfigurationTest.CamelCasedEnumTest.Value")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Need fields for testing serialization behavior.", Scope = "member", Target = "~F:Naos.Serialization.Test.JsonConfigurationTest.CamelCasedPropertyTest.TestName")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Need fields for testing serialization behavior.", Scope = "member", Target = "~F:Naos.Serialization.Test.JsonConfigurationTest.InheritedType1.Child1")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Need fields for testing serialization behavior.", Scope = "member", Target = "~F:Naos.Serialization.Test.JsonConfigurationTest.InheritedType2.Child2")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Need fields for testing serialization behavior.", Scope = "member", Target = "~F:Naos.Serialization.Test.JsonConfigurationTest.InheritedTypeBase.Base")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Need fields for testing serialization behavior.", Scope = "member", Target = "~F:Naos.Serialization.Test.JsonConfigurationTest.SecureStringTest.Secure")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Need fields for testing serialization behavior.", Scope = "member", Target = "~F:Naos.Serialization.Test.Mouse.TailLength")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Need fields for testing serialization behavior.", Scope = "member", Target = "~F:Naos.Serialization.Test.Tiger.TailLength")]

/* This file is used by Code Analysis to maintain SuppressMessage
attributes that are applied to this project.
Project-level suppressions either have no target or are given
a specific target and scoped to a namespace, type, member, etc.

To add a suppression to this file, right-click the message in the
Code Analysis results, point to "Suppress Message", and click
 "In Suppression File".

You do not need to add suppressions to this file manually.
*/
