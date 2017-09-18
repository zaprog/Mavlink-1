﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MavlinkCommunicatorFactory.cs" company="Patryk Mikulicz">
//   Copyright (c) 2016 Patryk Mikulicz.
// </copyright>
// <summary>
//   Component which is responsible for creating new instance of mavlink communicator
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Mavlink.Messages;
using Mavlink.Connection;

namespace Mavlink
{
    /// <summary>
    /// Component which is responsible for creating new instance of mavlink communicator
    /// </summary>
    public sealed class MavlinkCommunicatorFactory : IMavlinkCommunicatorFactory
    {
        /// <inheritdoc />
        public IMavlinkCommunicator<TMessage> Create<TMessage>(IConnectionService connectionService, MavlinkVersion mavlinkVersion) where TMessage : MavlinkMessage
        {
            return new MavlinkCommunicator<TMessage>(connectionService, mavlinkVersion, new MavlinkEngineFactory());
        }
    }
}