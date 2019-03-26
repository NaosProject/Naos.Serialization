// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITopInterface.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "required for test")]
    public interface ITopInterface
    {
        string Species { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "required for test")]
    public interface IMiddleInterface : ITopInterface
    {
        string Name { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "required for test")]
    public class BottomClass : IMiddleInterface
    {
        public string Species { get; set; }

        public string Name { get; set; }
    }
}
