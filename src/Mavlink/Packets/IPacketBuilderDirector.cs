﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPacketBuilderDirector.cs" company="Patryk Mikulicz">
//   Copyright (c) 2018 Patryk Mikulicz.
// </copyright>
// <summary>
//   Interface of a component which is responsible for constructing appropriate packet builder
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Mavlink.Packets
{
    /// <summary>
    /// Interface of a component which is responsible for constructing appropriate packet builder
    /// </summary>
    internal interface IPacketBuilderDirector
    {
        /// <summary>
        /// Add single packet byte
        /// </summary>
        /// <param name="packetByte">Single packet byte</param>
        /// <returns>True if there are more bytes to append, otherwise false</returns>
        bool AddByte(byte packetByte);

        /// <summary>
        /// Constructs appriopriate packet builder
        /// </summary>
        /// <returns>Concrete packet builder</returns>
        IPacketBuilder Construct();
    }
}