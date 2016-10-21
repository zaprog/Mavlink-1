﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MavlinkCommunicator.cs" company="Patryk Mikulicz">
//   Copyright (c) 2016 Patryk Mikulicz.
// </copyright>
// <summary>
//   Component which is responsible for communication via mavlink protocol
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Mavlink.Messages;
using Mavlink.Messages.Definitions;
using Mavlink.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mavlink
{
    /// <summary>
    /// Component which is responsible for communication via mavlink protocol
    /// </summary>
    internal sealed class MavlinkCommunicator<TMessage> : IMavlinkCommunicator<TMessage> where TMessage : ICommonMessage
    {
        private readonly IPacketHandler _packetHandler;
        private readonly IMessageFactory<TMessage> _messageFactory;
        private readonly Dictionary<Func<TMessage, bool>, MessageNotifier<TMessage>> _messageNotifiers;
        private readonly Stream _stream;

        private readonly Task _streamReadingTaks;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly object _syncRoot = new object();
        private bool _disposed;
        private const int BufferSize = 1024;

        internal MavlinkCommunicator(Stream stream, IPacketHandler packetHandler, IMessageFactory<TMessage> messageFactory)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (packetHandler == null)
                throw new ArgumentNullException(nameof(packetHandler));
            if (messageFactory == null)
                throw new ArgumentNullException(nameof(messageFactory));

            _stream = stream;
            _packetHandler = packetHandler;
            _messageFactory = messageFactory;
            _messageNotifiers = new Dictionary<Func<TMessage, bool>, MessageNotifier<TMessage>>();
            _cancellationTokenSource = new CancellationTokenSource();
            _streamReadingTaks = Task.Factory.StartNew(ProcessReading, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Subscribes for notification of received message from mavlink protocol
        /// </summary>
        /// <param name="condition">A condition which must meet the message</param>
        /// <returns>Component which will notify an incoming message</returns>
        public IMessageNotifier<TMessage> SubscribeForReceive(Func<TMessage, bool> condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            if (_messageNotifiers.ContainsKey(condition))
                return _messageNotifiers[condition];

            MessageNotifier<TMessage> messageNotifier = new MessageNotifier<TMessage>();
            _messageNotifiers.Add(condition, messageNotifier);

            return messageNotifier;
        }

        /// <summary>
        /// Sends message via mavlink protocol asynchronously
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <param name="systemId">Id of a system which is sending message</param>
        /// <param name="componentId">Id of a component which is sending message</param>
        /// <param name="sequenceNumber"></param>
        /// <returns>Value which indicates whether operation completed successfully</returns>
        public bool SendMessage(TMessage message, byte systemId, byte componentId, byte sequenceNumber = 1)
        {
            byte[] packetPayload = _messageFactory.CreateBytes(message);
            Packet packet = _packetHandler.GetPacket(systemId, componentId, sequenceNumber, message.Id, packetPayload);

            if (packet == null)
                return false;

            try
            {
                lock (_syncRoot)
                    _stream.Write(packet.RawBytes, 0, packet.RawBytes.Length);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sends message via mavlink protocol asynchronously
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <param name="systemId">Id of a system which is sending message</param>
        /// <param name="componentId">Id of a component which is sending message</param>
        /// <param name="sequenceNumber">Sequence number of a message</param>
        /// <returns>Value which indicates whether operation completed successfully</returns>
        public async Task<bool> SendMessageAsync(TMessage message, byte systemId, byte componentId, byte sequenceNumber = 1)
        {
            byte[] packetPayload = _messageFactory.CreateBytes(message);
            Packet packet = _packetHandler.GetPacket(systemId, componentId, sequenceNumber, message.Id, packetPayload);

            if (packet == null)
                return await Task.FromResult(false);

            try
            {
                // ReSharper disable once InconsistentlySynchronizedField
                await _stream.WriteAsync(packet.RawBytes, 0, packet.RawBytes.Length);

                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

        public Task<bool> SendMessagesAsync(IEnumerable<TMessage> messages, byte systemId, byte componentId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _cancellationTokenSource.Cancel();

                lock (_syncRoot)
                    _stream.Dispose();
            }
            _disposed = true;
        }

        private void ProcessReading()
        {
            while (true)
            {
                byte[] buffer = new byte[BufferSize];
                int bytesRead;

                lock (_syncRoot)
                    bytesRead = _stream.Read(buffer, 0, BufferSize);

                if (bytesRead == 0)
                    continue;

                byte[] packetBytes = new byte[bytesRead];
                Array.Copy(buffer, 0, packetBytes, 0, bytesRead);
                IEnumerable<Packet> packets = _packetHandler.HandlePackets(packetBytes);

                foreach (Packet packet in packets)
                {
                    TMessage message = _messageFactory.CreateMessage(packet.Payload, packet.MessageId);
                    NotifyForMessage(message);
                }
            }
        }

        private void NotifyForMessage(TMessage message)
        {
            foreach (var messageNotifier in _messageNotifiers)
            {
                if (messageNotifier.Key(message))
                    messageNotifier.Value.OnMessageReceived(new MessageReceivedEventArgs<TMessage>(message));
            }
        }

        ~MavlinkCommunicator()
        {
            Dispose(false);
        }
    }
}