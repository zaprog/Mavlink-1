﻿using Mavlink.Messages;
using Mavlink.Messages.Configuration;
using Mavlink.Messages.Dialects.Ardupilot;
using Mavlink.Packets;
using Moq;

namespace Mavlink.UnitTests
{
    public class MavlinkEngineTests
    {
        private static readonly IMavlinkEngine<ArdupilotMessage> _mavlinkEngine;

        private static readonly Mock<IPacketHandler> PacketHandlerMock = new Mock<IPacketHandler>();

        private static readonly Mock<IMessageFactory<ArdupilotMessage>> MessageFactoryMock =
            new Mock<IMessageFactory<ArdupilotMessage>>();

        private static readonly Mock<IMessageMetadataContainer> MessageMetadataContainerMock =
            new Mock<IMessageMetadataContainer>();

        public sealed class CreatePacketTests : MavlinkEngineTests
        {
            public void CreatePacket_NullMessage_ThrowArgumentNullException()
            {
            }
        }
    }
}