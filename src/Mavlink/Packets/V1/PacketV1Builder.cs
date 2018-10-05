﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PacketV1Builder.cs" company="Patryk Mikulicz">
//   Copyright (c) 2016 Patryk Mikulicz.
// </copyright>
// <summary>
//   Component which is responsible for building single first version mavlink packet
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Mavlink.Packets.V1
{
    /// <summary>
    /// Component which is responsible for building the first version of the mavlink packet
    /// </summary>
    internal sealed class PacketV1Builder : IPacketBuilder
    {
        private readonly PacketStructure _packetStructure;
        private readonly byte[] _packetBytes;

        public PacketV1Builder(byte[] packetBytes, PacketStructure packetStructure)
        {
            _packetBytes = packetBytes ?? throw new ArgumentNullException(nameof(packetBytes));
            _packetStructure = packetStructure ?? throw new ArgumentNullException(nameof(packetStructure));
        }

        /// <inheritdoc />
        public Packet Build()
        {
            if (!(_packetStructure is PacketV1Structure packetStructure))
                throw new InvalidCastException($"Cannot cast packet structure to {typeof(PacketV1Structure)}");

            byte payloadLength = _packetBytes.ElementAt(packetStructure.PayloadLenghtIndex);
            int checksumSize = 2;
            int metadataSize = packetStructure.PayloadIndex;
            int totalPacketSize = metadataSize + payloadLength + checksumSize;
            int checksumIndex = packetStructure.PayloadIndex + payloadLength;

            if (totalPacketSize != _packetBytes.Length)
            {
                // incorrect packet bytes
            }

            byte[] payload = new byte[payloadLength];
            byte[] checksum = new byte[checksumSize];
            Array.Copy(_packetBytes, packetStructure.PayloadIndex, payload, 0, payloadLength);
            Array.Copy(_packetBytes, checksumIndex, checksum, 0, checksumSize);

            return new PacketV1
            {
                ByteId = packetStructure.Header,
                ComponentId = _packetBytes.ElementAt(packetStructure.ComponentIdIndex),
                SystemId = _packetBytes.ElementAt(packetStructure.SystemIdIndex),
                PayloadLength = payloadLength,
                SequenceNumber = _packetBytes.ElementAt(packetStructure.SequenceNumberIndex),
                Payload = payload,
                Checksum = checksum
            };
        }
    }
}