// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions;
using ConsumerAuthorizationService.Core.Services.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Manage.Controllers;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ConsumerAuthorizationService.Manage.Tests.Unit.Controllers.SubscriberAgreements
{
    public partial class SubscriberAgreementsControllerTests : RESTFulController
    {
        private readonly Mock<ISubscriberAgreementService> subscriberAgreementServiceMock;
        private readonly SubscriberAgreementsController subscriberAgreementsController;

        public SubscriberAgreementsControllerTests()
        {
            subscriberAgreementServiceMock = new Mock<ISubscriberAgreementService>();
            subscriberAgreementsController = new SubscriberAgreementsController(subscriberAgreementServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new SubscriberAgreementValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new SubscriberAgreementServiceDependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new SubscriberAgreementServiceDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new SubscriberAgreementServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static SubscriberAgreement CreateRandomSubscriberAgreement() =>
            CreateSubscriberAgreementFiller().Create();

        private static IQueryable<SubscriberAgreement> CreateRandomSubscriberAgreements() =>
            CreateSubscriberAgreementFiller().Create(count: GetRandomNumber()).AsQueryable();

        private static Filler<SubscriberAgreement> CreateSubscriberAgreementFiller()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<SubscriberAgreement>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(s => s.Consumer).IgnoreIt()
                .OnProperty(s => s.CreatedBy).Use(user)
                .OnProperty(s => s.UpdatedBy).Use(user);

            return filler;
        }
    }
}
