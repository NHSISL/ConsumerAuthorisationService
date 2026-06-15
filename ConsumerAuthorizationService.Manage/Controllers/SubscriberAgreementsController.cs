// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Services.Foundations.SubscriberAgreements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ConsumerAuthorizationService.Manage.Controllers
{
    [Authorize(Roles = "Administrators,Users")]
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriberAgreementsController : RESTFulController
    {
        private readonly ISubscriberAgreementService subscriberAgreementService;

        public SubscriberAgreementsController(ISubscriberAgreementService subscriberAgreementService) =>
            this.subscriberAgreementService = subscriberAgreementService;

        [HttpPost]
        public async ValueTask<ActionResult<SubscriberAgreement>> PostSubscriberAgreementAsync(
            [FromBody] SubscriberAgreement subscriberAgreement)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
#if !DEBUG
        [EnableQuery(PageSize = 50)]
#endif
#if DEBUG
        [EnableQuery(PageSize = 5000)]
#endif
        public async ValueTask<ActionResult<IQueryable<SubscriberAgreement>>> Get()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{subscriberAgreementId}")]
        public async ValueTask<ActionResult<SubscriberAgreement>> GetSubscriberAgreementByIdAsync(
            Guid subscriberAgreementId)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public async ValueTask<ActionResult<SubscriberAgreement>> PutSubscriberAgreementAsync(
            [FromBody] SubscriberAgreement subscriberAgreement)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{subscriberAgreementId}")]
        public async ValueTask<ActionResult<SubscriberAgreement>> DeleteSubscriberAgreementByIdAsync(
            Guid subscriberAgreementId)
        {
            throw new NotImplementedException();
        }
    }
}
