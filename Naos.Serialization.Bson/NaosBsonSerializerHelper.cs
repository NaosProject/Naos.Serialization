// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosBsonSerializerHelper.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.IO;

    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;

    using Spritely.Recipes;

    /// <summary>
    /// Helper class for using <see cref="BsonSerializer"/>.
    /// </summary>
    public static class NaosBsonSerializerHelper
    {
        /// <summary>
        /// Serializes an object into a byte array.
        /// </summary>
        /// <param name="objectToSerialize">Object to serialize.</param>
        /// <returns>Serialized object into a byte array.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "I like this name...")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It should not actually be an issue.")]
        public static byte[] Serialize(object objectToSerialize)
        {
            new { objectToSerialize }.Must().NotBeNull().OrThrow();

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new BsonBinaryWriter(memoryStream))
                {
                    BsonSerializer.Serialize(writer, objectToSerialize.GetType(), objectToSerialize);
                    writer.Close();
                    memoryStream.Close();
                    return memoryStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Deserializes the byte array into an object.
        /// </summary>
        /// <param name="serializedBytes">Byte array to deserialize.</param>
        /// <typeparam name="T">Type of object to deserialize.</typeparam>
        /// <returns>Deserialized bytes into object of specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "I like this name...")]
        public static T Deserialize<T>(byte[] serializedBytes)
        {
            var ret = Deserialize(serializedBytes, typeof(T));
            return (T)ret;
        }

        /// <summary>
        /// Deserializes the byte array into an object.
        /// </summary>
        /// <param name="serializedBytes">Byte array to deserialize.</param>
        /// <param name="type">Type to deserialize into.</param>
        /// <returns>Deserialized bytes into object of specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "I like this name...")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It should not actually be an issue.")]
        public static object Deserialize(byte[] serializedBytes, Type type)
        {
            new { serializedBytes }.Must().NotBeNull().OrThrow();

            using (var memoryStream = new MemoryStream(serializedBytes))
            {
                using (var reader = new BsonBinaryReader(memoryStream))
                {
                    var ret = BsonSerializer.Deserialize(reader, type);
                    reader.Close();
                    memoryStream.Close();
                    return ret;
                }
            }
        }
    }
}