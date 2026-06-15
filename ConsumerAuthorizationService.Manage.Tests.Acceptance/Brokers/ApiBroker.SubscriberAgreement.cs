// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.SubscriberAgreements;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string SubscriberAgreementsRelativeUrl = "api/subscriberagreements";

        public async ValueTask<SubscriberAgreement> PostSubscriberAgreementAsync(
            SubscriberAgreement subscriberAgreement) =>
            await this.apiFactoryClient.PostContentAsync(SubscriberAgreementsRelativeUrl, subscriberAgreement);

        public async ValueTask<List<SubscriberAgreement>> GetAllSubscriberAgreementsAsync() =>
            await this.apiFactoryClient
                .GetContentAsync<List<SubscriberAgreement>>($"{SubscriberAgreementsRelativeUrl}/");

        public async ValueTask<SubscriberAgreement> GetSubscriberAgreementByIdAsync(Guid subscriberAgreementId) =>
            await this.apiFactoryClient
                .GetContentAsync<SubscriberAgreement>(
                    $"{SubscriberAgreementsRelativeUrl}/{subscriberAgreementId}");

        public async ValueTask<SubscriberAgreement> PutSubscriberAgreementAsync(
            SubscriberAgreement subscriberAgreement) =>
            await this.apiFactoryClient.PutContentAsync(SubscriberAgreementsRelativeUrl, subscriberAgreement);

        public async ValueTask<SubscriberAgreement> DeleteSubscriberAgreementByIdAsync(
            Guid subscriberAgreementId) =>
            await this.apiFactoryClient
                .DeleteContentAsync<SubscriberAgreement>(
                    $"{SubscriberAgreementsRelativeUrl}/{subscriberAgreementId}");
    }
}
