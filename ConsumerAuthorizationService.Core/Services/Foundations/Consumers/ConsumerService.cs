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
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;

namespace ConsumerAuthorizationService.Core.Services.Foundations.Consumers
{
    internal partial class ConsumerService : IConsumerService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityAuditBroker securityAuditBroker;
        private readonly ILoggingBroker loggingBroker;

        public ConsumerService(
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

        public ValueTask<Consumer> AddConsumerAsync(Consumer consumer) =>
            TryCatch(async () =>
            {
                consumer = await this.securityAuditBroker.ApplyAddAuditValuesAsync(consumer);
                await ValidateConsumerOnAdd(consumer);

                return await this.storageBroker.InsertConsumerAsync(consumer);
            });

        public ValueTask<IQueryable<Consumer>> RetrieveAllConsumersAsync() =>
            TryCatch(async () => await this.storageBroker.SelectAllConsumersAsync());

        public ValueTask<Consumer> RetrieveConsumerByIdAsync(Guid consumerId) =>
            TryCatch(async () =>
            {
                ValidateConsumerId(consumerId);

                Consumer maybeConsumer = await this.storageBroker
                    .SelectConsumerByIdAsync(consumerId);

                ValidateStorageConsumer(maybeConsumer, consumerId);

                return maybeConsumer;
            });

        public ValueTask<Consumer> ModifyConsumerAsync(Consumer consumer) =>
            TryCatch(async () =>
            {
                consumer = await this.securityAuditBroker.ApplyModifyAuditValuesAsync(consumer);
                await ValidateConsumerOnModify(consumer);

                Consumer maybeConsumer =
                    await this.storageBroker.SelectConsumerByIdAsync(consumer.Id);

                ValidateStorageConsumer(maybeConsumer, consumer.Id);

                consumer = await this.securityAuditBroker
                    .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(consumer, maybeConsumer);

                ValidateAgainstStorageConsumerOnModify(
                    inputConsumer: consumer,
                    storageConsumer: maybeConsumer);

                return await this.storageBroker.UpdateConsumerAsync(consumer);
            });

        public ValueTask<Consumer> RemoveConsumerByIdAsync(Guid consumerId) =>
            TryCatch(async () =>
            {
                ValidateConsumerId(consumerId);
                Consumer maybeConsumer = await this.storageBroker.SelectConsumerByIdAsync(consumerId);
                ValidateStorageConsumer(maybeConsumer, consumerId);
                maybeConsumer = await this.securityAuditBroker.ApplyRemoveAuditValuesAsync(maybeConsumer);

                return await this.storageBroker.UpdateConsumerAsync(maybeConsumer);
            });
    }
}
