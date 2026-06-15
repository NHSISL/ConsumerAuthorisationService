// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.Consumers;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string ConsumersRelativeUrl = "api/consumers";

        public async ValueTask<Consumer> PostConsumerAsync(Consumer consumer) =>
            await this.apiFactoryClient.PostContentAsync(ConsumersRelativeUrl, consumer);

        public async ValueTask<List<Consumer>> GetAllConsumersAsync() =>
            await this.apiFactoryClient
                .GetContentAsync<List<Consumer>>($"{ConsumersRelativeUrl}/");

        public async ValueTask<Consumer> GetConsumerByIdAsync(Guid consumerId) =>
            await this.apiFactoryClient
                .GetContentAsync<Consumer>($"{ConsumersRelativeUrl}/{consumerId}");

        public async ValueTask<Consumer> PutConsumerAsync(Consumer consumer) =>
            await this.apiFactoryClient.PutContentAsync(ConsumersRelativeUrl, consumer);

        public async ValueTask<Consumer> DeleteConsumerByIdAsync(Guid consumerId) =>
            await this.apiFactoryClient
                .DeleteContentAsync<Consumer>($"{ConsumersRelativeUrl}/{consumerId}");
    }
}
