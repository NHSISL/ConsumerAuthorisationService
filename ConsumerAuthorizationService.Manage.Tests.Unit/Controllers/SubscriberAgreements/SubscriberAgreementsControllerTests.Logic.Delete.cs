// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ConsumerAuthorizationService.Manage.Tests.Unit.Controllers.SubscriberAgreements
{
    public partial class SubscriberAgreementsControllerTests
    {
        [Fact]
        public async Task ShouldRemoveSubscriberAgreementOnDeleteByIdAsync()
        {
            // given
            SubscriberAgreement randomSubscriberAgreement = CreateRandomSubscriberAgreement();
            Guid inputId = randomSubscriberAgreement.Id;
            SubscriberAgreement storageSubscriberAgreement = randomSubscriberAgreement;
            SubscriberAgreement expectedSubscriberAgreement = storageSubscriberAgreement.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedSubscriberAgreement);

            var expectedActionResult =
                new ActionResult<SubscriberAgreement>(expectedObjectResult);

            subscriberAgreementServiceMock
                .Setup(service => service.RemoveSubscriberAgreementByIdAsync(inputId))
                    .ReturnsAsync(storageSubscriberAgreement);

            // when
            ActionResult<SubscriberAgreement> actualActionResult =
                await subscriberAgreementsController.DeleteSubscriberAgreementByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            subscriberAgreementServiceMock
                .Verify(service => service.RemoveSubscriberAgreementByIdAsync(inputId),
                    Times.Once);

            subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
