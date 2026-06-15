// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Models.Orchestrations.Accesses.Exceptions;
using ConsumerAuthorizationService.Core.Services.Orchestrations.Accesses;
using ConsumerAuthorizationService.Manage.Controllers;
using ConsumerAuthorizationService.Manage.Models.Accesses;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ConsumerAuthorizationService.Manage.Tests.Unit.Controllers.Accesses
{
    public partial class AccessControllerTests : RESTFulController
    {
        private readonly Mock<IAccessOrchestrationService> accessOrchestrationServiceMock;
        private readonly AccessController accessController;

        public AccessControllerTests()
        {
            accessOrchestrationServiceMock = new Mock<IAccessOrchestrationService>();
            accessController = new AccessController(accessOrchestrationServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new AccessOrchestrationValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new AccessOrchestrationDependencyValidationException(
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
                new AccessOrchestrationDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new AccessOrchestrationServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static Guid GetRandomGuid() => Guid.NewGuid();

        private static ValidateAccessRequest CreateRandomValidateAccessRequest() =>
            new ValidateAccessRequest
            {
                ConsumerUserId = GetRandomString(),
                NhsNumber = GetRandomString(),
                CorrelationId = GetRandomGuid()
            };

        private static Access CreateRandomAccess() =>
            new Access
            {
                NhsNumber = GetRandomString(),
                ConsumerId = GetRandomString(),
                IsAccessAllowed = true,
                CorrelationId = GetRandomGuid()
            };
    }
}
