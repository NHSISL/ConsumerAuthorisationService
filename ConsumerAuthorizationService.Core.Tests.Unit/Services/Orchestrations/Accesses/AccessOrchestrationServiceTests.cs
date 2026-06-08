// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using ConsumerAuthorizationService.Core.Brokers.Audits;
using ConsumerAuthorizationService.Core.Brokers.DateTimes;
using ConsumerAuthorizationService.Core.Brokers.Identifiers;
using ConsumerAuthorizationService.Core.Brokers.Loggings;
using ConsumerAuthorizationService.Core.Brokers.Securities;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses.Exceptions;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers.Exceptions;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions;
using ConsumerAuthorizationService.Core.Models.Orchestrations.Accesses;
using ConsumerAuthorizationService.Core.Services.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Services.Foundations.Consumers;
using ConsumerAuthorizationService.Core.Services.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Services.Orchestrations.Accesses;
using ISL.Security.Client.Models.Foundations.Users;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        private readonly Mock<IConsumerService> consumerServiceMock;
        private readonly Mock<ISubscriberAgreementService> consumerAccessServiceMock;
        private readonly Mock<IAccessService> pdsDataServiceMock;
        private readonly Mock<IAuditBroker> auditBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<IIdentifierBroker> identifierBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly AccessConfigurations accessConfigurations;
        private readonly AccessOrchestrationService accessOrchestrationService;

        public AccessOrchestrationServiceTests()
        {
            this.consumerServiceMock = new Mock<IConsumerService>();
            this.consumerAccessServiceMock = new Mock<ISubscriberAgreementService>();
            this.pdsDataServiceMock = new Mock<IAccessService>();
            this.auditBrokerMock = new Mock<IAuditBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.identifierBrokerMock = new Mock<IIdentifierBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.accessConfigurations = new AccessConfigurations
            {
                UseHashedNhsNumber = true,
                HashPepper = GetRandomStringWithLength(100),
                CheckAccessPermissions = true
            };

            this.accessOrchestrationService = new AccessOrchestrationService(
                consumerService: this.consumerServiceMock.Object,
                subscriberAgreementService: this.consumerAccessServiceMock.Object,
                accessService: this.pdsDataServiceMock.Object,
                auditBroker: this.auditBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                identifierBroker: this.identifierBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                accessConfigurations: this.accessConfigurations);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLength(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static User CreateRandomUser(string userId)
        {
            string randomString = GetRandomString();

            User user = new User(
                userId: userId,
                givenName: randomString,
                surname: randomString,
                displayName: randomString,
                email: randomString,
                jobTitle: randomString,
                roles: new List<string> { randomString },
                claims: CreateRandomClaims());

            return user;
        }

        private static List<Claim> CreateRandomClaims()
        {
            string randomString = GetRandomString();

            return Enumerable.Range(start: 1, count: GetRandomNumber())
                .Select(_ => new Claim(type: randomString, value: randomString)).ToList();
        }

        private static Consumer CreateRandomConsumer(string userId = "") =>
            CreateConsumerFiller(GetRandomDateTimeOffset(), userId).Create();

        private static Filler<Consumer> CreateConsumerFiller(DateTimeOffset dateTimeOffset, string userId = "")
        {
            userId = string.IsNullOrEmpty(userId) ? Guid.NewGuid().ToString() : userId;
            var filler = new Filler<Consumer>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(consumer => consumer.UserId).Use(userId)
                .OnProperty(consumer => consumer.Name).Use(GetRandomStringWithLength(255))
                .OnProperty(consumer => consumer.CreatedBy).Use(userId)
                .OnProperty(consumer => consumer.UpdatedBy).Use(userId)
                .OnProperty(consumer => consumer.IsDeleted).Use(false)
                .OnProperty(consumer => consumer.SubscriberAgreements).IgnoreIt();

            return filler;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(
            Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again",
                    innerException),

                new ConsumerDependencyValidationException(
                    message: "Consumer dependency validation occurred, please try again.",
                    innerException),

                new SubscriberAgreementValidationException(
                    message: "Subscriber agreement validation errors occurred, please try again",
                    innerException),

                new SubscriberAgreementServiceDependencyValidationException(
                    message: "Subscriber agreement dependency validation occurred, please try again.",
                    innerException),

                new AccessValidationException(
                    message: "Access validation errors occurred, please try again.",
                    innerException),

                new AccessDependencyValidationException(
                    message: "Access dependency validation errors occurred, please try again.",
                    innerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new ConsumerDependencyException(
                    message: "Consumer dependency error occurred, please contact support.",
                    innerException),

                new ConsumerServiceException(
                    message: "Consumer service error occurred, please contact support.",
                    innerException),

                new SubscriberAgreementServiceDependencyException(
                    message: "Subscriber agreement dependency error occurred, please contact support.",
                    innerException),

                new SubscriberAgreementServiceException(
                    message: "Subscriber agreement service error occurred, please contact support.",
                    innerException),

                new AccessDependencyException(
                    message: "Access dependency error occurred, please contact support.",
                    innerException),

                new AccessServiceException(
                    message: "Access service error occurred, please contact support.",
                    innerException),
            };
        }
    }
}
