// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
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
        public async Task ShouldReturnSubscriberAgreementsOnGetAsync()
        {
            // given
            IQueryable<SubscriberAgreement> randomSubscriberAgreements = CreateRandomSubscriberAgreements();
            IQueryable<SubscriberAgreement> storageSubscriberAgreements = randomSubscriberAgreements.DeepClone();
            IQueryable<SubscriberAgreement> expectedSubscriberAgreements = storageSubscriberAgreements.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedSubscriberAgreements);

            var expectedActionResult =
                new ActionResult<IQueryable<SubscriberAgreement>>(expectedObjectResult);

            subscriberAgreementServiceMock
                .Setup(service => service.RetrieveAllSubscriberAgreementsAsync())
                    .ReturnsAsync(storageSubscriberAgreements);

            // when
            ActionResult<IQueryable<SubscriberAgreement>> actualActionResult =
                await subscriberAgreementsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            subscriberAgreementServiceMock
                .Verify(service => service.RetrieveAllSubscriberAgreementsAsync(),
                    Times.Once);

            subscriberAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
