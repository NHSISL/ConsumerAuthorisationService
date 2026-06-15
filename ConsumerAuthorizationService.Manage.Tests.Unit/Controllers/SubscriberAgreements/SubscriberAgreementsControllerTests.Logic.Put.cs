// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            SubscriberAgreement randomSubscriberAgreement = CreateRandomSubscriberAgreement();
            SubscriberAgreement inputSubscriberAgreement = randomSubscriberAgreement;
            SubscriberAgreement storageSubscriberAgreement = inputSubscriberAgreement.DeepClone();
            SubscriberAgreement expectedSubscriberAgreement = storageSubscriberAgreement.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedSubscriberAgreement);

            var expectedActionResult =
                new ActionResult<SubscriberAgreement>(expectedObjectResult);

            subscriberAgreementServiceMock
                .Setup(service => service.ModifySubscriberAgreementAsync(inputSubscriberAgreement))
                    .ReturnsAsync(storageSubscriberAgreement);

            // when
            ActionResult<SubscriberAgreement> actualActionResult =
                await subscriberAgreementsController.PutSubscriberAgreementAsync(randomSubscriberAgreement);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            subscriberAgreementServiceMock
                .Verify(service => service.ModifySubscriberAgreementAsync(inputSubscriberAgreement),
                    Times.Once);

            subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
