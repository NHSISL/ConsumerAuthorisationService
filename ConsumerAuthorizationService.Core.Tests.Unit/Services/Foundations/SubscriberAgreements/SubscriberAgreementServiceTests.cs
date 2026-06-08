// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ConsumerAuthorizationService.Core.Brokers.DateTimes;
using ConsumerAuthorizationService.Core.Brokers.Loggings;
using ConsumerAuthorizationService.Core.Brokers.Securities;
using ConsumerAuthorizationService.Core.Brokers.Storages.Sql;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Services.Foundations.SubscriberAgreements;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Foundations.SubscriberAgreements
{
    public partial class SubscriberAgreementServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ISecurityAuditBroker> securityAuditBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ISubscriberAgreementService subscriberAgreementService;

        public SubscriberAgreementServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityAuditBrokerMock = new Mock<ISecurityAuditBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.subscriberAgreementService = new SubscriberAgreementService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                securityAuditBroker: this.securityAuditBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static IQueryable<SubscriberAgreement> CreateRandomSubscriberAgreements() =>
            CreateSubscriberAgreementFiller(dateTimeOffset: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                .AsQueryable();

        private static SubscriberAgreement CreateRandomModifySubscriberAgreement(
            DateTimeOffset dateTimeOffset,
            string userId = "")
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            SubscriberAgreement randomSubscriberAgreement = CreateRandomSubscriberAgreement(dateTimeOffset, userId);
            randomSubscriberAgreement.CreatedWhen = randomSubscriberAgreement.CreatedWhen.AddDays(randomDaysInPast);

            return randomSubscriberAgreement;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        public static TheoryData<int> MinutesBeforeOrAfter()
        {
            int randomNumber = GetRandomNumber();
            int randomNegativeNumber = GetRandomNegativeNumber();

            return new TheoryData<int>
            {
                randomNumber,
                randomNegativeNumber
            };
        }

        private static SqlException GetSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(typeof(SqlException));

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static SubscriberAgreement CreateRandomSubscriberAgreement() =>
            CreateSubscriberAgreementFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static SubscriberAgreement CreateRandomSubscriberAgreement(
            DateTimeOffset dateTimeOffset,
            string userId = "") =>
            CreateSubscriberAgreementFiller(dateTimeOffset, userId).Create();

        private static Filler<SubscriberAgreement> CreateSubscriberAgreementFiller(
            DateTimeOffset dateTimeOffset,
            string userId = "")
        {
            userId = string.IsNullOrEmpty(userId) ? Guid.NewGuid().ToString() : userId;
            var filler = new Filler<SubscriberAgreement>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(sa => sa.ConsumerId).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(sa => sa.SubscriberAgreementId).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(sa => sa.SubscriberAgreementName).Use(GetRandomStringWithLengthOf(255))
                .OnProperty(sa => sa.CreatedBy).Use(userId)
                .OnProperty(sa => sa.UpdatedBy).Use(userId)
                .OnProperty(sa => sa.Consumer).IgnoreIt();

            return filler;
        }
    }
}
