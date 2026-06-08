// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ConsumerAuthorizationService.Core.Brokers.Loggings;
using ConsumerAuthorizationService.Core.Brokers.Storages.Snowflake;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Services.Foundations.Accesses;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Foundations.Accesses
{
    public partial class AccessServiceTests
    {
        private readonly Mock<ISnowflakeFhirStorageBroker> snowflakeFhirStorageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IAccessService accessService;

        public AccessServiceTests()
        {
            this.snowflakeFhirStorageBrokerMock = new Mock<ISnowflakeFhirStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.accessService = new AccessService(
                snowflakeFhirStorageBroker: this.snowflakeFhirStorageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static Guid GetRandomGuid() => Guid.NewGuid();

        private static List<string> GetRandomStringList() =>
            new Filler<List<string>>().Create();

        private static Access CreateRandomAccess() =>
            CreateAccessFiller().Create();

        private static List<Access> CreateRandomAccesses() =>
            CreateAccessFiller().Create(count: GetRandomNumber()).ToList();

        private static Filler<Access> CreateAccessFiller()
        {
            var filler = new Filler<Access>();

            filler.Setup()
                .OnType<List<string>>().Use(GetRandomStringList)
                .OnType<List<AccessReason>>().IgnoreIt();

            return filler;
        }
    }
}
