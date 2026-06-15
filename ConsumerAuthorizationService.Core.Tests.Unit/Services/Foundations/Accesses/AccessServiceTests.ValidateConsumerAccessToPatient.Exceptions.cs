// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses.Exceptions;
using FluentAssertions;
using Moq;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Foundations.Accesses
{
    public partial class AccessServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnValidateIfExceptionOccursAndLogItAsync()
        {
            // given
            string randomNhsNumber = GetRandomString();
            string randomConsumerId = GetRandomString();
            List<string> randomSubscriberAgreementIds = GetRandomStringList();
            Guid randomCorrelationId = GetRandomGuid();
            CancellationToken cancellationToken = CancellationToken.None;
            var serviceException = new Exception();

            var failedStorageAccessException =
                new FailedStorageAccessException(
                    message: "Failed access storage error occurred, contact support.",
                    innerException: serviceException);

            var expectedAccessServiceDependencyException =
                new AccessDependencyException(
                    message: "Access dependency error occurred, contact support.",
                    innerException: failedStorageAccessException);

            this.snowflakeFhirStorageBrokerMock.Setup(broker =>
                broker.ValidateConsumerAccessToPatientAsync(
                    randomNhsNumber,
                    randomConsumerId,
                    randomSubscriberAgreementIds,
                    randomCorrelationId,
                    cancellationToken))
                .ThrowsAsync(serviceException);

            // when
            ValueTask<Access> validateTask = this.accessService.ValidateConsumerAccessToPatientAsync(
                nhsNumber: randomNhsNumber,
                consumerUserId: randomConsumerId,
                subscriberAgreementIds: randomSubscriberAgreementIds,
                correlationId: randomCorrelationId,
                cancellationToken: cancellationToken);

            AccessDependencyException actualAccessServiceDependencyException =
                await Assert.ThrowsAsync<AccessDependencyException>(
                    validateTask.AsTask);

            // then
            actualAccessServiceDependencyException.Should()
                .BeEquivalentTo(expectedAccessServiceDependencyException);

            this.snowflakeFhirStorageBrokerMock.Verify(broker =>
                broker.ValidateConsumerAccessToPatientAsync(
                    randomNhsNumber,
                    randomConsumerId,
                    randomSubscriberAgreementIds,
                    randomCorrelationId,
                    cancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedAccessServiceDependencyException))),
                Times.Once);

            this.snowflakeFhirStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
