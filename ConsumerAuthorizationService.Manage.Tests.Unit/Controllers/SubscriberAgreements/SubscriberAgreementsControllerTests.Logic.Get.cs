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
        public async Task ShouldReturnSubscriberAgreementOnGetByIdAsync()
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
                .Setup(service => service.RetrieveSubscriberAgreementByIdAsync(inputId))
                    .ReturnsAsync(storageSubscriberAgreement);

            // when
            ActionResult<SubscriberAgreement> actualActionResult =
                await subscriberAgreementsController.GetSubscriberAgreementByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            subscriberAgreementServiceMock
                .Verify(service => service.RetrieveSubscriberAgreementByIdAsync(inputId),
                    Times.Once);

            subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
