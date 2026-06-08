// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using FluentAssertions;
using Force.DeepCloner;
using ISL.Security.Client.Models.Foundations.Users;
using Moq;
using Tynamix.ObjectFiller;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldValidateAccess()
        {
            // given
            string userId = GetRandomString();
            User randomUser = CreateRandomUser(userId);
            User outputUser = randomUser;
            Consumer randomConsumer = CreateRandomConsumer(userId);
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset validActiveFromDate = randomDateTimeOffset.AddDays(-2);
            DateTimeOffset validActiveToDate = randomDateTimeOffset.AddDays(2);
            randomConsumer.ActiveFrom = validActiveFromDate;
            randomConsumer.ActiveTo = validActiveToDate;
            Consumer inputConsumer = randomConsumer.DeepClone();
            Guid correlationId = Guid.NewGuid();

            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            string currentUserJson = JsonSerializer.Serialize(outputUser, options);

            IQueryable<Consumer> storageConsumers =
                new List<Consumer> { inputConsumer }.AsQueryable();

            string randomNhsNumber = GetRandomStringWithLength(5);
            string inputNhsNumber = randomNhsNumber;

            SubscriberAgreement randomSubscriberAgreement = CreateRandomSubscriberAgreement(userId);
            IQueryable<SubscriberAgreement> storageSubscriberAgreements =
                new List<SubscriberAgreement> { randomSubscriberAgreement }.AsQueryable();

            List<string> expectedSubscriberAgreementIds =
                new List<string> { randomSubscriberAgreement.Id.ToString() };

            Access randomAccess = CreateRandomAccess();
            Access expectedAccess = randomAccess;

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(outputUser);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ReturnsAsync(storageConsumers);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.consumerAccessServiceMock.Setup(service =>
                service.RetrieveAllSubscriberAgreementsAsync())
                    .ReturnsAsync(storageSubscriberAgreements);

            this.pdsDataServiceMock.Setup(service =>
                service.ValidateConsumerAccessToPatientAsync(
                    inputNhsNumber,
                    userId,
                    It.Is<List<string>>(ids => ids.SequenceEqual(expectedSubscriberAgreementIds)),
                    correlationId,
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomAccess);

            // when
            Access actualAccess = await accessOrchestrationService.ValidateAccess(inputNhsNumber, correlationId);

            // then
            actualAccess.Should().BeEquivalentTo(expectedAccess);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Access",
                    "Check Access Permissons",
                    currentUserJson,
                    null,
                    correlationId.ToString()),
                        Times.Once);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.consumerAccessServiceMock.Verify(service =>
                service.RetrieveAllSubscriberAgreementsAsync(),
                    Times.Once);

            this.pdsDataServiceMock.Verify(service =>
                service.ValidateConsumerAccessToPatientAsync(
                    inputNhsNumber,
                    userId,
                    It.Is<List<string>>(ids => ids.SequenceEqual(expectedSubscriberAgreementIds)),
                    correlationId,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
            this.consumerAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static SubscriberAgreement CreateRandomSubscriberAgreement(string consumerId = "")
        {
            consumerId = string.IsNullOrEmpty(consumerId) ? Guid.NewGuid().ToString() : consumerId;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            var filler = new Filler<SubscriberAgreement>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(randomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(randomDateTimeOffset)
                .OnProperty(sa => sa.ConsumerId).Use(consumerId)
                .OnProperty(sa => sa.IsDeleted).Use(false)
                .OnProperty(sa => sa.Consumer).IgnoreIt();

            return filler.Create();
        }

        private static Access CreateRandomAccess()
        {
            var filler = new Filler<Access>();

            filler.Setup()
                .OnProperty(a => a.Reasons).IgnoreIt()
                .OnProperty(a => a.AllowedViaInformationSharingAgreements).IgnoreIt()
                .OnProperty(a => a.AllowedViaOrganisations).IgnoreIt();

            return filler.Create();
        }
    }
}
