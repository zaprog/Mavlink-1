﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UInt32Converter.cs" company="Patryk Mikulicz">
//   Copyright (c) 2017 Patryk Mikulicz.
// </copyright>
// <summary>
//   Represents converter dedicated for uint types
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Mavlink.Common.Converters
{
    /// <summary>
    /// Represents converter dedicated for uint types
    /// </summary>
    internal sealed class UInt32Converter : Converter<uint>
    {
        private const int UintSize = sizeof(uint);

        /// <inheritdoc />
        public override uint ConvertBytes(byte[] bytes)
        {
            if (bytes.Length != UintSize)
                throw new ArgumentException(
                    $"Cannot convert byte array with length {bytes.Length} to uint which size is {UintSize}");

            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <inheritdoc />
        public override byte[] ConvertValue(uint value)
        {
            var convertedValue = BitConverter.GetBytes(value);

            if (convertedValue.Length != UintSize)
                throw new ArgumentException(
                    $"Size of converted value as bytes {convertedValue.Length} is different than uint size which is {UintSize}");

            return convertedValue;
        }
    }
}