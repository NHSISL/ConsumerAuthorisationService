// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;

namespace ConsumerAuthorizationService.Core.Services.Orchestrations.Accesses
{
    public interface IAccessOrchestrationService
    {
        public ValueTask<Access> ValidateAccess(
            string consumerUserId,
            string nhsNumber,
            Guid correlationId,
            CancellationToken cancellationToken = default);
    }
}
