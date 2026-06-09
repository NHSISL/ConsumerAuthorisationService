// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;
using ConsumerAuthorizationService.Core.Models.Orchestrations.Accesses.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnValidateAccess(string invalidText)
        {
            // given
            Guid correlationId = Guid.Empty;

            var invalidArgumentAccessOrchestrationException =
                new InvalidArgumentAccessOrchestrationException(
                    message: "Invalid argument(s), please correct the errors and try again.");

            invalidArgumentAccessOrchestrationException.AddData(
                key: "consumerUserId",
                values: "Text is invalid");

            invalidArgumentAccessOrchestrationException.AddData(
                key: "nhsNumber",
                values: "Text is invalid");

            invalidArgumentAccessOrchestrationException.AddData(
                key: "correlationId",
                values: "Id is invalid");

            var expectedAccessOrchestrationValidationException =
                new AccessOrchestrationValidationException(
                    message: "Access orchestration validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: invalidArgumentAccessOrchestrationException);

            // when
            ValueTask<Access> validateAccessTask =
                accessOrchestrationService.ValidateAccess(invalidText, invalidText, correlationId);

            AccessOrchestrationValidationException actualAccessOrchestrationValidationException =
                await Assert.ThrowsAsync<AccessOrchestrationValidationException>(
                    testCode: validateAccessTask.AsTask);

            // then
            actualAccessOrchestrationValidationException
                .Should().BeEquivalentTo(expectedAccessOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedAccessOrchestrationValidationException))),
                       Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
            this.consumerAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowUnauthorisedExceptionOnValidateAccessWhenNoMatchingConsumer()
        {
            // given
            string userId = GetRandomString();
            Consumer randomConsumer = CreateRandomConsumer();
            Consumer inputConsumer = randomConsumer.DeepClone();
            Guid correlationId = Guid.NewGuid();

            IQueryable<Consumer> storageConsumers =
                new List<Consumer> { inputConsumer }.AsQueryable();

            string randomNhsNumber = GetRandomStringWithLength(5);
            string inputNhsNumber = randomNhsNumber;

            var unauthorizedAccessOrchestrationException =
                new UnauthorizedAccessOrchestrationException(
                    $"Current consumer with user id `{userId}` is not a valid consumer.");

            var expectedAccessOrchestrationValidationException =
                new AccessOrchestrationValidationException(
                    message: "Access orchestration validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: unauthorizedAccessOrchestrationException);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ReturnsAsync(storageConsumers);

            // when
            ValueTask<Access> validateAccessTask =
                accessOrchestrationService.ValidateAccess(userId, inputNhsNumber, correlationId);

            AccessOrchestrationValidationException actualAccessOrchestrationValidationException =
                await Assert.ThrowsAsync<AccessOrchestrationValidationException>(
                    testCode: validateAccessTask.AsTask);

            // then
            actualAccessOrchestrationValidationException
                .Should().BeEquivalentTo(expectedAccessOrchestrationValidationException);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Access",
                    "Check Access Permissons",
                    $"Check access permissions for consumer with user id `{userId}`.",
                    correlationId.ToString(),
                    null),
                        Times.Once);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedAccessOrchestrationValidationException))),
                       Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
            this.consumerAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowUnauthorisedExceptionOnValidateAccessWhenConsumerIsDeleted()
        {
            // given
            string userId = GetRandomString();
            Consumer randomConsumer = CreateRandomConsumer(userId);
            randomConsumer.IsDeleted = true;
            Consumer inputConsumer = randomConsumer.DeepClone();
            Guid correlationId = Guid.NewGuid();

            IQueryable<Consumer> storageConsumers =
                new List<Consumer> { inputConsumer }.AsQueryable();

            string randomNhsNumber = GetRandomStringWithLength(5);
            string inputNhsNumber = randomNhsNumber;

            var unauthorizedAccessOrchestrationException =
                new UnauthorizedAccessOrchestrationException(
                    $"Current consumer with user id `{userId}` is not a valid consumer.");

            var expectedAccessOrchestrationValidationException =
                new AccessOrchestrationValidationException(
                    message: "Access orchestration validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: unauthorizedAccessOrchestrationException);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ReturnsAsync(storageConsumers);

            // when
            ValueTask<Access> validateAccessTask =
                accessOrchestrationService.ValidateAccess(userId, inputNhsNumber, correlationId);

            AccessOrchestrationValidationException actualAccessOrchestrationValidationException =
                await Assert.ThrowsAsync<AccessOrchestrationValidationException>(
                    testCode: validateAccessTask.AsTask);

            // then
            actualAccessOrchestrationValidationException
                .Should().BeEquivalentTo(expectedAccessOrchestrationValidationException);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Access",
                    "Check Access Permissons",
                    $"Check access permissions for consumer with user id `{userId}`.",
                    correlationId.ToString(),
                    null),
                        Times.Once);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedAccessOrchestrationValidationException))),
                       Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
            this.consumerAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowForbiddenExceptionOnValidateAccessWhenConsumerNotYetActive()
        {
            // given
            string userId = GetRandomString();
            Consumer randomConsumer = CreateRandomConsumer(userId);
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            randomConsumer.ActiveFrom = randomDateTimeOffset.AddDays(1);
            randomConsumer.ActiveTo = randomDateTimeOffset.AddDays(10);
            Consumer inputConsumer = randomConsumer.DeepClone();
            Guid correlationId = Guid.NewGuid();

            IQueryable<Consumer> storageConsumers =
                new List<Consumer> { inputConsumer }.AsQueryable();

            string randomNhsNumber = GetRandomStringWithLength(5);
            string inputNhsNumber = randomNhsNumber;

            var forbiddenAccessOrchestrationException =
                new ForbiddenAccessOrchestrationException(
                   "Current consumer is not active or does not have a valid access window.  " +
                        $"CorrelationId: {correlationId.ToString()}");

            var expectedAccessOrchestrationValidationException =
                new AccessOrchestrationValidationException(
                    message: "Access orchestration validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: forbiddenAccessOrchestrationException);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ReturnsAsync(storageConsumers);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Access> validateAccessTask =
                accessOrchestrationService.ValidateAccess(userId, inputNhsNumber, correlationId);

            AccessOrchestrationValidationException actualAccessOrchestrationValidationException =
                await Assert.ThrowsAsync<AccessOrchestrationValidationException>(
                    testCode: validateAccessTask.AsTask);

            // then
            actualAccessOrchestrationValidationException
                .Should().BeEquivalentTo(expectedAccessOrchestrationValidationException);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Access",
                    "Check Access Permissons",
                    $"Check access permissions for consumer with user id `{userId}`.",
                    correlationId.ToString(),
                    null),
                        Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Access",
                    "Access Forbidden",

                    $"Access was forbidden as consumer with user id {inputConsumer.UserId} " +
                        $"is not active / does not have a valid access window " +
                            $"(ActiveFrom: {inputConsumer.ActiveFrom}, ActiveTo: {inputConsumer.ActiveTo})  " +
                                $"CorrelationId: {correlationId.ToString()}",
                    correlationId.ToString(),
                    null),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
              broker.LogErrorAsync(It.Is(SameExceptionAs(
                  expectedAccessOrchestrationValidationException))),
                      Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
            this.consumerAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowForbiddenExceptionOnValidateAccessWhenInactiveConsumer()
        {
            // given
            string userId = GetRandomString();
            Consumer randomConsumer = CreateRandomConsumer(userId);
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            randomConsumer.ActiveFrom = randomDateTimeOffset.AddDays(-2);
            randomConsumer.ActiveTo = randomDateTimeOffset.AddDays(-2);
            Consumer inputConsumer = randomConsumer.DeepClone();
            Guid correlationId = Guid.NewGuid();

            IQueryable<Consumer> storageConsumers =
                new List<Consumer> { inputConsumer }.AsQueryable();

            string randomNhsNumber = GetRandomStringWithLength(5);
            string inputNhsNumber = randomNhsNumber;

            var forbiddenAccessOrchestrationException =
                new ForbiddenAccessOrchestrationException(
                   "Current consumer is not active or does not have a valid access window.  " +
                        $"CorrelationId: {correlationId.ToString()}");

            var expectedAccessOrchestrationValidationException =
                new AccessOrchestrationValidationException(
                    message: "Access orchestration validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: forbiddenAccessOrchestrationException);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ReturnsAsync(storageConsumers);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Access> validateAccessTask =
                accessOrchestrationService.ValidateAccess(userId, inputNhsNumber, correlationId);

            AccessOrchestrationValidationException actualAccessOrchestrationValidationException =
                await Assert.ThrowsAsync<AccessOrchestrationValidationException>(
                    testCode: validateAccessTask.AsTask);

            // then
            actualAccessOrchestrationValidationException
                .Should().BeEquivalentTo(expectedAccessOrchestrationValidationException);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Access",
                    "Check Access Permissons",
                    $"Check access permissions for consumer with user id `{userId}`.",
                    correlationId.ToString(),
                    null),
                        Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Access",
                    "Access Forbidden",

                    $"Access was forbidden as consumer with user id {inputConsumer.UserId} " +
                        $"is not active / does not have a valid access window " +
                            $"(ActiveFrom: {inputConsumer.ActiveFrom}, ActiveTo: {inputConsumer.ActiveTo})  " +
                                $"CorrelationId: {correlationId.ToString()}",
                    correlationId.ToString(),
                    null),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
              broker.LogErrorAsync(It.Is(SameExceptionAs(
                  expectedAccessOrchestrationValidationException))),
                      Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
            this.consumerAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
