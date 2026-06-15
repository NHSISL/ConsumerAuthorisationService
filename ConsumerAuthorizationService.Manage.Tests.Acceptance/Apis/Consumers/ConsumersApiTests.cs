// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Brokers;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.Consumers;
using Tynamix.ObjectFiller;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Apis.Consumers
{
    [Collection(nameof(ApiTestCollection))]
    public partial class ConsumersApiTests
    {
        private readonly ApiBroker apiBroker;

        public ConsumersApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static Consumer CreateRandomConsumer() =>
            CreateRandomConsumerFiller().Create();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static Filler<Consumer> CreateRandomConsumerFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Consumer>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use(now)
                .OnProperty(consumer => consumer.CreatedBy).Use(user)
                .OnProperty(consumer => consumer.UpdatedBy).Use(user);

            return filler;
        }
    }
}
