// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.Consumers;
using FluentAssertions;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Apis.Consumers
{
    public partial class ConsumersApiTests
    {
        [Fact]
        public async Task ShouldPostConsumerAsync()
        {
            // given
            Consumer randomConsumer = CreateRandomConsumer();
            Consumer inputConsumer = randomConsumer;
            Consumer expectedConsumer = inputConsumer;

            // when
            Consumer actualConsumer = await this.apiBroker.PostConsumerAsync(inputConsumer);

            // then
            actualConsumer.Should().BeEquivalentTo(expectedConsumer,
                options => options
                    .Excluding(consumer => consumer.CreatedBy)
                    .Excluding(consumer => consumer.CreatedWhen)
                    .Excluding(consumer => consumer.UpdatedBy)
                    .Excluding(consumer => consumer.UpdatedWhen));
        }

        [Fact]
        public async Task ShouldGetAllConsumersAsync()
        {
            // given / when
            var actualConsumers = await this.apiBroker.GetAllConsumersAsync();

            // then
            actualConsumers.Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldGetConsumerByIdAsync()
        {
            // given
            Consumer randomConsumer = CreateRandomConsumer();
            Consumer inputConsumer = randomConsumer;
            Consumer postedConsumer = await this.apiBroker.PostConsumerAsync(inputConsumer);

            // when
            Consumer actualConsumer = await this.apiBroker.GetConsumerByIdAsync(postedConsumer.Id);

            // then
            actualConsumer.Should().NotBeNull();
            actualConsumer.Id.Should().Be(postedConsumer.Id);
        }

        [Fact]
        public async Task ShouldPutConsumerAsync()
        {
            // given
            Consumer randomConsumer = CreateRandomConsumer();
            Consumer inputConsumer = randomConsumer;
            Consumer postedConsumer = await this.apiBroker.PostConsumerAsync(inputConsumer);
            Consumer modifiedConsumer = CreateRandomConsumer();
            modifiedConsumer.Id = postedConsumer.Id;

            // when
            Consumer actualConsumer = await this.apiBroker.PutConsumerAsync(modifiedConsumer);

            // then
            actualConsumer.Should().NotBeNull();
            actualConsumer.Id.Should().Be(modifiedConsumer.Id);
        }

        [Fact]
        public async Task ShouldDeleteConsumerAsync()
        {
            // given
            Consumer randomConsumer = CreateRandomConsumer();
            Consumer inputConsumer = randomConsumer;
            Consumer postedConsumer = await this.apiBroker.PostConsumerAsync(inputConsumer);

            // when
            Consumer deletedConsumer = await this.apiBroker.DeleteConsumerByIdAsync(postedConsumer.Id);

            // then
            deletedConsumer.Should().NotBeNull();
            deletedConsumer.Id.Should().Be(postedConsumer.Id);
        }
    }
}
