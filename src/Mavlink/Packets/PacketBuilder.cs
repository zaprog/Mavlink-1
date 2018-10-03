﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PacketBuilder.cs" company="Patryk Mikulicz">
//   Copyright (c) 2016 Patryk Mikulicz.
// </copyright>
// <summary>
//   Abstract implementation of a component which is responsible for building mavlink packet
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Mavlink.Packets
{
    /// <summary>
    /// Abstract implementation of a component which is responsible for building mavlink packet
    /// </summary>
    internal abstract class PacketBuilder : IPacketBuilder
    {
        private readonly byte[] _content;

        protected PacketBuilder(byte[] content)
        {
            _content = content;
        }

        /// <inheritdoc />
        public abstract Packet Build(BuildType buildType = BuildType.WithCrc);

        /// <inheritdoc />
        public abstract MavlinkVersion MavlinkVersion { get; }
    }
}