// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;

namespace ConsumerAuthorizationService.Core.Brokers.Storages.Snowflake
{
    public partial interface ISnowflakeFhirStorageBroker
    {
        ValueTask<List<Access>> ValidateConsumerAccessToPatientAsync(
            string nhsNumber,
            string consumerUserId,
            List<string> subscriberAgreementIds,
            Guid correlationId,
            CancellationToken cancellationToken);
    }
}
