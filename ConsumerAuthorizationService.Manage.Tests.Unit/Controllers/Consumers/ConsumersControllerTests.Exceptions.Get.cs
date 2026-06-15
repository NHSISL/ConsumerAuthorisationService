// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ConsumerAuthorizationService.Manage.Tests.Unit.Controllers.Consumers
{
    public partial class ConsumersControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnGetByIdIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedBadRequestObjectResult);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveConsumerByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Consumer> actualActionResult =
                await this.consumersController.GetConsumerByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveConsumerByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetByIdIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedInternalServerErrorObjectResult);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveConsumerByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<Consumer> actualActionResult =
                await this.consumersController.GetConsumerByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveConsumerByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByIdIfConsumerDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundConsumerException =
                new NotFoundConsumerException(
                    message: someMessage);

            var consumerValidationException =
                new ConsumerValidationException(
                    message: someMessage,
                    innerException: notFoundConsumerException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundConsumerException);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedNotFoundObjectResult);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveConsumerByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(consumerValidationException);

            // when
            ActionResult<Consumer> actualActionResult =
                await this.consumersController.GetConsumerByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveConsumerByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
        }
    }
}
