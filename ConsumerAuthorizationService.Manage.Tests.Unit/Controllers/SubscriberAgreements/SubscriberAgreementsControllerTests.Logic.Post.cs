// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace ConsumerAuthorizationService.Manage.Tests.Unit.Controllers.SubscriberAgreements
{
    public partial class SubscriberAgreementsControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            SubscriberAgreement randomSubscriberAgreement = CreateRandomSubscriberAgreement();
            SubscriberAgreement inputSubscriberAgreement = randomSubscriberAgreement;
            SubscriberAgreement addedSubscriberAgreement = inputSubscriberAgreement.DeepClone();
            SubscriberAgreement expectedSubscriberAgreement = addedSubscriberAgreement.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedSubscriberAgreement);

            var expectedActionResult =
                new ActionResult<SubscriberAgreement>(expectedObjectResult);

            subscriberAgreementServiceMock
                .Setup(service => service.AddSubscriberAgreementAsync(inputSubscriberAgreement))
                    .ReturnsAsync(addedSubscriberAgreement);

            // when
            ActionResult<SubscriberAgreement> actualActionResult =
                await subscriberAgreementsController.PostSubscriberAgreementAsync(randomSubscriberAgreement);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            subscriberAgreementServiceMock
                .Verify(service => service.AddSubscriberAgreementAsync(inputSubscriberAgreement),
                    Times.Once);

            subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
