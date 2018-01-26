﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Int64Converter.cs" company="Patryk Mikulicz">
//   Copyright (c) 2017 Patryk Mikulicz.
// </copyright>
// <summary>
//   Represents converter dedicated for long types
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Mavlink.Common.Converters
{
    /// <summary>
    /// Represents converter dedicated for long types
    /// </summary>
    internal sealed class Int64Converter : Converter<long>
    {
        private const int LongSize = sizeof(long);

        /// <inheritdoc />
        public override long ConvertBytes(byte[] bytes)
        {
            if (bytes.Length != LongSize)
                throw new ArgumentException(
                    $"Cannot convert byte array with length {bytes.Length} to long which size is {LongSize}");

            return BitConverter.ToInt64(bytes, 0);
        }

        /// <inheritdoc />
        public override byte[] ConvertValue(long value)
        {
            var convertedValue = BitConverter.GetBytes(value);

            if (convertedValue.Length != LongSize)
                throw new ArgumentException(
                    $"Size of converted value as bytes {convertedValue.Length} is different than long size which is {LongSize}");

            return convertedValue;
        }
    }
}