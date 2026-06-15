// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Brokers;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.SubscriberAgreements;
using Tynamix.ObjectFiller;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Apis.SubscriberAgreements
{
    [Collection(nameof(ApiTestCollection))]
    public partial class SubscriberAgreementsApiTests
    {
        private readonly ApiBroker apiBroker;

        public SubscriberAgreementsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static SubscriberAgreement CreateRandomSubscriberAgreement() =>
            CreateRandomSubscriberAgreementFiller().Create();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static Filler<SubscriberAgreement> CreateRandomSubscriberAgreementFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<SubscriberAgreement>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use(now)
                .OnProperty(sa => sa.CreatedBy).Use(user)
                .OnProperty(sa => sa.UpdatedBy).Use(user);

            return filler;
        }
    }
}
