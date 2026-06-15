// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Services.Orchestrations.Accesses;
using ConsumerAuthorizationService.Manage.Models.Accesses;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace ConsumerAuthorizationService.Manage.Controllers
{
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
            throw new NotImplementedException();
        }
    }
}
