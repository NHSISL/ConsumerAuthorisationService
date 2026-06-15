// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.SubscriberAgreements;
using FluentAssertions;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Apis.SubscriberAgreements
{
    public partial class SubscriberAgreementsApiTests
    {
        [Fact]
        public async Task ShouldPostSubscriberAgreementAsync()
        {
            // given
            SubscriberAgreement randomSubscriberAgreement = CreateRandomSubscriberAgreement();
            SubscriberAgreement inputSubscriberAgreement = randomSubscriberAgreement;
            SubscriberAgreement expectedSubscriberAgreement = inputSubscriberAgreement;

            // when
            SubscriberAgreement actualSubscriberAgreement =
                await this.apiBroker.PostSubscriberAgreementAsync(inputSubscriberAgreement);

            // then
            actualSubscriberAgreement.Should().BeEquivalentTo(expectedSubscriberAgreement,
                options => options
                    .Excluding(sa => sa.CreatedBy)
                    .Excluding(sa => sa.CreatedWhen)
                    .Excluding(sa => sa.UpdatedBy)
                    .Excluding(sa => sa.UpdatedWhen));
        }

        [Fact]
        public async Task ShouldGetAllSubscriberAgreementsAsync()
        {
            // given / when
            var actualSubscriberAgreements = await this.apiBroker.GetAllSubscriberAgreementsAsync();

            // then
            actualSubscriberAgreements.Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldGetSubscriberAgreementByIdAsync()
        {
            // given
            SubscriberAgreement randomSubscriberAgreement = CreateRandomSubscriberAgreement();
            SubscriberAgreement inputSubscriberAgreement = randomSubscriberAgreement;

            SubscriberAgreement postedSubscriberAgreement =
                await this.apiBroker.PostSubscriberAgreementAsync(inputSubscriberAgreement);

            // when
            SubscriberAgreement actualSubscriberAgreement =
                await this.apiBroker.GetSubscriberAgreementByIdAsync(postedSubscriberAgreement.Id);

            // then
            actualSubscriberAgreement.Should().NotBeNull();
            actualSubscriberAgreement.Id.Should().Be(postedSubscriberAgreement.Id);
        }

        [Fact]
        public async Task ShouldPutSubscriberAgreementAsync()
        {
            // given
            SubscriberAgreement randomSubscriberAgreement = CreateRandomSubscriberAgreement();
            SubscriberAgreement inputSubscriberAgreement = randomSubscriberAgreement;

            SubscriberAgreement postedSubscriberAgreement =
                await this.apiBroker.PostSubscriberAgreementAsync(inputSubscriberAgreement);

            SubscriberAgreement modifiedSubscriberAgreement = CreateRandomSubscriberAgreement();
            modifiedSubscriberAgreement.Id = postedSubscriberAgreement.Id;

            // when
            SubscriberAgreement actualSubscriberAgreement =
                await this.apiBroker.PutSubscriberAgreementAsync(modifiedSubscriberAgreement);

            // then
            actualSubscriberAgreement.Should().NotBeNull();
            actualSubscriberAgreement.Id.Should().Be(modifiedSubscriberAgreement.Id);
        }

        [Fact]
        public async Task ShouldDeleteSubscriberAgreementAsync()
        {
            // given
            SubscriberAgreement randomSubscriberAgreement = CreateRandomSubscriberAgreement();
            SubscriberAgreement inputSubscriberAgreement = randomSubscriberAgreement;

            SubscriberAgreement postedSubscriberAgreement =
                await this.apiBroker.PostSubscriberAgreementAsync(inputSubscriberAgreement);

            // when
            SubscriberAgreement deletedSubscriberAgreement =
                await this.apiBroker.DeleteSubscriberAgreementByIdAsync(postedSubscriberAgreement.Id);

            // then
            deletedSubscriberAgreement.Should().NotBeNull();
            deletedSubscriberAgreement.Id.Should().Be(postedSubscriberAgreement.Id);
        }
    }
}
