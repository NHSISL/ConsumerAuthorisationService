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
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Consumer someConsumer = CreateRandomConsumer();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedBadRequestObjectResult);

            this.consumerServiceMock.Setup(service =>
                service.ModifyConsumerAsync(It.IsAny<Consumer>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Consumer> actualActionResult =
                await this.consumersController.PutConsumerAsync(someConsumer);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerServiceMock.Verify(service =>
                service.ModifyConsumerAsync(It.IsAny<Consumer>()),
                    Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            Consumer someConsumer = CreateRandomConsumer();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedInternalServerErrorObjectResult);

            this.consumerServiceMock.Setup(service =>
                service.ModifyConsumerAsync(It.IsAny<Consumer>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<Consumer> actualActionResult =
                await this.consumersController.PutConsumerAsync(someConsumer);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerServiceMock.Verify(service =>
                service.ModifyConsumerAsync(It.IsAny<Consumer>()),
                    Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfConsumerDoesNotExistAsync()
        {
            // given
            Consumer someConsumer = CreateRandomConsumer();
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
                service.ModifyConsumerAsync(It.IsAny<Consumer>()))
                    .ThrowsAsync(consumerValidationException);

            // when
            ActionResult<Consumer> actualActionResult =
                await this.consumersController.PutConsumerAsync(someConsumer);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerServiceMock.Verify(service =>
                service.ModifyConsumerAsync(It.IsAny<Consumer>()),
                    Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsConsumerErrorOccurredAsync()
        {
            // given
            Consumer someConsumer = CreateRandomConsumer();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsConsumerException =
                new AlreadyExistsConsumerException(
                    message: someMessage,
                    innerException: someInnerException);

            var consumerDependencyValidationException =
                new ConsumerDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsConsumerException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsConsumerException);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedConflictObjectResult);

            this.consumerServiceMock.Setup(service =>
                service.ModifyConsumerAsync(It.IsAny<Consumer>()))
                    .ThrowsAsync(consumerDependencyValidationException);

            // when
            ActionResult<Consumer> actualActionResult =
                await this.consumersController.PutConsumerAsync(someConsumer);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerServiceMock.Verify(service =>
                service.ModifyConsumerAsync(It.IsAny<Consumer>()),
                    Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
        }
    }
}
