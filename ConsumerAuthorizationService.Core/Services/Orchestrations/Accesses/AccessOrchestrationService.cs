// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Brokers.Audits;
using ConsumerAuthorizationService.Core.Brokers.DateTimes;
using ConsumerAuthorizationService.Core.Brokers.Identifiers;
using ConsumerAuthorizationService.Core.Brokers.Loggings;
using ConsumerAuthorizationService.Core.Brokers.Securities;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Models.Orchestrations.Accesses;
using ConsumerAuthorizationService.Core.Models.Orchestrations.Accesses.Exceptions;
using ConsumerAuthorizationService.Core.Services.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Services.Foundations.Consumers;
using ConsumerAuthorizationService.Core.Services.Foundations.SubscriberAgreements;

namespace ConsumerAuthorizationService.Core.Services.Orchestrations.Accesses
{
    internal partial class AccessOrchestrationService : IAccessOrchestrationService
    {
        private readonly IConsumerService consumerService;
        private readonly ISubscriberAgreementService subscriberAgreementService;
        private readonly IAccessService accessService;
        private readonly IAuditBroker auditBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IIdentifierBroker identifierBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly AccessConfigurations accessConfigurations;

        public AccessOrchestrationService(
            IConsumerService consumerService,
            ISubscriberAgreementService subscriberAgreementService,
            IAccessService accessService,
            IAuditBroker auditBroker,
            ISecurityBroker securityBroker,
            IDateTimeBroker dateTimeBroker,
            IIdentifierBroker identifierBroker,
            ILoggingBroker loggingBroker,
            AccessConfigurations accessConfigurations)
        {
            this.consumerService = consumerService;
            this.subscriberAgreementService = subscriberAgreementService;
            this.accessService = accessService;
            this.auditBroker = auditBroker;
            this.securityBroker = securityBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.identifierBroker = identifierBroker;
            this.loggingBroker = loggingBroker;
            this.accessConfigurations = accessConfigurations;
        }

        public ValueTask<Access> ValidateAccess(
            string consumerUserId,
            string nhsNumber,
            Guid correlationId,
            CancellationToken cancellationToken = default) =>
            TryCatch(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                ValidateArguments(consumerUserId, nhsNumber, correlationId);
                //User currentUser = await securityBroker.GetCurrentUserAsync();
                //string currentUserId = currentUser.UserId;

                JsonSerializerOptions options = new()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                await this.auditBroker.LogInformationAsync(
                    auditType: "Access",
                    title: "Check Access Permissons",
                    message: $"Check access permissions for consumer with user id `{consumerUserId}`.",
                    correlationId: correlationId.ToString(),
                    fileName: null);

                IQueryable<Consumer> consumers = await consumerService.RetrieveAllConsumersAsync();

                Consumer? matchingConsumer = consumers
                    .FirstOrDefault(consumer => consumer.UserId == consumerUserId && consumer.IsDeleted == false);

                if (matchingConsumer is null)
                {
                    throw new UnauthorizedAccessOrchestrationException(
                        $"Current consumer with user id `{consumerUserId}` is not a valid consumer.");
                }

                DateTimeOffset now = await dateTimeBroker.GetCurrentDateTimeOffsetAsync();

                bool isActive =
                    matchingConsumer.ActiveFrom <= now
                        && (!matchingConsumer.ActiveTo.HasValue || matchingConsumer.ActiveTo >= now);

                if (!isActive)
                {
                    await this.auditBroker.LogInformationAsync(
                            auditType: "Access",
                            title: "Access Forbidden",

                            message:
                                $"Access was forbidden as consumer with user id {matchingConsumer.UserId} " +
                                $"is not active / does not have a valid access window " +
                                $"(ActiveFrom: {matchingConsumer.ActiveFrom}, ActiveTo: {matchingConsumer.ActiveTo})  " +
                                $"CorrelationId: {correlationId.ToString()}",

                            correlationId: correlationId.ToString(),
                            fileName: null);

                    throw new ForbiddenAccessOrchestrationException(
                        "Current consumer is not active or does not have a valid access window.  " +
                        $"CorrelationId: {correlationId.ToString()}");
                }

                IQueryable<SubscriberAgreement> subscriberAgreementsQueryable =
                    await this.subscriberAgreementService.RetrieveAllSubscriberAgreementsAsync();

                subscriberAgreementsQueryable = subscriberAgreementsQueryable.Where(
                    subscriberAgreement => subscriberAgreement.IsDeleted == false &&
                        subscriberAgreement.ConsumerId == matchingConsumer.Id);

                List<SubscriberAgreement> subscriberAgreements = subscriberAgreementsQueryable.ToList();

                Access? access = await this.accessService.ValidateConsumerAccessToPatientAsync(
                    nhsNumber: nhsNumber,
                    consumerUserId: matchingConsumer.UserId,

                    subscriberAgreementIds:
                        [.. subscriberAgreements.Select(subscriberAgreement => subscriberAgreement.Id.ToString())],

                    correlationId,
                    cancellationToken);

                return access!;
            });
    }
}
