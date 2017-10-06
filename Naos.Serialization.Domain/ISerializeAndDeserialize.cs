// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializeAndDeserialize.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    /// <summary>
    /// Interface to serialize and deserialize to and from a byte array or string.
    /// </summary>
    public interface ISerializeAndDeserialize : IStringSerializeAndDeserialize, IBinarySerializeAndDeserialize
    {
    }
}