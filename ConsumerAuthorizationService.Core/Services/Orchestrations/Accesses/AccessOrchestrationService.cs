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
using ISL.Security.Client.Models.Foundations.Users;

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
            string nhsNumber,
            Guid correlationId,
            CancellationToken cancellationToken = default) =>
            TryCatch(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                ValidateArguments(nhsNumber, correlationId);
                User currentUser = await securityBroker.GetCurrentUserAsync();
                string currentUserId = currentUser.UserId;

                JsonSerializerOptions options = new()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                string currentUserJson = JsonSerializer.Serialize(currentUser, options);

                await this.auditBroker.LogInformationAsync(
                    auditType: "Access",
                    title: "Check Access Permissons",
                    message: currentUserJson,
                    fileName: null,
                    correlationId: correlationId.ToString());

                if (currentUser is null)
                {
                    throw new UnauthorizedAccessOrchestrationException(
                        $"Current consumer is not a valid consumer.");
                }

                IQueryable<Consumer> consumers = await consumerService.RetrieveAllConsumersAsync();

                Consumer matchingConsumer = consumers
                    .FirstOrDefault(consumer => consumer.UserId == currentUserId && consumer.IsDeleted == false);

                if (matchingConsumer is null)
                {
                    throw new UnauthorizedAccessOrchestrationException(
                        $"Current consumer with id `{currentUserId}` is not a valid consumer.");
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
                                $"Access was forbidden as consumer with id {matchingConsumer.Id} " +
                                $"is not active / does not have valid access window " +
                                $"(ActiveFrom: {matchingConsumer.ActiveFrom}, ActiveTo: {matchingConsumer.ActiveTo})  " +
                                $"CorrelationId: {correlationId.ToString()}",

                            fileName: null,
                            correlationId: correlationId.ToString());

                    throw new ForbiddenAccessOrchestrationException(
                        "Current consumer is not active or does not have a valid access window.  " +
                        $"CorrelationId: {correlationId.ToString()}");
                }

                IQueryable<SubscriberAgreement> subscriberAgreementsQueryable =
                    await this.subscriberAgreementService.RetrieveAllSubscriberAgreementsAsync();

                subscriberAgreementsQueryable = subscriberAgreementsQueryable.Where(
                    subscriberAgreement => subscriberAgreement.IsDeleted == false &&
                        subscriberAgreement.ConsumerId == currentUserId);

                List<SubscriberAgreement> subscriberAgreements = subscriberAgreementsQueryable.ToList();

                Access access = await this.accessService.ValidateConsumerAccessToPatientAsync(
                    nhsNumber: nhsNumber,
                    consumerId: currentUserId,
                    subscriberAgreementIds: subscriberAgreements.Select(sa => sa.Id.ToString()).ToList(),
                    correlationId,
                    cancellationToken);

                return access;
            });
    }
}
