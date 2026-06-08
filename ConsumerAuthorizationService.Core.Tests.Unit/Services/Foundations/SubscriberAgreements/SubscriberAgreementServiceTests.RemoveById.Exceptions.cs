// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Foundations.SubscriberAgreements
{
    public partial class SubscriberAgreementServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someSubscriberAgreementId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedStorageSubscriberAgreementException =
                new FailedStorageSubscriberAgreementException(
                    message: "Failed subscriber agreement storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedSubscriberAgreementDependencyException =
                new SubscriberAgreementServiceDependencyException(
                    message: "Subscriber agreement dependency error occurred, contact support.",
                    innerException: failedStorageSubscriberAgreementException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriberAgreementByIdAsync(someSubscriberAgreementId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<SubscriberAgreement> removeSubscriberAgreementByIdTask =
                this.subscriberAgreementService.RemoveSubscriberAgreementByIdAsync(someSubscriberAgreementId);

            SubscriberAgreementServiceDependencyException actualSubscriberAgreementDependencyException =
                await Assert.ThrowsAsync<SubscriberAgreementServiceDependencyException>(
                    removeSubscriberAgreementByIdTask.AsTask);

            // then
            actualSubscriberAgreementDependencyException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriberAgreementByIdAsync(someSubscriberAgreementId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementDependencyException))),
                        Times.Once);

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
            ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDatabaseUpdateConcurrencyOccursAndLogItAsync()
        {
            // given
            Guid someSubscriberAgreementId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedSubscriberAgreementException =
                new LockedSubscriberAgreementException(
                    message: "Locked subscriber agreement record exception, please try again later",
                    innerException: dbUpdateConcurrencyException);

            var expectedSubscriberAgreementDependencyValidationException =
                new SubscriberAgreementServiceDependencyValidationException(
                    message: "Subscriber agreement dependency validation occurred, please try again.",
                    innerException: lockedSubscriberAgreementException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriberAgreementByIdAsync(someSubscriberAgreementId))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<SubscriberAgreement> removeSubscriberAgreementByIdTask =
                this.subscriberAgreementService.RemoveSubscriberAgreementByIdAsync(someSubscriberAgreementId);

            SubscriberAgreementServiceDependencyValidationException
                actualSubscriberAgreementDependencyValidationException =
                    await Assert.ThrowsAsync<SubscriberAgreementServiceDependencyValidationException>(
                        removeSubscriberAgreementByIdTask.AsTask);

            // then
            actualSubscriberAgreementDependencyValidationException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriberAgreementByIdAsync(someSubscriberAgreementId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Guid someSubscriberAgreementId = Guid.NewGuid();
            var databaseUpdateException = new DbUpdateException();

            var failedStorageSubscriberAgreementException =
                new FailedStorageSubscriberAgreementException(
                    message: "Failed subscriber agreement storage error occurred, contact support.",
                    innerException: databaseUpdateException);

            var expectedSubscriberAgreementDependencyException =
                new SubscriberAgreementServiceDependencyException(
                    message: "Subscriber agreement dependency error occurred, contact support.",
                    innerException: failedStorageSubscriberAgreementException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriberAgreementByIdAsync(someSubscriberAgreementId))
                    .ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<SubscriberAgreement> removeSubscriberAgreementByIdTask =
                this.subscriberAgreementService.RemoveSubscriberAgreementByIdAsync(someSubscriberAgreementId);

            SubscriberAgreementServiceDependencyException actualSubscriberAgreementDependencyException =
                await Assert.ThrowsAsync<SubscriberAgreementServiceDependencyException>(
                    removeSubscriberAgreementByIdTask.AsTask);

            // then
            actualSubscriberAgreementDependencyException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriberAgreementByIdAsync(someSubscriberAgreementId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someSubscriberAgreementId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedSubscriberAgreementException =
                new FailedSubscriberAgreementException(
                    message: "Failed subscriber agreement service occurred, please contact support",
                    innerException: serviceException);

            var expectedSubscriberAgreementServiceException =
                new SubscriberAgreementServiceException(
                    message: "Subscriber agreement service error occurred, contact support.",
                    innerException: failedSubscriberAgreementException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriberAgreementByIdAsync(someSubscriberAgreementId))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<SubscriberAgreement> removeSubscriberAgreementByIdTask =
                this.subscriberAgreementService.RemoveSubscriberAgreementByIdAsync(someSubscriberAgreementId);

            SubscriberAgreementServiceException actualSubscriberAgreementServiceException =
                await Assert.ThrowsAsync<SubscriberAgreementServiceException>(
                    removeSubscriberAgreementByIdTask.AsTask);

            // then
            actualSubscriberAgreementServiceException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriberAgreementByIdAsync(someSubscriberAgreementId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }
    }
}
