// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;

namespace ConsumerAuthorizationService.Core.Services.Foundations.Accesses
{
    internal interface IAccessService
    {
        ValueTask<Access?> ValidateConsumerAccessToPatientAsync(
            string nhsNumber,
            string consumerId,
            List<string> subscriberAgreementIds,
            Guid correlationId,
            CancellationToken cancellationToken);
    }
}
