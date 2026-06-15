// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;
using ConsumerAuthorizationService.Core.Services.Foundations.Consumers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ConsumerAuthorizationService.Manage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumersController : RESTFulController
    {
        private readonly IConsumerService consumerService;

        public ConsumersController(IConsumerService consumerService) =>
            this.consumerService = consumerService;

        [HttpPost]
        public async ValueTask<ActionResult<Consumer>> PostConsumerAsync([FromBody] Consumer consumer)
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
        public async ValueTask<ActionResult<IQueryable<Consumer>>> Get()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{consumerId}")]
        public async ValueTask<ActionResult<Consumer>> GetConsumerByIdAsync(Guid consumerId)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public async ValueTask<ActionResult<Consumer>> PutConsumerAsync([FromBody] Consumer consumer)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{consumerId}")]
        public async ValueTask<ActionResult<Consumer>> DeleteConsumerByIdAsync(Guid consumerId)
        {
            throw new NotImplementedException();
        }
    }
}
