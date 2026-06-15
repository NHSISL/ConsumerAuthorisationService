// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;
using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ConsumerAuthorizationService.Manage.Tests.Unit.Controllers.Consumers
{
    public partial class ConsumersControllerTests
    {
        [Fact]
        public async Task ShouldReturnConsumerOnGetByIdAsync()
        {
            // given
            Consumer randomConsumer = CreateRandomConsumer();
            Guid inputId = randomConsumer.Id;
            Consumer storageConsumer = randomConsumer;
            Consumer expectedConsumer = storageConsumer.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedConsumer);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedObjectResult);

            consumerServiceMock
                .Setup(service => service.RetrieveConsumerByIdAsync(inputId))
                    .ReturnsAsync(storageConsumer);

            // when
            ActionResult<Consumer> actualActionResult =
                await consumersController.GetConsumerByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            consumerServiceMock
                .Verify(service => service.RetrieveConsumerByIdAsync(inputId),
                    Times.Once);

            consumerServiceMock.VerifyNoOtherCalls();
        }
    }
}
