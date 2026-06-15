// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions;
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
            try
            {
                SubscriberAgreement addedSubscriberAgreement =
                    await this.subscriberAgreementService.AddSubscriberAgreementAsync(subscriberAgreement);

                return Created(addedSubscriberAgreement);
            }
            catch (SubscriberAgreementValidationException subscriberAgreementValidationException)
            {
                return BadRequest(subscriberAgreementValidationException.InnerException);
            }
            catch (SubscriberAgreementServiceDependencyValidationException subscriberAgreementDependencyValidationException)
                when (subscriberAgreementDependencyValidationException.InnerException is AlreadyExistsSubscriberAgreementException)
            {
                return Conflict(subscriberAgreementDependencyValidationException.InnerException);
            }
            catch (SubscriberAgreementServiceDependencyValidationException subscriberAgreementDependencyValidationException)
            {
                return BadRequest(subscriberAgreementDependencyValidationException.InnerException);
            }
            catch (SubscriberAgreementServiceDependencyException subscriberAgreementDependencyException)
            {
                return InternalServerError(subscriberAgreementDependencyException);
            }
            catch (SubscriberAgreementServiceException subscriberAgreementServiceException)
            {
                return InternalServerError(subscriberAgreementServiceException);
            }
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
            try
            {
                IQueryable<SubscriberAgreement> subscriberAgreements =
                    await this.subscriberAgreementService.RetrieveAllSubscriberAgreementsAsync();

                return Ok(subscriberAgreements);
            }
            catch (SubscriberAgreementServiceDependencyException subscriberAgreementDependencyException)
            {
                return InternalServerError(subscriberAgreementDependencyException);
            }
            catch (SubscriberAgreementServiceException subscriberAgreementServiceException)
            {
                return InternalServerError(subscriberAgreementServiceException);
            }
        }

        [HttpGet("{subscriberAgreementId}")]
        public async ValueTask<ActionResult<SubscriberAgreement>> GetSubscriberAgreementByIdAsync(
            Guid subscriberAgreementId)
        {
            try
            {
                SubscriberAgreement subscriberAgreement =
                    await this.subscriberAgreementService.RetrieveSubscriberAgreementByIdAsync(subscriberAgreementId);

                return Ok(subscriberAgreement);
            }
            catch (SubscriberAgreementValidationException subscriberAgreementValidationException)
                when (subscriberAgreementValidationException.InnerException is NotFoundSubscriberAgreementException)
            {
                return NotFound(subscriberAgreementValidationException.InnerException);
            }
            catch (SubscriberAgreementValidationException subscriberAgreementValidationException)
            {
                return BadRequest(subscriberAgreementValidationException.InnerException);
            }
            catch (SubscriberAgreementServiceDependencyValidationException subscriberAgreementDependencyValidationException)
            {
                return BadRequest(subscriberAgreementDependencyValidationException.InnerException);
            }
            catch (SubscriberAgreementServiceDependencyException subscriberAgreementDependencyException)
            {
                return InternalServerError(subscriberAgreementDependencyException);
            }
            catch (SubscriberAgreementServiceException subscriberAgreementServiceException)
            {
                return InternalServerError(subscriberAgreementServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<SubscriberAgreement>> PutSubscriberAgreementAsync(
            [FromBody] SubscriberAgreement subscriberAgreement)
        {
            try
            {
                SubscriberAgreement modifiedSubscriberAgreement =
                    await this.subscriberAgreementService.ModifySubscriberAgreementAsync(subscriberAgreement);

                return Ok(modifiedSubscriberAgreement);
            }
            catch (SubscriberAgreementValidationException subscriberAgreementValidationException)
                when (subscriberAgreementValidationException.InnerException is NotFoundSubscriberAgreementException)
            {
                return NotFound(subscriberAgreementValidationException.InnerException);
            }
            catch (SubscriberAgreementValidationException subscriberAgreementValidationException)
            {
                return BadRequest(subscriberAgreementValidationException.InnerException);
            }
            catch (SubscriberAgreementServiceDependencyValidationException subscriberAgreementDependencyValidationException)
                when (subscriberAgreementDependencyValidationException.InnerException is AlreadyExistsSubscriberAgreementException)
            {
                return Conflict(subscriberAgreementDependencyValidationException.InnerException);
            }
            catch (SubscriberAgreementServiceDependencyValidationException subscriberAgreementDependencyValidationException)
            {
                return BadRequest(subscriberAgreementDependencyValidationException.InnerException);
            }
            catch (SubscriberAgreementServiceDependencyException subscriberAgreementDependencyException)
            {
                return InternalServerError(subscriberAgreementDependencyException);
            }
            catch (SubscriberAgreementServiceException subscriberAgreementServiceException)
            {
                return InternalServerError(subscriberAgreementServiceException);
            }
        }

        [HttpDelete("{subscriberAgreementId}")]
        public async ValueTask<ActionResult<SubscriberAgreement>> DeleteSubscriberAgreementByIdAsync(
            Guid subscriberAgreementId)
        {
            try
            {
                SubscriberAgreement deletedSubscriberAgreement =
                    await this.subscriberAgreementService.RemoveSubscriberAgreementByIdAsync(subscriberAgreementId);

                return Ok(deletedSubscriberAgreement);
            }
            catch (SubscriberAgreementValidationException subscriberAgreementValidationException)
                when (subscriberAgreementValidationException.InnerException is NotFoundSubscriberAgreementException)
            {
                return NotFound(subscriberAgreementValidationException.InnerException);
            }
            catch (SubscriberAgreementValidationException subscriberAgreementValidationException)
            {
                return BadRequest(subscriberAgreementValidationException.InnerException);
            }
            catch (SubscriberAgreementServiceDependencyValidationException subscriberAgreementDependencyValidationException)
                when (subscriberAgreementDependencyValidationException.InnerException is LockedSubscriberAgreementException)
            {
                return Locked(subscriberAgreementDependencyValidationException.InnerException);
            }
            catch (SubscriberAgreementServiceDependencyValidationException subscriberAgreementDependencyValidationException)
            {
                return BadRequest(subscriberAgreementDependencyValidationException.InnerException);
            }
            catch (SubscriberAgreementServiceDependencyException subscriberAgreementDependencyException)
            {
                return InternalServerError(subscriberAgreementDependencyException);
            }
            catch (SubscriberAgreementServiceException subscriberAgreementServiceException)
            {
                return InternalServerError(subscriberAgreementServiceException);
            }
        }
    }
}
