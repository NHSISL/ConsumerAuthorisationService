// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Brokers;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.Accesses;
using Tynamix.ObjectFiller;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Apis.Accesses
{
    [Collection(nameof(ApiTestCollection))]
    public partial class AccessesApiTests
    {
        private readonly ApiBroker apiBroker;

        public AccessesApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static ValidateAccessRequest CreateRandomValidateAccessRequest()
        {
            return new ValidateAccessRequest
            {
                ConsumerUserId = GetRandomString(),
                NhsNumber = GetRandomString(),
                CorrelationId = Guid.NewGuid()
            };
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();
    }
}
