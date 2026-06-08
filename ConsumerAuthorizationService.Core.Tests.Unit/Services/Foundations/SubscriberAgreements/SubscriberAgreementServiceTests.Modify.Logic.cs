// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using Moq;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Foundations.SubscriberAgreements
{
    public partial class SubscriberAgreementServiceTests
    {
        [Fact]
        public async Task ShouldModifySubscriberAgreementAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            SubscriberAgreement randomSubscriberAgreement =
                CreateRandomModifySubscriberAgreement(randomDateTimeOffset);

            SubscriberAgreement inputSubscriberAgreement = randomSubscriberAgreement;
            SubscriberAgreement storageSubscriberAgreement = inputSubscriberAgreement.DeepClone();
            storageSubscriberAgreement.UpdatedWhen = randomSubscriberAgreement.CreatedWhen;
            SubscriberAgreement auditAppliedSubscriberAgreement = inputSubscriberAgreement.DeepClone();
            auditAppliedSubscriberAgreement.UpdatedBy = randomUserId;
            auditAppliedSubscriberAgreement.UpdatedWhen = randomDateTimeOffset;
            SubscriberAgreement auditEnsuredSubscriberAgreement = auditAppliedSubscriberAgreement.DeepClone();
            SubscriberAgreement updatedSubscriberAgreement = inputSubscriberAgreement;
            SubscriberAgreement expectedSubscriberAgreement = updatedSubscriberAgreement.DeepClone();
            Guid subscriberAgreementId = inputSubscriberAgreement.Id;

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(inputSubscriberAgreement))
                    .ReturnsAsync(auditAppliedSubscriberAgreement);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriberAgreementByIdAsync(subscriberAgreementId))
                    .ReturnsAsync(storageSubscriberAgreement);

            this.securityAuditBrokerMock.Setup(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    auditAppliedSubscriberAgreement,
                    storageSubscriberAgreement))
                        .ReturnsAsync(auditEnsuredSubscriberAgreement);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateSubscriberAgreementAsync(auditEnsuredSubscriberAgreement))
                    .ReturnsAsync(updatedSubscriberAgreement);

            // when
            SubscriberAgreement actualSubscriberAgreement =
                await this.subscriberAgreementService.ModifySubscriberAgreementAsync(inputSubscriberAgreement);

            // then
            actualSubscriberAgreement.Should().BeEquivalentTo(expectedSubscriberAgreement);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(inputSubscriberAgreement),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriberAgreementByIdAsync(subscriberAgreementId),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    auditAppliedSubscriberAgreement,
                    storageSubscriberAgreement),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriberAgreementAsync(auditEnsuredSubscriberAgreement),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
