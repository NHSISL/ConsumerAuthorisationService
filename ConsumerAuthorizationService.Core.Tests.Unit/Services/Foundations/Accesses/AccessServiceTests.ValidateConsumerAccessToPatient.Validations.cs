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
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnValidateIfNhsNumberIsInvalidAndLogItAsync(
            string invalidNhsNumber)
        {
            // given
            string randomConsumerId = GetRandomString();
            List<string> randomSubscriberAgreementIds = GetRandomStringList();
            Guid randomCorrelationId = GetRandomGuid();

            var invalidAccessException =
                new InvalidAccessException(
                    message: "Invalid access. Please correct the errors and try again.");

            invalidAccessException.AddData(
                key: "nhsNumber",
                values: "Text is required");

            var expectedAccessServiceValidationException =
                new AccessValidationException(
                    message: "Access validation errors occurred, please try again.",
                    innerException: invalidAccessException);

            // when
            ValueTask<Access> validateTask = this.accessService.ValidateConsumerAccessToPatientAsync(
                nhsNumber: invalidNhsNumber,
                consumerUserId: randomConsumerId,
                subscriberAgreementIds: randomSubscriberAgreementIds,
                correlationId: randomCorrelationId,
                cancellationToken: CancellationToken.None);

            AccessValidationException actualAccessServiceValidationException =
                await Assert.ThrowsAsync<AccessValidationException>(
                    validateTask.AsTask);

            // then
            actualAccessServiceValidationException.Should()
                .BeEquivalentTo(expectedAccessServiceValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessServiceValidationException))),
                Times.Once);

            this.snowflakeFhirStorageBrokerMock.Verify(broker =>
                broker.ValidateConsumerAccessToPatientAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            this.snowflakeFhirStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnValidateIfConsumerIdIsInvalidAndLogItAsync(
            string invalidConsumerId)
        {
            // given
            string randomNhsNumber = GetRandomString();
            List<string> randomSubscriberAgreementIds = GetRandomStringList();
            Guid randomCorrelationId = GetRandomGuid();

            var invalidAccessException =
                new InvalidAccessException(
                    message: "Invalid access. Please correct the errors and try again.");

            invalidAccessException.AddData(
                key: "consumerUserId",
                values: "Text is required");

            var expectedAccessServiceValidationException =
                new AccessValidationException(
                    message: "Access validation errors occurred, please try again.",
                    innerException: invalidAccessException);

            // when
            ValueTask<Access> validateTask = this.accessService.ValidateConsumerAccessToPatientAsync(
                nhsNumber: randomNhsNumber,
                consumerUserId: invalidConsumerId,
                subscriberAgreementIds: randomSubscriberAgreementIds,
                correlationId: randomCorrelationId,
                cancellationToken: CancellationToken.None);

            AccessValidationException actualAccessServiceValidationException =
                await Assert.ThrowsAsync<AccessValidationException>(
                    validateTask.AsTask);

            // then
            actualAccessServiceValidationException.Should()
                .BeEquivalentTo(expectedAccessServiceValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessServiceValidationException))),
                Times.Once);

            this.snowflakeFhirStorageBrokerMock.Verify(broker =>
                broker.ValidateConsumerAccessToPatientAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            this.snowflakeFhirStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnValidateIfSubscriberAgreementIdsIsNullAndLogItAsync()
        {
            // given
            string randomNhsNumber = GetRandomString();
            string randomConsumerId = GetRandomString();
            List<string> nullSubscriberAgreementIds = null;
            Guid randomCorrelationId = GetRandomGuid();

            var invalidAccessException =
                new InvalidAccessException(
                    message: "Invalid access. Please correct the errors and try again.");

            invalidAccessException.AddData(
                key: "subscriberAgreementIds",
                values: "List is required");

            var expectedAccessServiceValidationException =
                new AccessValidationException(
                    message: "Access validation errors occurred, please try again.",
                    innerException: invalidAccessException);

            // when
            ValueTask<Access> validateTask = this.accessService.ValidateConsumerAccessToPatientAsync(
                nhsNumber: randomNhsNumber,
                consumerUserId: randomConsumerId,
                subscriberAgreementIds: nullSubscriberAgreementIds,
                correlationId: randomCorrelationId,
                cancellationToken: CancellationToken.None);

            AccessValidationException actualAccessServiceValidationException =
                await Assert.ThrowsAsync<AccessValidationException>(
                    validateTask.AsTask);

            // then
            actualAccessServiceValidationException.Should()
                .BeEquivalentTo(expectedAccessServiceValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessServiceValidationException))),
                Times.Once);

            this.snowflakeFhirStorageBrokerMock.Verify(broker =>
                broker.ValidateConsumerAccessToPatientAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            this.snowflakeFhirStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnValidateIfCorrelationIdIsEmptyAndLogItAsync()
        {
            // given
            string randomNhsNumber = GetRandomString();
            string randomConsumerId = GetRandomString();
            List<string> randomSubscriberAgreementIds = GetRandomStringList();
            Guid emptyCorrelationId = Guid.Empty;

            var invalidAccessException =
                new InvalidAccessException(
                    message: "Invalid access. Please correct the errors and try again.");

            invalidAccessException.AddData(
                key: "correlationId",
                values: "Id is required");

            var expectedAccessServiceValidationException =
                new AccessValidationException(
                    message: "Access validation errors occurred, please try again.",
                    innerException: invalidAccessException);

            // when
            ValueTask<Access> validateTask = this.accessService.ValidateConsumerAccessToPatientAsync(
                nhsNumber: randomNhsNumber,
                consumerUserId: randomConsumerId,
                subscriberAgreementIds: randomSubscriberAgreementIds,
                correlationId: emptyCorrelationId,
                cancellationToken: CancellationToken.None);

            AccessValidationException actualAccessServiceValidationException =
                await Assert.ThrowsAsync<AccessValidationException>(
                    validateTask.AsTask);

            // then
            actualAccessServiceValidationException.Should()
                .BeEquivalentTo(expectedAccessServiceValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessServiceValidationException))),
                Times.Once);

            this.snowflakeFhirStorageBrokerMock.Verify(broker =>
                broker.ValidateConsumerAccessToPatientAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            this.snowflakeFhirStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
