// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Models.Orchestrations.Accesses.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnValidateAccessAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            string randomConsumerUserId = GetRandomString();
            string randomNhsNumber = GetRandomString();
            string inputNhsNumber = randomNhsNumber;
            Guid correlationId = Guid.NewGuid();

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ThrowsAsync(dependencyException);

            var expectedAccessOrchestrationDependencyValidationException =
                new AccessOrchestrationDependencyValidationException(
                    message: "Access orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            // when
            ValueTask<Access> validateAccessTask =
                accessOrchestrationService.ValidateAccess(randomConsumerUserId, inputNhsNumber, correlationId, TestContext.Current.CancellationToken);

            AccessOrchestrationDependencyValidationException actualAccessOrchestrationDependencyValidationException =
                await Assert.ThrowsAsync<AccessOrchestrationDependencyValidationException>(
                    testCode: validateAccessTask.AsTask);

            // then
            actualAccessOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedAccessOrchestrationDependencyValidationException);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    null),
                        Times.Once);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedAccessOrchestrationDependencyValidationException))),
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

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnValidateAccessAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            string randomConsumerUserId = GetRandomString();
            string randomNhsNumber = GetRandomString();
            string inputNhsNumber = randomNhsNumber;
            Guid correlationId = Guid.NewGuid();

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ThrowsAsync(dependencyException);

            var expectedAccessOrchestrationDependencyException =
                new AccessOrchestrationDependencyException(
                    message: "Access orchestration dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            // when
            ValueTask<Access> validateAccessTask =
                accessOrchestrationService.ValidateAccess(randomConsumerUserId, inputNhsNumber, correlationId, TestContext.Current.CancellationToken);

            AccessOrchestrationDependencyException actualAccessOrchestrationDependencyException =
                await Assert.ThrowsAsync<AccessOrchestrationDependencyException>(
                    testCode: validateAccessTask.AsTask);

            // then
            actualAccessOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedAccessOrchestrationDependencyException);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    null),
                        Times.Once);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedAccessOrchestrationDependencyException))),
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
        public async Task ShouldThrowServiceExceptionOnValidateAccessAndLogItAsync()
        {
            // given
            var serviceException = new Exception();
            string randomConsumerUserId = GetRandomString();
            string randomNhsNumber = GetRandomString();
            string inputNhsNumber = randomNhsNumber;
            Guid correlationId = Guid.NewGuid();

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ThrowsAsync(serviceException);

            var failedServiceAccessOrchestrationException =
                new FailedServiceAccessOrchestrationException(
                    message: "Failed access orchestration service error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedAccessOrchestrationServiceException =
                new AccessOrchestrationServiceException(
                    message: "Access orchestration service error occurred, please contact support.",
                    innerException: failedServiceAccessOrchestrationException);

            // when
            ValueTask<Access> validateAccessTask =
                accessOrchestrationService.ValidateAccess(randomConsumerUserId, inputNhsNumber, correlationId, TestContext.Current.CancellationToken);

            AccessOrchestrationServiceException actualAccessOrchestrationServiceException =
                await Assert.ThrowsAsync<AccessOrchestrationServiceException>(
                    testCode: validateAccessTask.AsTask);

            // then
            actualAccessOrchestrationServiceException
                .Should().BeEquivalentTo(expectedAccessOrchestrationServiceException);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    null),
                        Times.Once);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedAccessOrchestrationServiceException))),
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
