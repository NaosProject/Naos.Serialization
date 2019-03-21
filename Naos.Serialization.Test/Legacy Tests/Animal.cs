// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Animal.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    internal class Animal
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called during JSON deserialization but code analysis cannot detect that.")]
        protected Animal(int age, string name)
        {
            this.Age = age;
            this.Name = name;
        }

        public string Name;

        public int Age { get; }
    }

    internal enum FurColor
    {
        /// <summary>
        /// Black fur.
        /// </summary>
        Black,

        /// <summary>
        /// Brindle fur.
        /// </summary>
        Brindle,

        /// <summary>
        /// Golden fur.
        /// </summary>
        Golden,
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
    internal class Dog : Animal
    {
        public Dog(int age, string name, FurColor furColor)
            : base(age, name)
        {
            this.FurColor = furColor;
            this.DogTag = "my name is " + name;
        }

        public FurColor FurColor { get; }

        public string DogTag { get; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Property is used via reflection and code analysis cannot detect that.")]
        public string Nickname { get; set; }
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
    internal class Cat : Animal
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Constructor is used via reflection and code analysis cannot detect that.")]
        public Cat(int age, string name, int numberOfLives)
            : base(age, name)
        {
            this.NumberOfLives = numberOfLives;
        }

        public int NumberOfLives;
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
    internal class Mouse : Animal
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Constructor is used via reflection and code analysis cannot detect that.")]
        public Mouse(int age, string name, FurColor furColor, int tailLength)
            : base(age, name)
        {
            this.FurColor = furColor;
            this.TailLength = tailLength;
        }

        public FurColor FurColor { get; }

        public int TailLength;
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used but is constructed via reflection and code analysis cannot detect that.")]
    internal class Tiger : Animal
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Constructor is used via reflection and code analysis cannot detect that.")]
        public Tiger(int age, string name, int numberOfTeeth, int tailLength)
            : base(age, name)
        {
            this.NumberOfTeeth = numberOfTeeth;
            this.TailLength = tailLength;
        }

        public int NumberOfTeeth { get; }

        public int TailLength;
    }
}
