// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Foundations.Consumers
{
    public partial class ConsumerServiceTests
    {
        [Fact]
        public async Task ShouldRemoveConsumerByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputConsumerId = randomId;
            Consumer randomConsumer = CreateRandomConsumer();
            Consumer storageConsumer = randomConsumer;
            Consumer auditedConsumer = storageConsumer.DeepClone();
            Consumer expectedConsumer = auditedConsumer.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerByIdAsync(inputConsumerId))
                    .ReturnsAsync(storageConsumer);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyRemoveAuditValuesAsync(storageConsumer))
                    .ReturnsAsync(auditedConsumer);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateConsumerAsync(auditedConsumer))
                    .ReturnsAsync(expectedConsumer);

            // when
            Consumer actualConsumer = await this.consumerService
                .RemoveConsumerByIdAsync(inputConsumerId);

            // then
            actualConsumer.Should().BeEquivalentTo(expectedConsumer);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerByIdAsync(inputConsumerId),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyRemoveAuditValuesAsync(storageConsumer),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerAsync(auditedConsumer),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
