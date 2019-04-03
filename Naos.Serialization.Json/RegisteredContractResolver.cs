// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredContractResolver.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using Newtonsoft.Json.Serialization;
    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Json converter to use.
    /// </summary>
    public class RegisteredContractResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredContractResolver"/> class.
        /// </summary>
        /// <param name="contractResolverBuilderFunction">Builder function.</param>
        public RegisteredContractResolver(Func<IContractResolver> contractResolverBuilderFunction)
        {
            new { contractResolverBuilderFunction }.Must().NotBeNull();

            this.ContractResolverBuilderFunction = contractResolverBuilderFunction;
        }

        /// <summary>
        /// Gets the builder function.
        /// </summary>
        public Func<IContractResolver> ContractResolverBuilderFunction { get; private set; }
    }
}