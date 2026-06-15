// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Brokers.Loggings;
using ConsumerAuthorizationService.Core.Brokers.Storages.Snowflake;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;

namespace ConsumerAuthorizationService.Core.Services.Foundations.Accesses
{
    internal partial class AccessService : IAccessService
    {
        private readonly ISnowflakeFhirStorageBroker snowflakeFhirStorageBroker;
        private readonly ILoggingBroker loggingBroker;

        public AccessService(
            ISnowflakeFhirStorageBroker snowflakeFhirStorageBroker,
            ILoggingBroker loggingBroker)
        {
            this.snowflakeFhirStorageBroker = snowflakeFhirStorageBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Access?> ValidateConsumerAccessToPatientAsync(
            string nhsNumber,
            string consumerUserId,
            List<string> subscriberAgreementIds,
            Guid correlationId,
            CancellationToken cancellationToken) =>
            TryCatch(async () =>
            {
                ValidateAccessOnValidate(nhsNumber, consumerUserId, subscriberAgreementIds, correlationId);

                List<Access> accesses = await this.snowflakeFhirStorageBroker
                    .ValidateConsumerAccessToPatientAsync(
                        nhsNumber,
                        consumerUserId,
                        subscriberAgreementIds,
                        correlationId,
                        cancellationToken);

                return accesses.FirstOrDefault();
            });
    }
}
