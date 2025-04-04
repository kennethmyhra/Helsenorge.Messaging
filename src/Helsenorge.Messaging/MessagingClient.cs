﻿/* 
 * Copyright (c) 2020-2024, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/Helsenorge.Messaging/master/LICENSE
 */

using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Helsenorge.Messaging.Abstractions;
using Helsenorge.Messaging.Amqp.Senders;
using Helsenorge.Registries.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Helsenorge.Registries;

namespace Helsenorge.Messaging
{
    /// <summary>
    /// Default implementation of <see cref="IMessagingClient"/>. This must act as a singleton, otherwise syncronous messaging will not work
    /// </summary>
    public sealed class MessagingClient : MessagingCore, IMessagingClient, IAsyncDisposable
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly AsynchronousSender _asynchronousServiceBusSender;
        private readonly SynchronousSender _synchronousServiceBusSender;
        private bool _disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">Set of options to use</param>
        /// <param name="loggerFactory"></param>
        /// <param name="collaborationProtocolRegistry">Reference to the collaboration protocol registry</param>
        /// <param name="addressRegistry">Reference to the address registry</param>
        public MessagingClient(
            MessagingSettings settings,
            ILoggerFactory loggerFactory,
            ICollaborationProtocolRegistry collaborationProtocolRegistry,
            IAddressRegistry addressRegistry) : base(settings, collaborationProtocolRegistry, addressRegistry, loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger(nameof(MessagingClient));
            _asynchronousServiceBusSender = new AsynchronousSender(AmqpCore);
            _synchronousServiceBusSender = new SynchronousSender(AmqpCore);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">Set of options to use</param>
        /// <param name="loggerFactory"></param>
        /// <param name="collaborationProtocolRegistry">Reference to the collaboration protocol registry</param>
        /// <param name="addressRegistry">Reference to the address registry</param>
        /// <param name="certificateStore">Reference to an implementation of <see cref="ICertificateStore"/>.</param>
        public MessagingClient(
            MessagingSettings settings,
            ILoggerFactory loggerFactory,
            ICollaborationProtocolRegistry collaborationProtocolRegistry,
            IAddressRegistry addressRegistry,
            ICertificateStore certificateStore) : base(settings, collaborationProtocolRegistry, addressRegistry, certificateStore, loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger(nameof(MessagingClient));
            _asynchronousServiceBusSender = new AsynchronousSender(AmqpCore);
            _synchronousServiceBusSender = new SynchronousSender(AmqpCore);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">Set of options to use</param>
        /// <param name="loggerFactory"></param>
        /// <param name="collaborationProtocolRegistry">Reference to the collaboration protocol registry</param>
        /// <param name="addressRegistry">Reference to the address registry</param>
        /// <param name="certificateStore">Reference to an implementation of <see cref="ICertificateStore"/>.</param>
        /// <param name="certificateValidator">Reference to an implementation of <see cref="ICertificateValidator"/>.</param>
        /// <param name="messageProtection">Reference to an implementation of <see cref="IMessageProtection"/>.</param>
        public MessagingClient(
            MessagingSettings settings,
            ILoggerFactory loggerFactory,
            ICollaborationProtocolRegistry collaborationProtocolRegistry,
            IAddressRegistry addressRegistry,
            ICertificateStore certificateStore,
            ICertificateValidator certificateValidator,
            IMessageProtection messageProtection) : base(settings, collaborationProtocolRegistry, addressRegistry, certificateStore, certificateValidator, messageProtection, loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger(nameof(MessagingClient));
            _asynchronousServiceBusSender = new AsynchronousSender(AmqpCore);
            _synchronousServiceBusSender = new SynchronousSender(AmqpCore);
        }

        /// <summary>
        /// Sends a message and allows the calling code to continue its work
        /// </summary>
        /// <param name="message">Information about the message being sent</param>
        /// <returns></returns>
        public async Task SendAndContinueAsync(OutgoingMessage message)
        {
            var collaborationProtocolMessage = await PreCheckAsync(_logger, message).ConfigureAwait(false);

            switch (collaborationProtocolMessage.DeliveryProtocol)
            {
                case DeliveryProtocol.Amqp:
                    await _asynchronousServiceBusSender.SendAsync(_logger, message).ConfigureAwait(false);
                    return;
                case DeliveryProtocol.Unknown:
                default:
                    var profile = await AmqpCore.FindProfileAsync(_logger, message).ConfigureAwait(false);
                    throw new MessagingException($"Invalid delivery protocol. Message Function {message.MessageFunction} might be missing from either CPA Id: {profile?.CpaId} or CPP Id: {profile?.CppId}.")
                    {
                        EventId = EventIds.InvalidMessageFunction
                    };
            }
        }

        /// <summary>
        /// Sends a message and blocks the calling code until we have an answer
        /// </summary>
        /// <param name="message"></param>
        /// <returns>The received XML</returns>
        public async Task<XDocument> SendAndWaitAsync(OutgoingMessage message)
        {
            var collaborationProtocolMessage = await PreCheckAsync(_logger, message).ConfigureAwait(false);

            switch (collaborationProtocolMessage.DeliveryProtocol)
            {
                case DeliveryProtocol.Amqp:
                    return await _synchronousServiceBusSender.SendAsync(_logger, message).ConfigureAwait(false);
                case DeliveryProtocol.Unknown:
                default:
                    var profile = await AmqpCore.FindProfileAsync(_logger, message).ConfigureAwait(false);
                    throw new MessagingException($"Invalid delivery protocol. Message Function {message.MessageFunction} might be missing from either CPA Id: {profile?.CpaId} or CPP Id: {profile?.CppId}.")
                    {
                        EventId = EventIds.InvalidMessageFunction
                    };
            }
        }

        /// <summary>
        /// Sends a message without waiting for a reply
        /// </summary>
        /// <param name="message"></param>
        /// <param name="correlationId">The correlation id to use when sending the message. Only relevant in synchronous messaging</param>
        /// <returns>The received XML</returns>
        public async Task SendWithoutWaitingAsync(OutgoingMessage message, string correlationId = null)
        {
            var collaborationProtocolMessage = await PreCheckAsync(_logger, message).ConfigureAwait(false);

            switch (collaborationProtocolMessage.DeliveryProtocol)
            {
                case DeliveryProtocol.Amqp:
                    await _synchronousServiceBusSender.SendWithoutWaitingAsync(_logger, message, correlationId).ConfigureAwait(false);
                    break;
                case DeliveryProtocol.Unknown:
                default:
                    var profile = await AmqpCore.FindProfileAsync(_logger, message).ConfigureAwait(false);
                    throw new MessagingException($"Invalid delivery protocol. Message Function {message.MessageFunction} might be missing from either CPA Id: {profile?.CpaId} or CPP Id: {profile?.CppId}.")
                    {
                        EventId = EventIds.InvalidMessageFunction
                    };
            }
        }

        /// <summary>
        /// Registers a delegate that should be called when we receive a synchronous reply message. This is where the main reply message processing is done.
        /// </summary>
        /// <param name="action">The delegate that should be called</param>
        public void RegisterSynchronousReplyMessageReceivedCallback(Func<IncomingMessage, Task> action) => _synchronousServiceBusSender.OnSynchronousReplyMessageReceived = action;

        private async Task<CollaborationProtocolMessage> PreCheckAsync(ILogger logger, OutgoingMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrEmpty(message.MessageFunction)) throw new ArgumentNullException(nameof(message.MessageFunction));
            if (message.ToHerId <= 0) throw new ArgumentOutOfRangeException(nameof(message.ToHerId));

            var messageFunction = string.IsNullOrEmpty(message.ReceiptForMessageFunction)
                ? message.MessageFunction
                : message.ReceiptForMessageFunction;

            var profile = await AmqpCore.FindProfileAsync(logger, message).ConfigureAwait(false);
            var collaborationProtocolMessage = profile?.FindMessageForReceiver(messageFunction);

            if ((profile != null && DummyCollaborationProtocolProfileFactory.IsDummyProfile(profile))
                || (collaborationProtocolMessage == null && messageFunction.ToUpper().Contains("DIALOG_INNBYGGER_TIMERESERVASJON")))
            {
                // HACK: This whole section inside this if statement is a hack to support communication parties which do not have the process DIALOG_INNBYGGER_TIMERESERVASJON configured.
                // FIXME: This hack is to be removed in the future and external parties should start to use DIALOG_INNBYGGER_AVTALEUSTENDING and DIALOG_INNBYGGER_AVTALEAVBESTILLING.
                var communicationParty = await AddressRegistry.FindCommunicationPartyDetailsAsync(message.ToHerId).ConfigureAwait(false);

                collaborationProtocolMessage = new CollaborationProtocolMessage
                {
                    Name = messageFunction,
                    Action = "APPREC",
                    DeliveryProtocol = DeliveryProtocol.Amqp,
                    DeliveryChannel = communicationParty.AsynchronousQueueName,
                    Parts = new List<CollaborationProtocolMessagePart>
                    {
                        new CollaborationProtocolMessagePart
                        {
                            MaxOccurrence = 1,
                            MinOccurrence = 0,
                            XmlNamespace = "http://www.kith.no/xmlstds/msghead/2006-05-24",
                            XmlSchema = "MsgHead-v1_2.xsd"
                        },
                        new CollaborationProtocolMessagePart
                        {
                            MaxOccurrence = 1,
                            MinOccurrence = 0,
                            XmlNamespace = "http://www.kith.no/xmlstds/dialog/2013-01-23",
                            XmlSchema = "dialogmelding-1.1.xsd"
                        }
                    }
                };
            }
            else if (collaborationProtocolMessage == null)
            {
                throw new MessagingException($"Invalid delivery protocol. Message Function {messageFunction} might be missing from either CPA Id: {profile?.CpaId} or CPP Id: {profile?.CppId}.")
                {
                    EventId = EventIds.InvalidMessageFunction
                };
            }

            return collaborationProtocolMessage;
        }

        /// <inheritdoc />
        public async Task CloseAsync()
        {
            // when all the listeners have shut down, close down the messaging infrastructure
            await AmqpCore.SenderPool.ShutdownAsync(_logger).ConfigureAwait(false);
            await AmqpCore.ReceiverPool.ShutdownAsync(_logger).ConfigureAwait(false);
            await AmqpCore.FactoryPool.ShutdownAsync(_logger).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            _loggerFactory?.Dispose();

            await CloseAsync().ConfigureAwait(false);

            _disposed = true;
        }
    }
}
