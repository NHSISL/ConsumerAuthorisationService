// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using Moq;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Foundations.SubscriberAgreements
{
    public partial class SubscriberAgreementServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllSubscriberAgreementsAsync()
        {
            // given
            IQueryable<SubscriberAgreement> randomSubscriberAgreements = CreateRandomSubscriberAgreements();
            IQueryable<SubscriberAgreement> storageSubscriberAgreements = randomSubscriberAgreements;
            IQueryable<SubscriberAgreement> expectedSubscriberAgreements = storageSubscriberAgreements;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSubscriberAgreementsAsync())
                    .ReturnsAsync(storageSubscriberAgreements);

            // when
            IQueryable<SubscriberAgreement> actualSubscriberAgreements =
                await this.subscriberAgreementService.RetrieveAllSubscriberAgreementsAsync();

            // then
            actualSubscriberAgreements.Should().BeEquivalentTo(expectedSubscriberAgreements);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSubscriberAgreementsAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
