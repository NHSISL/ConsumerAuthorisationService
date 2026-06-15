// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Models.Orchestrations.Accesses.Exceptions;
using ConsumerAuthorizationService.Core.Services.Orchestrations.Accesses;
using ConsumerAuthorizationService.Manage.Models.Accesses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace ConsumerAuthorizationService.Manage.Controllers
{
    [Authorize(Roles = "Administrators,Users")]
    [ApiController]
    [Route("api/[controller]")]
    public class AccessController : RESTFulController
    {
        private readonly IAccessOrchestrationService accessOrchestrationService;

        public AccessController(IAccessOrchestrationService accessOrchestrationService) =>
            this.accessOrchestrationService = accessOrchestrationService;

        [HttpPost]
        public async ValueTask<ActionResult<Access>> PostAccessValidationAsync(
            [FromBody] ValidateAccessRequest request)
        {
            try
            {
                Access access = await this.accessOrchestrationService.ValidateAccess(
                    request.ConsumerUserId,
                    request.NhsNumber,
                    request.CorrelationId);

                return Ok(access);
            }
            catch (AccessOrchestrationValidationException accessOrchestrationValidationException)
            {
                return BadRequest(accessOrchestrationValidationException.InnerException);
            }
            catch (AccessOrchestrationDependencyValidationException accessOrchestrationDependencyValidationException)
            {
                return BadRequest(accessOrchestrationDependencyValidationException.InnerException);
            }
            catch (AccessOrchestrationDependencyException accessOrchestrationDependencyException)
            {
                return InternalServerError(accessOrchestrationDependencyException);
            }
            catch (AccessOrchestrationServiceException accessOrchestrationServiceException)
            {
                return InternalServerError(accessOrchestrationServiceException);
            }
        }
    }
}
