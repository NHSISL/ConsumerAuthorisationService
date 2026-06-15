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
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            SubscriberAgreement someSubscriberAgreement = CreateRandomSubscriberAgreement();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<SubscriberAgreement>(expectedBadRequestObjectResult);

            this.subscriberAgreementServiceMock.Setup(service =>
                service.ModifySubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<SubscriberAgreement> actualActionResult =
                await this.subscriberAgreementsController.PutSubscriberAgreementAsync(someSubscriberAgreement);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.subscriberAgreementServiceMock.Verify(service =>
                service.ModifySubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Once);

            this.subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            SubscriberAgreement someSubscriberAgreement = CreateRandomSubscriberAgreement();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<SubscriberAgreement>(expectedInternalServerErrorObjectResult);

            this.subscriberAgreementServiceMock.Setup(service =>
                service.ModifySubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<SubscriberAgreement> actualActionResult =
                await this.subscriberAgreementsController.PutSubscriberAgreementAsync(someSubscriberAgreement);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.subscriberAgreementServiceMock.Verify(service =>
                service.ModifySubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Once);

            this.subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfSubscriberAgreementDoesNotExistAsync()
        {
            // given
            SubscriberAgreement someSubscriberAgreement = CreateRandomSubscriberAgreement();
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
                service.ModifySubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()))
                    .ThrowsAsync(subscriberAgreementValidationException);

            // when
            ActionResult<SubscriberAgreement> actualActionResult =
                await this.subscriberAgreementsController.PutSubscriberAgreementAsync(someSubscriberAgreement);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.subscriberAgreementServiceMock.Verify(service =>
                service.ModifySubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Once);

            this.subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsSubscriberAgreementErrorOccurredAsync()
        {
            // given
            SubscriberAgreement someSubscriberAgreement = CreateRandomSubscriberAgreement();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsSubscriberAgreementException =
                new AlreadyExistsSubscriberAgreementException(
                    message: someMessage,
                    innerException: someInnerException);

            var subscriberAgreementDependencyValidationException =
                new SubscriberAgreementServiceDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsSubscriberAgreementException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsSubscriberAgreementException);

            var expectedActionResult =
                new ActionResult<SubscriberAgreement>(expectedConflictObjectResult);

            this.subscriberAgreementServiceMock.Setup(service =>
                service.ModifySubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()))
                    .ThrowsAsync(subscriberAgreementDependencyValidationException);

            // when
            ActionResult<SubscriberAgreement> actualActionResult =
                await this.subscriberAgreementsController.PutSubscriberAgreementAsync(someSubscriberAgreement);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.subscriberAgreementServiceMock.Verify(service =>
                service.ModifySubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Once);

            this.subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
