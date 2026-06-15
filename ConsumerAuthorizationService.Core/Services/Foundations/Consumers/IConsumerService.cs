// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;

namespace ConsumerAuthorizationService.Core.Services.Foundations.Consumers
{
    public interface IConsumerService
    {
        ValueTask<Consumer> AddConsumerAsync(Consumer consumer);
        ValueTask<IQueryable<Consumer>> RetrieveAllConsumersAsync();
        ValueTask<Consumer> RetrieveConsumerByIdAsync(Guid consumerId);
        ValueTask<Consumer> ModifyConsumerAsync(Consumer consumer);
        ValueTask<Consumer> RemoveConsumerByIdAsync(Guid consumerId);
    }
}
