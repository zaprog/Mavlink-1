﻿using Mavlink.Messages;
using Mavlink.Packets;
using NUnit.Framework;
using System;

namespace Mavlink.UnitTests.Packets
{
    [TestFixture]
    internal class PacketValidatorTests

    {
        protected IPacketValidator PacketValidator;

        [SetUp]
        public void SetUp()
        {
            PacketValidator = new PacketValidator();
        }

        [TestFixture]
        public sealed class ValidateMethodTests : PacketValidatorTests
        {
            [Test]
            public void PacketValidReturnsTrue()
            {
                /*
                hdr len seq sys cmp id   payload (len 9)------------------  crc---
                0   1   2   3   4   5   6   7   8   9   a   b   c   d   e   f  10
                FE  09  4E  01  01  00  00  00  00  00  02  03  51  04  03  1C  7F
                 */
                Packet validPacket = new Packet
                {
                    PayloadLength = 0x09,
                    SequenceNumber = 0x4E,
                    SystemId = 0x01,
                    ComponentId = 0x01,
                    MessageId = MessageId.Heartbeat,
                    Payload = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x51, 0x04, 0x03 },
                    Checksum = new byte[] { 0x1C, 0x7F }
                };

                bool result = PacketValidator.Validate(validPacket);

                Assert.AreEqual(true, result);
            }

            [Test]
            public void PacketInvalidReturnsFalse()
            {
                /*
                hdr len seq sys cmp id   payload (len 9)------------------  crc---
                0   1   2   3   4   5   6   7   8   9   a   b   c   d   e   f  10
                FE  09  4E  01  01  00  01  00  00  00  02  03  51  04  03  1C  7F
                 */
                Packet invalidPacket = new Packet
                {
                    PayloadLength = 0x09,
                    SequenceNumber = 0x4E,
                    SystemId = 0x01,
                    ComponentId = 0x01,
                    MessageId = MessageId.Heartbeat,
                    Payload = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x02, 0x03, 0x51, 0x04, 0x03 },
                    Checksum = new byte[] { 0x1C, 0x7F }
                };

                bool result = PacketValidator.Validate(invalidPacket);

                Assert.AreEqual(false, result);
            }

            [Test]
            public void PacketNullThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => PacketValidator.Validate(null));
            }
        }

        [TestFixture]
        public sealed class GetChecksumMethodTests : PacketValidatorTests
        {
            [Test]
            public void GetChechsumReturnsCorrectCrc()
            {
                /*
                hdr len seq sys cmp id   payload (len 9)------------------  crc---
                0   1   2   3   4   5   6   7   8   9   a   b   c   d   e   f  10
                FE  09  4E  01  01  00  00  00  00  00  02  03  51  04  03  1C  7F
                 */
                Packet packetWithoutChecksum = new Packet
                {
                    PayloadLength = 0x09,
                    SequenceNumber = 0x4E,
                    SystemId = 0x01,
                    ComponentId = 0x01,
                    MessageId = MessageId.Heartbeat,
                    Payload = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x51, 0x04, 0x03 },
                    Checksum = new byte[] { 0x00, 0x00 }
                };

                byte[] expectedChecksum = { 0x1C, 0x7F };

                byte[] checksum = PacketValidator.GetChecksum(packetWithoutChecksum);

                Assert.AreEqual(expectedChecksum, checksum);
            }
        }
    }
}