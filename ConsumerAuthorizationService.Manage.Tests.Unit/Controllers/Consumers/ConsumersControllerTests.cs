// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers.Exceptions;
using ConsumerAuthorizationService.Core.Services.Foundations.Consumers;
using ConsumerAuthorizationService.Manage.Controllers;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ConsumerAuthorizationService.Manage.Tests.Unit.Controllers.Consumers
{
    public partial class ConsumersControllerTests : RESTFulController
    {
        private readonly Mock<IConsumerService> consumerServiceMock;
        private readonly ConsumersController consumersController;

        public ConsumersControllerTests()
        {
            consumerServiceMock = new Mock<IConsumerService>();
            consumersController = new ConsumersController(consumerServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new ConsumerValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new ConsumerDependencyValidationException(
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
                new ConsumerDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new ConsumerServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static Consumer CreateRandomConsumer() =>
            CreateConsumerFiller().Create();

        private static IQueryable<Consumer> CreateRandomConsumers() =>
            CreateConsumerFiller().Create(count: GetRandomNumber()).AsQueryable();

        private static Filler<Consumer> CreateConsumerFiller()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<Consumer>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(c => c.SubscriberAgreements).IgnoreIt()
                .OnProperty(c => c.CreatedBy).Use(user)
                .OnProperty(c => c.UpdatedBy).Use(user);

            return filler;
        }
    }
}
