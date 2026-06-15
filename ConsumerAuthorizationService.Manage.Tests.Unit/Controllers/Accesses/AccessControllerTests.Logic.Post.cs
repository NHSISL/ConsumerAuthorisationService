// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Manage.Models.Accesses;
using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace ConsumerAuthorizationService.Manage.Tests.Unit.Controllers.Accesses
{
    public partial class AccessControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPostAsync()
        {
            // given
            ValidateAccessRequest randomRequest = CreateRandomValidateAccessRequest();
            ValidateAccessRequest inputRequest = randomRequest;
            Access randomAccess = CreateRandomAccess();
            Access returnedAccess = randomAccess;
            Access expectedAccess = returnedAccess.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedAccess);

            var expectedActionResult =
                new ActionResult<Access>(expectedObjectResult);

            accessOrchestrationServiceMock
                .Setup(service => service.ValidateAccess(
                    inputRequest.ConsumerUserId,
                    inputRequest.NhsNumber,
                    inputRequest.CorrelationId,
                    default))
                .ReturnsAsync(returnedAccess);

            // when
            ActionResult<Access> actualActionResult =
                await accessController.PostAccessValidationAsync(inputRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            accessOrchestrationServiceMock
                .Verify(service => service.ValidateAccess(
                    inputRequest.ConsumerUserId,
                    inputRequest.NhsNumber,
                    inputRequest.CorrelationId,
                    default),
                    Times.Once);

            accessOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
