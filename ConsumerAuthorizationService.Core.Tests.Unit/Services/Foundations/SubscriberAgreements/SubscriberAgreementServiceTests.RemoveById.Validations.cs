// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions;
using Moq;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Foundations.SubscriberAgreements
{
    public partial class SubscriberAgreementServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidSubscriberAgreementId = Guid.Empty;

            var invalidSubscriberAgreementException =
                new InvalidSubscriberAgreementException(
                    message: "Invalid subscriber agreement. Please correct the errors and try again.");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.Id),
                values: "Id is required");

            var expectedSubscriberAgreementValidationException =
                new SubscriberAgreementValidationException(
                    message: "Subscriber agreement validation errors occurred, please try again.",
                    innerException: invalidSubscriberAgreementException);

            // when
            ValueTask<SubscriberAgreement> removeSubscriberAgreementByIdTask =
                this.subscriberAgreementService.RemoveSubscriberAgreementByIdAsync(invalidSubscriberAgreementId);

            SubscriberAgreementValidationException actualSubscriberAgreementServiceValidationException =
                await Assert.ThrowsAsync<SubscriberAgreementValidationException>(
                    removeSubscriberAgreementByIdTask.AsTask);

            // then
            actualSubscriberAgreementServiceValidationException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriberAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowNotFoundExceptionOnRemoveByIdIfSubscriberAgreementIsNotFoundAndLogItAsync()
        {
            // given
            Guid someSubscriberAgreementId = Guid.NewGuid();
            SubscriberAgreement noSubscriberAgreement = null;

            var notFoundSubscriberAgreementException =
                new NotFoundSubscriberAgreementException(
                    message: $"Couldn't find subscriber agreement with subscriberAgreementId: {someSubscriberAgreementId}.");

            var expectedSubscriberAgreementValidationException =
                new SubscriberAgreementValidationException(
                    message: "Subscriber agreement validation errors occurred, please try again.",
                    innerException: notFoundSubscriberAgreementException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriberAgreementByIdAsync(someSubscriberAgreementId))
                    .ReturnsAsync(noSubscriberAgreement);

            // when
            ValueTask<SubscriberAgreement> removeSubscriberAgreementByIdTask =
                this.subscriberAgreementService.RemoveSubscriberAgreementByIdAsync(someSubscriberAgreementId);

            SubscriberAgreementValidationException actualSubscriberAgreementServiceValidationException =
                await Assert.ThrowsAsync<SubscriberAgreementValidationException>(
                    removeSubscriberAgreementByIdTask.AsTask);

            // then
            actualSubscriberAgreementServiceValidationException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriberAgreementByIdAsync(someSubscriberAgreementId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }
    }
}
