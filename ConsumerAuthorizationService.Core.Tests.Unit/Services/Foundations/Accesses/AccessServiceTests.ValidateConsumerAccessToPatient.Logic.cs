// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using Moq;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Foundations.Accesses
{
    public partial class AccessServiceTests
    {
        [Fact]
        public async Task ShouldValidateConsumerAccessToPatientAsync()
        {
            // given
            string randomNhsNumber = GetRandomString();
            string randomConsumerId = GetRandomString();
            List<string> randomSubscriberAgreementIds = GetRandomStringList();
            Guid randomCorrelationId = GetRandomGuid();
            CancellationToken cancellationToken = CancellationToken.None;

            List<Access> randomAccesses = CreateRandomAccesses();
            Access expectedAccess = randomAccesses[0];

            this.snowflakeFhirStorageBrokerMock.Setup(broker =>
                broker.ValidateConsumerAccessToPatientAsync(
                    randomNhsNumber,
                    randomConsumerId,
                    randomSubscriberAgreementIds,
                    randomCorrelationId,
                    cancellationToken))
                .ReturnsAsync(randomAccesses);

            // when
            Access actualAccess = await this.accessService.ValidateConsumerAccessToPatientAsync(
                nhsNumber: randomNhsNumber,
                consumerUserId: randomConsumerId,
                subscriberAgreementIds: randomSubscriberAgreementIds,
                correlationId: randomCorrelationId,
                cancellationToken: cancellationToken);

            // then
            actualAccess.Should().BeEquivalentTo(expectedAccess);

            this.snowflakeFhirStorageBrokerMock.Verify(broker =>
                broker.ValidateConsumerAccessToPatientAsync(
                    randomNhsNumber,
                    randomConsumerId,
                    randomSubscriberAgreementIds,
                    randomCorrelationId,
                    cancellationToken),
                Times.Once);

            this.snowflakeFhirStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNullWhenBrokerReturnsEmptyListAsync()
        {
            // given
            string randomNhsNumber = GetRandomString();
            string randomConsumerId = GetRandomString();
            List<string> randomSubscriberAgreementIds = GetRandomStringList();
            Guid randomCorrelationId = GetRandomGuid();
            CancellationToken cancellationToken = CancellationToken.None;

            List<Access> emptyAccesses = new List<Access>();

            this.snowflakeFhirStorageBrokerMock.Setup(broker =>
                broker.ValidateConsumerAccessToPatientAsync(
                    randomNhsNumber,
                    randomConsumerId,
                    randomSubscriberAgreementIds,
                    randomCorrelationId,
                    cancellationToken))
                .ReturnsAsync(emptyAccesses);

            // when
            Access actualAccess = await this.accessService.ValidateConsumerAccessToPatientAsync(
                nhsNumber: randomNhsNumber,
                consumerUserId: randomConsumerId,
                subscriberAgreementIds: randomSubscriberAgreementIds,
                correlationId: randomCorrelationId,
                cancellationToken: cancellationToken);

            // then
            actualAccess.Should().BeNull();

            this.snowflakeFhirStorageBrokerMock.Verify(broker =>
                broker.ValidateConsumerAccessToPatientAsync(
                    randomNhsNumber,
                    randomConsumerId,
                    randomSubscriberAgreementIds,
                    randomCorrelationId,
                    cancellationToken),
                Times.Once);

            this.snowflakeFhirStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
