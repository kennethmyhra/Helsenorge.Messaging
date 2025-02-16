﻿/* 
 * Copyright (c) 2020-2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/Helsenorge.Messaging/master/LICENSE
 */

using Helsenorge.Messaging.Amqp;
using Helsenorge.Messaging.Amqp.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using Amqp.Framing;
using Helsenorge.Messaging.Tests.Mocks;
using Xunit;
using AmqpException = Amqp.AmqpException;

namespace Helsenorge.Messaging.Tests.Amqp.Exceptions
{
    public class BusOperationTests
    {
        public static object[][] RecoverableExceptions =
        {
            new object[]{new AmqpCommunicationException(string.Empty), typeof(AmqpCommunicationException)},
            new object[]{new ServerBusyException(string.Empty), typeof(ServerBusyException) },
            new object[]{new TimeoutException(), typeof(AmqpTimeoutException) },
            new object[]{new AmqpException(new Error(AmqpClientConstants.TimeoutError)), typeof(AmqpTimeoutException) },
            new object[]{new AmqpException(new Error(AmqpClientConstants.ServerBusyError)), typeof(ServerBusyException) }
        };

        public static object[][] NonRecoverableExceptions =
        {
            new object[]{new AuthenticationException(), typeof(AuthenticationException) },
            new object[]{new ArgumentException(), typeof(ArgumentException) },
            new object[]{new FormatException(), typeof(FormatException) },
            new object[]{new InvalidOperationException(string.Empty), typeof(InvalidOperationException) },
            new object[]{new QuotaExceededException(string.Empty), typeof(QuotaExceededException) },
            new object[]{new MessagingEntityNotFoundException(string.Empty), typeof(MessagingEntityNotFoundException) },
            new object[]{new MessageLockLostException(string.Empty), typeof(MessageLockLostException) },
            new object[]{new MessagingEntityDisabledException(string.Empty), typeof(MessagingEntityDisabledException) },
            new object[]{new SessionLockLostException(string.Empty), typeof(SessionLockLostException) },
            new object[]{new AmqpException(new Error(AmqpClientConstants.AuthorizationFailedError)), typeof(UnauthorizedException) },
            new object[]{new AmqpException(new Error(AmqpClientConstants.MessageLockLostError)), typeof(MessageLockLostException) },
            new object[]{new AmqpException(new Error(AmqpClientConstants.SessionLockLostError)), typeof(SessionLockLostException) },
            new object[]{new AmqpException(new Error(AmqpClientConstants.SessionCannotBeLockedError)), typeof(SessionCannotBeLockedException) },
            new object[]{new AmqpException(new Error(AmqpClientConstants.ArgumentError)), typeof(ArgumentException) },
            new object[]{new AmqpException(new Error(AmqpClientConstants.ArgumentOutOfRangeError)), typeof(ArgumentOutOfRangeException) },
            new object[]{new AmqpException(new Error(AmqpClientConstants.EntityDisabledError)), typeof(MessagingEntityDisabledException) },
            new object[]{new AmqpException(new Error(AmqpClientConstants.MessageNotFoundError)), typeof(MessageNotFoundException) }
        };

        private MockLoggerProvider _mockLoggerProvider = new MockLoggerProvider(null);
        private readonly ILogger _logger;
        private readonly TestTimeManager _timeManager = new TestTimeManager();


        public BusOperationTests()
        {
            _logger = _mockLoggerProvider.CreateLogger("TestLogger");
        }

        [Theory]
        [MemberData(nameof(RecoverableExceptions))]
        public async Task Should_Retry_For_Recoverable_Exceptions(Exception e, Type resultingExceptionType)
        {
            var attempts = 0;

            var exception = await Assert.ThrowsAsync(resultingExceptionType, () =>
                new AmqpOperationBuilder(_logger, e.GetType().Name)
                {
                    TimeManager = _timeManager,
                    MaxRetryCount = 5,
                    MaximumBackoff = TimeSpan.FromMinutes(1),
                    Timeout = TimeSpan.FromMinutes(5)
                }.Build(() =>
                {
                    ++attempts;
                    throw e;
                }).PerformAsync());

            Assert.Equal(resultingExceptionType, exception.GetType());
            Assert.Equal(6, attempts);
            Assert.Equal(5, _timeManager.Delays.Count);

            var previousDelay = TimeSpan.Zero;
            foreach (var delay in _timeManager.Delays)
            {
                Assert.True(delay > previousDelay);
                previousDelay = delay;
            }
        }

        [Theory]
        [MemberData(nameof(NonRecoverableExceptions))]
        public async Task Should_Not_Retry_For_Non_Recoverable_Exceptions(Exception e, Type resultingExceptionType)
        {
            var attempts = 0;

            var exception = await Assert.ThrowsAsync(resultingExceptionType, () =>
                new AmqpOperationBuilder(_logger, e.GetType().Name)
                {
                    TimeManager = _timeManager
                }.Build(() =>
                {
                    ++attempts;
                    throw e;
                }).PerformAsync());

            Assert.Equal(resultingExceptionType, exception.GetType());
            Assert.Equal(1, attempts);
        }

        private sealed class TestTimeManager : ITimeManager
        {
            public List<TimeSpan> Delays { get; } = new List<TimeSpan>();

            public void Delay(TimeSpan timeSpan)
            {
                Delays.Add(timeSpan);
                CurrentTimeUtc += timeSpan;
            }

            public Task DelayAsync(TimeSpan timeSpan)
            {
                Delays.Add(timeSpan);
                CurrentTimeUtc += timeSpan;
                return Task.CompletedTask;
            }

            public DateTime CurrentTimeUtc { get; private set; } = DateTime.UtcNow;
        }
    }
}
