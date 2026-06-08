// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Brokers.DateTimes;
using ConsumerAuthorizationService.Core.Brokers.Loggings;
using ConsumerAuthorizationService.Core.Brokers.Securities;
using ConsumerAuthorizationService.Core.Brokers.Storages.Sql;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;

namespace ConsumerAuthorizationService.Core.Services.Foundations.SubscriberAgreements
{
    internal partial class SubscriberAgreementService : ISubscriberAgreementService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityAuditBroker securityAuditBroker;
        private readonly ILoggingBroker loggingBroker;

        public SubscriberAgreementService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityAuditBroker securityAuditBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityAuditBroker = securityAuditBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<SubscriberAgreement> AddSubscriberAgreementAsync(SubscriberAgreement subscriberAgreement) =>
            TryCatch(async () =>
            {
                subscriberAgreement = await this.securityAuditBroker.ApplyAddAuditValuesAsync(subscriberAgreement);
                await ValidateSubscriberAgreementOnAdd(subscriberAgreement);

                return await this.storageBroker.InsertSubscriberAgreementAsync(subscriberAgreement);
            });

        public ValueTask<IQueryable<SubscriberAgreement>> RetrieveAllSubscriberAgreementsAsync() =>
            TryCatch(async () => await this.storageBroker.SelectAllSubscriberAgreementsAsync());

        public ValueTask<SubscriberAgreement> RetrieveSubscriberAgreementByIdAsync(Guid subscriberAgreementId) =>
            TryCatch(async () =>
            {
                ValidateSubscriberAgreementId(subscriberAgreementId);

                SubscriberAgreement maybeSubscriberAgreement =
                    await this.storageBroker.SelectSubscriberAgreementByIdAsync(subscriberAgreementId);

                ValidateStorageSubscriberAgreement(maybeSubscriberAgreement, subscriberAgreementId);

                return maybeSubscriberAgreement;
            });

        public ValueTask<SubscriberAgreement> ModifySubscriberAgreementAsync(SubscriberAgreement subscriberAgreement) =>
            TryCatch(async () =>
            {
                subscriberAgreement =
                    await this.securityAuditBroker.ApplyModifyAuditValuesAsync(subscriberAgreement);

                await ValidateSubscriberAgreementOnModify(subscriberAgreement);

                SubscriberAgreement maybeSubscriberAgreement =
                    await this.storageBroker.SelectSubscriberAgreementByIdAsync(subscriberAgreement.Id);

                ValidateStorageSubscriberAgreement(maybeSubscriberAgreement, subscriberAgreement.Id);

                subscriberAgreement = await this.securityAuditBroker
                    .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(subscriberAgreement, maybeSubscriberAgreement);

                ValidateAgainstStorageSubscriberAgreementOnModify(
                    inputSubscriberAgreement: subscriberAgreement,
                    storageSubscriberAgreement: maybeSubscriberAgreement);

                return await this.storageBroker.UpdateSubscriberAgreementAsync(subscriberAgreement);
            });

        public ValueTask<SubscriberAgreement> RemoveSubscriberAgreementByIdAsync(Guid subscriberAgreementId) =>
            TryCatch(async () =>
            {
                ValidateSubscriberAgreementId(subscriberAgreementId);

                SubscriberAgreement maybeSubscriberAgreement =
                    await this.storageBroker.SelectSubscriberAgreementByIdAsync(subscriberAgreementId);

                ValidateStorageSubscriberAgreement(maybeSubscriberAgreement, subscriberAgreementId);
                maybeSubscriberAgreement =
                    await this.securityAuditBroker.ApplyRemoveAuditValuesAsync(maybeSubscriberAgreement);

                return await this.storageBroker.UpdateSubscriberAgreementAsync(maybeSubscriberAgreement);
            });
    }
}
