// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Foundations.SubscriberAgreements
{
    public partial class SubscriberAgreementServiceTests
    {
        [Fact]
        public async Task ShouldRemoveSubscriberAgreementByIdAsync()
        {
            // given
            Guid randomSubscriberAgreementId = Guid.NewGuid();
            Guid inputSubscriberAgreementId = randomSubscriberAgreementId;
            SubscriberAgreement randomSubscriberAgreement = CreateRandomSubscriberAgreement();
            SubscriberAgreement storageSubscriberAgreement = randomSubscriberAgreement;
            SubscriberAgreement auditedSubscriberAgreement = storageSubscriberAgreement.DeepClone();
            SubscriberAgreement expectedSubscriberAgreement = auditedSubscriberAgreement.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriberAgreementByIdAsync(inputSubscriberAgreementId))
                    .ReturnsAsync(storageSubscriberAgreement);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyRemoveAuditValuesAsync(storageSubscriberAgreement))
                    .ReturnsAsync(auditedSubscriberAgreement);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateSubscriberAgreementAsync(auditedSubscriberAgreement))
                    .ReturnsAsync(expectedSubscriberAgreement);

            // when
            SubscriberAgreement actualSubscriberAgreement =
                await this.subscriberAgreementService
                    .RemoveSubscriberAgreementByIdAsync(inputSubscriberAgreementId);

            // then
            actualSubscriberAgreement.Should().BeEquivalentTo(expectedSubscriberAgreement);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriberAgreementByIdAsync(inputSubscriberAgreementId),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyRemoveAuditValuesAsync(storageSubscriberAgreement),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriberAgreementAsync(auditedSubscriberAgreement),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
