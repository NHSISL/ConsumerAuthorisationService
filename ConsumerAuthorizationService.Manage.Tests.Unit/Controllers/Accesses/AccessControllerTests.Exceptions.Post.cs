// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Manage.Models.Accesses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ConsumerAuthorizationService.Manage.Tests.Unit.Controllers.Accesses
{
    public partial class AccessControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            ValidateAccessRequest someRequest = CreateRandomValidateAccessRequest();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Access>(expectedBadRequestObjectResult);

            accessOrchestrationServiceMock
                .Setup(service => service.ValidateAccess(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Guid>(),
                    default))
                .ThrowsAsync(validationException);

            // when
            ActionResult<Access> actualActionResult =
                await accessController.PostAccessValidationAsync(someRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            accessOrchestrationServiceMock
                .Verify(service => service.ValidateAccess(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Guid>(),
                    default),
                    Times.Once);

            accessOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(Xeption serverException)
        {
            // given
            ValidateAccessRequest someRequest = CreateRandomValidateAccessRequest();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<Access>(expectedInternalServerErrorObjectResult);

            accessOrchestrationServiceMock
                .Setup(service => service.ValidateAccess(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Guid>(),
                    default))
                .ThrowsAsync(serverException);

            // when
            ActionResult<Access> actualActionResult =
                await accessController.PostAccessValidationAsync(someRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            accessOrchestrationServiceMock
                .Verify(service => service.ValidateAccess(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Guid>(),
                    default),
                    Times.Once);

            accessOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
