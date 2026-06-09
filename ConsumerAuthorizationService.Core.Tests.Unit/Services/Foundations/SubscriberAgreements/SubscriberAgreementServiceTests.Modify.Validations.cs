// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions;
using Moq;

namespace ConsumerAuthorizationService.Core.Tests.Unit.Services.Foundations.SubscriberAgreements
{
    public partial class SubscriberAgreementServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfSubscriberAgreementIsNullAndLogItAsync()
        {
            // given
            SubscriberAgreement nullSubscriberAgreement = null;

            var nullSubscriberAgreementException =
                new NullSubscriberAgreementException(message: "Subscriber agreement is null.");

            var expectedSubscriberAgreementValidationException =
                new SubscriberAgreementValidationException(
                    message: "Subscriber agreement validation errors occurred, please try again.",
                    innerException: nullSubscriberAgreementException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(nullSubscriberAgreement))
                    .ReturnsAsync(nullSubscriberAgreement);

            // when
            ValueTask<SubscriberAgreement> modifySubscriberAgreementTask =
                this.subscriberAgreementService.ModifySubscriberAgreementAsync(nullSubscriberAgreement);

            SubscriberAgreementValidationException actualSubscriberAgreementServiceValidationException =
                await Assert.ThrowsAsync<SubscriberAgreementValidationException>(
                    modifySubscriberAgreementTask.AsTask);

            // then
            actualSubscriberAgreementServiceValidationException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(nullSubscriberAgreement),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfSubscriberAgreementIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            string randomUserId = GetRandomString();

            var invalidSubscriberAgreement = new SubscriberAgreement
            {
                SubscriberAgreementName = invalidText
            };

            var invalidSubscriberAgreementException =
                new InvalidSubscriberAgreementException(
                    message: "Invalid subscriber agreement. Please correct the errors and try again.");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.Id),
                values: "Id is required");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.ConsumerId),
                values: "Id is required");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.SubscriberAgreementId),
                values: "Text is required");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.SubscriberAgreementName),
                values: "Text is required");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.CreatedWhen),
                values: "Date is required");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.CreatedBy),
                values: "Text is required");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.UpdatedWhen),
                values:
                    [
                        "Date is required",
                        $"Date is the same as {nameof(SubscriberAgreement.CreatedWhen)}"
                    ]);

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.UpdatedBy),
                values:
                    [
                        "Text is required",
                        $"Expected value to be '{randomUserId}' but found '{invalidSubscriberAgreement.UpdatedBy}'."
                    ]);

            var expectedSubscriberAgreementValidationException =
                new SubscriberAgreementValidationException(
                    message: "Subscriber agreement validation errors occurred, please try again.",
                    innerException: invalidSubscriberAgreementException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidSubscriberAgreement))
                    .ReturnsAsync(invalidSubscriberAgreement);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<SubscriberAgreement> modifySubscriberAgreementTask =
                this.subscriberAgreementService.ModifySubscriberAgreementAsync(invalidSubscriberAgreement);

            SubscriberAgreementValidationException actualSubscriberAgreementServiceValidationException =
                await Assert.ThrowsAsync<SubscriberAgreementValidationException>(() =>
                    modifySubscriberAgreementTask.AsTask());

            // then
            actualSubscriberAgreementServiceValidationException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidSubscriberAgreement),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnModifyIfSubscriberAgreementHasInvalidLengthPropertyAndLogItAsync()
        {
            // given
            string randomUserId = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            SubscriberAgreement randomSubscriberAgreement =
                CreateRandomModifySubscriberAgreement(randomDateTimeOffset, userId: randomUserId);

            SubscriberAgreement invalidSubscriberAgreement = randomSubscriberAgreement;
            invalidSubscriberAgreement.SubscriberAgreementId = GetRandomStringWithLengthOf(256);
            invalidSubscriberAgreement.SubscriberAgreementName = GetRandomStringWithLengthOf(256);

            var invalidSubscriberAgreementException =
                new InvalidSubscriberAgreementException(
                    message: "Invalid subscriber agreement. Please correct the errors and try again.");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.SubscriberAgreementId),
                values:
                    $"Text exceed max length of {invalidSubscriberAgreement.SubscriberAgreementId.Length - 1} characters");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.SubscriberAgreementName),
                values:
                    $"Text exceed max length of {invalidSubscriberAgreement.SubscriberAgreementName.Length - 1} characters");

            var expectedSubscriberAgreementValidationException =
                new SubscriberAgreementValidationException(
                    message: "Subscriber agreement validation errors occurred, please try again.",
                    innerException: invalidSubscriberAgreementException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidSubscriberAgreement))
                    .ReturnsAsync(invalidSubscriberAgreement);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<SubscriberAgreement> modifySubscriberAgreementTask =
                this.subscriberAgreementService.ModifySubscriberAgreementAsync(invalidSubscriberAgreement);

            SubscriberAgreementValidationException actualSubscriberAgreementServiceValidationException =
                await Assert.ThrowsAsync<SubscriberAgreementValidationException>(
                    modifySubscriberAgreementTask.AsTask);

            // then
            actualSubscriberAgreementServiceValidationException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidSubscriberAgreement),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnModifyIfUpdatedWhenIsSameAsCreatedWhenAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();

            SubscriberAgreement randomSubscriberAgreement =
                CreateRandomSubscriberAgreement(randomDateTimeOffset, userId: randomUserId);

            SubscriberAgreement invalidSubscriberAgreement = randomSubscriberAgreement;

            var invalidSubscriberAgreementException =
                new InvalidSubscriberAgreementException(
                    message: "Invalid subscriber agreement. Please correct the errors and try again.");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.UpdatedWhen),
                values: $"Date is the same as {nameof(SubscriberAgreement.CreatedWhen)}");

            var expectedSubscriberAgreementValidationException =
                new SubscriberAgreementValidationException(
                    message: "Subscriber agreement validation errors occurred, please try again.",
                    innerException: invalidSubscriberAgreementException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidSubscriberAgreement))
                    .ReturnsAsync(invalidSubscriberAgreement);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<SubscriberAgreement> modifySubscriberAgreementTask =
                this.subscriberAgreementService.ModifySubscriberAgreementAsync(invalidSubscriberAgreement);

            SubscriberAgreementValidationException actualSubscriberAgreementServiceValidationException =
                await Assert.ThrowsAsync<SubscriberAgreementValidationException>(() =>
                    modifySubscriberAgreementTask.AsTask());

            // then
            actualSubscriberAgreementServiceValidationException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidSubscriberAgreement),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedWhenIsNotRecentAndLogItAsync(
            int minutesBeforeOrAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();

            SubscriberAgreement randomSubscriberAgreement =
                CreateRandomModifySubscriberAgreement(randomDateTimeOffset, userId: randomUserId);

            SubscriberAgreement invalidSubscriberAgreement = randomSubscriberAgreement;
            invalidSubscriberAgreement.UpdatedWhen = randomDateTimeOffset.AddMinutes(minutesBeforeOrAfter);
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            var invalidSubscriberAgreementException =
                new InvalidSubscriberAgreementException(
                    message: "Invalid subscriber agreement. Please correct the errors and try again.");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.UpdatedWhen),
                values: $"Date is not recent. Expected a value between {startDate} and {endDate} " +
                        $"but found {invalidSubscriberAgreement.UpdatedWhen}");

            var expectedSubscriberAgreementValidationException =
                new SubscriberAgreementValidationException(
                    message: "Subscriber agreement validation errors occurred, please try again.",
                    innerException: invalidSubscriberAgreementException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidSubscriberAgreement))
                    .ReturnsAsync(invalidSubscriberAgreement);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<SubscriberAgreement> modifySubscriberAgreementTask =
                this.subscriberAgreementService.ModifySubscriberAgreementAsync(invalidSubscriberAgreement);

            SubscriberAgreementValidationException actualSubscriberAgreementServiceValidationException =
                await Assert.ThrowsAsync<SubscriberAgreementValidationException>(() =>
                    modifySubscriberAgreementTask.AsTask());

            // then
            actualSubscriberAgreementServiceValidationException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidSubscriberAgreement),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnModifyIfSubscriberAgreementDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();

            SubscriberAgreement randomSubscriberAgreement =
                CreateRandomModifySubscriberAgreement(randomDateTimeOffset, userId: randomUserId);

            SubscriberAgreement nonExistentSubscriberAgreement = randomSubscriberAgreement;
            SubscriberAgreement noSubscriberAgreement = null;
            Guid subscriberAgreementId = nonExistentSubscriberAgreement.Id;

            var notFoundSubscriberAgreementException =
                new NotFoundSubscriberAgreementException(
                    message: $"Couldn't find subscriber agreement with subscriberAgreementId: {subscriberAgreementId}.");

            var expectedSubscriberAgreementValidationException =
                new SubscriberAgreementValidationException(
                    message: "Subscriber agreement validation errors occurred, please try again.",
                    innerException: notFoundSubscriberAgreementException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(nonExistentSubscriberAgreement))
                    .ReturnsAsync(nonExistentSubscriberAgreement);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriberAgreementByIdAsync(subscriberAgreementId))
                    .ReturnsAsync(noSubscriberAgreement);

            // when
            ValueTask<SubscriberAgreement> modifySubscriberAgreementTask =
                this.subscriberAgreementService.ModifySubscriberAgreementAsync(nonExistentSubscriberAgreement);

            SubscriberAgreementValidationException actualSubscriberAgreementServiceValidationException =
                await Assert.ThrowsAsync<SubscriberAgreementValidationException>(() =>
                    modifySubscriberAgreementTask.AsTask());

            // then
            actualSubscriberAgreementServiceValidationException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(nonExistentSubscriberAgreement),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriberAgreementByIdAsync(subscriberAgreementId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnModifyIfStorageCreatedWhenNotSameAsCreatedWhenAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();

            SubscriberAgreement randomSubscriberAgreement =
                CreateRandomModifySubscriberAgreement(randomDateTimeOffset, userId: randomUserId);

            SubscriberAgreement invalidSubscriberAgreement = randomSubscriberAgreement.DeepClone();
            SubscriberAgreement storageSubscriberAgreement = invalidSubscriberAgreement.DeepClone();
            storageSubscriberAgreement.CreatedWhen = storageSubscriberAgreement.CreatedWhen.AddMinutes(randomMinutes);
            storageSubscriberAgreement.UpdatedWhen = storageSubscriberAgreement.UpdatedWhen.AddMinutes(randomMinutes);

            var invalidSubscriberAgreementException =
                new InvalidSubscriberAgreementException(
                    message: "Invalid subscriber agreement. Please correct the errors and try again.");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.CreatedWhen),
                values: $"Date is not the same as {nameof(SubscriberAgreement.CreatedWhen)}");

            var expectedSubscriberAgreementValidationException =
                new SubscriberAgreementValidationException(
                    message: "Subscriber agreement validation errors occurred, please try again.",
                    innerException: invalidSubscriberAgreementException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidSubscriberAgreement))
                    .ReturnsAsync(invalidSubscriberAgreement);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriberAgreementByIdAsync(invalidSubscriberAgreement.Id))
                    .ReturnsAsync(storageSubscriberAgreement);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    It.IsAny<SubscriberAgreement>(), It.IsAny<SubscriberAgreement>()))
                        .ReturnsAsync(invalidSubscriberAgreement);

            // when
            ValueTask<SubscriberAgreement> modifySubscriberAgreementTask =
                this.subscriberAgreementService.ModifySubscriberAgreementAsync(invalidSubscriberAgreement);

            SubscriberAgreementValidationException actualSubscriberAgreementServiceValidationException =
                await Assert.ThrowsAsync<SubscriberAgreementValidationException>(() =>
                    modifySubscriberAgreementTask.AsTask());

            // then
            actualSubscriberAgreementServiceValidationException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidSubscriberAgreement),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriberAgreementByIdAsync(invalidSubscriberAgreement.Id),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    It.IsAny<SubscriberAgreement>(), It.IsAny<SubscriberAgreement>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnModifyIfStorageUpdatedWhenSameAsUpdatedWhenAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();

            SubscriberAgreement randomSubscriberAgreement =
                CreateRandomModifySubscriberAgreement(randomDateTimeOffset, userId: randomUserId);

            SubscriberAgreement invalidSubscriberAgreement = randomSubscriberAgreement;
            SubscriberAgreement storageSubscriberAgreement = invalidSubscriberAgreement.DeepClone();
            storageSubscriberAgreement.UpdatedWhen = invalidSubscriberAgreement.UpdatedWhen;

            var invalidSubscriberAgreementException =
                new InvalidSubscriberAgreementException(
                    message: "Invalid subscriber agreement. Please correct the errors and try again.");

            invalidSubscriberAgreementException.AddData(
                key: nameof(SubscriberAgreement.UpdatedWhen),
                values: $"Date is the same as {nameof(SubscriberAgreement.UpdatedWhen)}");

            var expectedSubscriberAgreementValidationException =
                new SubscriberAgreementValidationException(
                    message: "Subscriber agreement validation errors occurred, please try again.",
                    innerException: invalidSubscriberAgreementException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidSubscriberAgreement))
                    .ReturnsAsync(invalidSubscriberAgreement);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriberAgreementByIdAsync(invalidSubscriberAgreement.Id))
                    .ReturnsAsync(storageSubscriberAgreement);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    invalidSubscriberAgreement, storageSubscriberAgreement))
                        .ReturnsAsync(invalidSubscriberAgreement);

            // when
            ValueTask<SubscriberAgreement> modifySubscriberAgreementTask =
                this.subscriberAgreementService.ModifySubscriberAgreementAsync(invalidSubscriberAgreement);

            SubscriberAgreementValidationException actualSubscriberAgreementServiceValidationException =
                await Assert.ThrowsAsync<SubscriberAgreementValidationException>(() =>
                    modifySubscriberAgreementTask.AsTask());

            // then
            actualSubscriberAgreementServiceValidationException.Should()
                .BeEquivalentTo(expectedSubscriberAgreementValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidSubscriberAgreement),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriberAgreementByIdAsync(invalidSubscriberAgreement.Id),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    invalidSubscriberAgreement, storageSubscriberAgreement),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSubscriberAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
