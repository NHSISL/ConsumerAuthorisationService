// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ConsumerAuthorizationService.Manage.Tests.Unit.Controllers.SubscriberAgreements
{
    public partial class SubscriberAgreementsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<SubscriberAgreement>(expectedBadRequestObjectResult);

            this.subscriberAgreementServiceMock.Setup(service =>
                service.RemoveSubscriberAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<SubscriberAgreement> actualActionResult =
                await this.subscriberAgreementsController.DeleteSubscriberAgreementByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.subscriberAgreementServiceMock.Verify(service =>
                service.RemoveSubscriberAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnDeleteIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<SubscriberAgreement>(expectedInternalServerErrorObjectResult);

            this.subscriberAgreementServiceMock.Setup(service =>
                service.RemoveSubscriberAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<SubscriberAgreement> actualActionResult =
                await this.subscriberAgreementsController.DeleteSubscriberAgreementByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.subscriberAgreementServiceMock.Verify(service =>
                service.RemoveSubscriberAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfSubscriberAgreementDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundSubscriberAgreementException =
                new NotFoundSubscriberAgreementException(
                    message: someMessage);

            var subscriberAgreementValidationException =
                new SubscriberAgreementValidationException(
                    message: someMessage,
                    innerException: notFoundSubscriberAgreementException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundSubscriberAgreementException);

            var expectedActionResult =
                new ActionResult<SubscriberAgreement>(expectedNotFoundObjectResult);

            this.subscriberAgreementServiceMock.Setup(service =>
                service.RemoveSubscriberAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(subscriberAgreementValidationException);

            // when
            ActionResult<SubscriberAgreement> actualActionResult =
                await this.subscriberAgreementsController.DeleteSubscriberAgreementByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.subscriberAgreementServiceMock.Verify(service =>
                service.RemoveSubscriberAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfSubscriberAgreementIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var lockedSubscriberAgreementException =
                new LockedSubscriberAgreementException(
                    message: someMessage,
                    innerException: someInnerException);

            var subscriberAgreementDependencyValidationException =
                new SubscriberAgreementServiceDependencyValidationException(
                    message: someMessage,
                    innerException: lockedSubscriberAgreementException);

            LockedObjectResult expectedLockedObjectResult =
                Locked(lockedSubscriberAgreementException);

            var expectedActionResult =
                new ActionResult<SubscriberAgreement>(expectedLockedObjectResult);

            this.subscriberAgreementServiceMock.Setup(service =>
                service.RemoveSubscriberAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(subscriberAgreementDependencyValidationException);

            // when
            ActionResult<SubscriberAgreement> actualActionResult =
                await this.subscriberAgreementsController.DeleteSubscriberAgreementByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.subscriberAgreementServiceMock.Verify(service =>
                service.RemoveSubscriberAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
