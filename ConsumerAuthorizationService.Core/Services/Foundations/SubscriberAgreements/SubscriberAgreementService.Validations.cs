// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Services.Foundations.SubscriberAgreements
{
    internal partial class SubscriberAgreementService
    {
        private async ValueTask ValidateSubscriberAgreementOnAdd(SubscriberAgreement subscriberAgreement)
        {
            ValidateSubscriberAgreementIsNotNull(subscriberAgreement);
            string currentUserId = await this.securityAuditBroker.GetUserIdAsync();

            Validate(
                createException: () => new InvalidSubscriberAgreementException(
                    message: "Invalid subscriber agreement. Please correct the errors and try again."),

                (Rule: IsInvalid(subscriberAgreement.Id), Parameter: nameof(SubscriberAgreement.Id)),
                (Rule: IsInvalid(subscriberAgreement.ConsumerId), Parameter: nameof(SubscriberAgreement.ConsumerId)),

                (Rule: IsInvalid(subscriberAgreement.SubscriberAgreementId),
                    Parameter: nameof(SubscriberAgreement.SubscriberAgreementId)),

                (Rule: IsInvalid(subscriberAgreement.SubscriberAgreementName),
                    Parameter: nameof(SubscriberAgreement.SubscriberAgreementName)),

                (Rule: IsInvalid(subscriberAgreement.CreatedWhen),
                    Parameter: nameof(SubscriberAgreement.CreatedWhen)),

                (Rule: IsInvalid(subscriberAgreement.CreatedBy),
                    Parameter: nameof(SubscriberAgreement.CreatedBy)),

                (Rule: IsInvalid(subscriberAgreement.UpdatedWhen),
                    Parameter: nameof(SubscriberAgreement.UpdatedWhen)),

                (Rule: IsInvalid(subscriberAgreement.UpdatedBy),
                    Parameter: nameof(SubscriberAgreement.UpdatedBy)),


                (Rule: IsGreaterThan(subscriberAgreement.SubscriberAgreementId, 255),
                    Parameter: nameof(SubscriberAgreement.SubscriberAgreementId)),

                (Rule: IsGreaterThan(subscriberAgreement.SubscriberAgreementName, 255),
                    Parameter: nameof(SubscriberAgreement.SubscriberAgreementName)),

                (Rule: IsGreaterThan(subscriberAgreement.CreatedBy, 255),
                    Parameter: nameof(SubscriberAgreement.CreatedBy)),

                (Rule: IsGreaterThan(subscriberAgreement.UpdatedBy, 255),
                    Parameter: nameof(SubscriberAgreement.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: subscriberAgreement.UpdatedWhen,
                    secondDate: subscriberAgreement.CreatedWhen,
                    secondDateName: nameof(SubscriberAgreement.CreatedWhen)),
                Parameter: nameof(SubscriberAgreement.UpdatedWhen)),

                (Rule: IsNotSame(
                    first: currentUserId,
                    second: subscriberAgreement.CreatedBy),
                Parameter: nameof(SubscriberAgreement.CreatedBy)),

                (Rule: IsNotSame(
                    first: subscriberAgreement.UpdatedBy,
                    second: subscriberAgreement.CreatedBy,
                    secondName: nameof(SubscriberAgreement.CreatedBy)),
                Parameter: nameof(SubscriberAgreement.UpdatedBy)),

                (Rule: await IsNotRecentAsync(subscriberAgreement.CreatedWhen),
                    Parameter: nameof(SubscriberAgreement.CreatedWhen)));
        }

        private async ValueTask ValidateSubscriberAgreementOnModify(SubscriberAgreement subscriberAgreement)
        {
            ValidateSubscriberAgreementIsNotNull(subscriberAgreement);
            string currentUserId = await this.securityAuditBroker.GetUserIdAsync();

            Validate(
                createException: () => new InvalidSubscriberAgreementException(
                    message: "Invalid subscriber agreement. Please correct the errors and try again."),

                (Rule: IsInvalid(subscriberAgreement.Id), Parameter: nameof(SubscriberAgreement.Id)),
                (Rule: IsInvalid(subscriberAgreement.ConsumerId), Parameter: nameof(SubscriberAgreement.ConsumerId)),

                (Rule: IsInvalid(subscriberAgreement.SubscriberAgreementId),
                    Parameter: nameof(SubscriberAgreement.SubscriberAgreementId)),

                (Rule: IsInvalid(subscriberAgreement.SubscriberAgreementName),
                    Parameter: nameof(SubscriberAgreement.SubscriberAgreementName)),

                (Rule: IsInvalid(subscriberAgreement.CreatedWhen),
                    Parameter: nameof(SubscriberAgreement.CreatedWhen)),

                (Rule: IsInvalid(subscriberAgreement.CreatedBy),
                    Parameter: nameof(SubscriberAgreement.CreatedBy)),

                (Rule: IsInvalid(subscriberAgreement.UpdatedWhen),
                    Parameter: nameof(SubscriberAgreement.UpdatedWhen)),

                (Rule: IsInvalid(subscriberAgreement.UpdatedBy),
                    Parameter: nameof(SubscriberAgreement.UpdatedBy)),


                (Rule: IsGreaterThan(subscriberAgreement.SubscriberAgreementId, 255),
                    Parameter: nameof(SubscriberAgreement.SubscriberAgreementId)),

                (Rule: IsGreaterThan(subscriberAgreement.SubscriberAgreementName, 255),
                    Parameter: nameof(SubscriberAgreement.SubscriberAgreementName)),

                (Rule: IsGreaterThan(subscriberAgreement.CreatedBy, 255),
                    Parameter: nameof(SubscriberAgreement.CreatedBy)),

                (Rule: IsGreaterThan(subscriberAgreement.UpdatedBy, 255),
                    Parameter: nameof(SubscriberAgreement.UpdatedBy)),

                (Rule: IsNotSame(
                    first: currentUserId,
                    second: subscriberAgreement.UpdatedBy),
                Parameter: nameof(SubscriberAgreement.UpdatedBy)),

                (Rule: IsSame(
                    firstDate: subscriberAgreement.UpdatedWhen,
                    secondDate: subscriberAgreement.CreatedWhen,
                    secondDateName: nameof(SubscriberAgreement.CreatedWhen)),
                Parameter: nameof(SubscriberAgreement.UpdatedWhen)),

                (Rule: await IsNotRecentAsync(subscriberAgreement.UpdatedWhen),
                    Parameter: nameof(SubscriberAgreement.UpdatedWhen)));
        }

        private static void ValidateSubscriberAgreementId(Guid subscriberAgreementId)
        {
            Validate(
                createException: () => new InvalidSubscriberAgreementException(
                    message: "Invalid subscriber agreement. Please correct the errors and try again."),

                validations: (Rule: IsInvalid(subscriberAgreementId), Parameter: nameof(SubscriberAgreement.Id)));
        }

        private static void ValidateStorageSubscriberAgreement(
            SubscriberAgreement maybeSubscriberAgreement,
            Guid subscriberAgreementId)
        {
            if (maybeSubscriberAgreement is null)
            {
                throw new NotFoundSubscriberAgreementException(
                    message: $"Couldn't find subscriber agreement with subscriberAgreementId: {subscriberAgreementId}.");
            }
        }

        private static void ValidateSubscriberAgreementIsNotNull(SubscriberAgreement subscriberAgreement)
        {
            if (subscriberAgreement is null)
            {
                throw new NullSubscriberAgreementException(message: "Subscriber agreement is null.");
            }
        }

        private static void ValidateAgainstStorageSubscriberAgreementOnModify(
            SubscriberAgreement inputSubscriberAgreement,
            SubscriberAgreement storageSubscriberAgreement)
        {
            Validate(
                createException: () => new InvalidSubscriberAgreementException(
                    message: "Invalid subscriber agreement. Please correct the errors and try again."),

                (Rule: IsNotSame(
                        firstDate: inputSubscriberAgreement.CreatedWhen,
                        secondDate: storageSubscriberAgreement.CreatedWhen,
                        secondDateName: nameof(SubscriberAgreement.CreatedWhen)),
                    Parameter: nameof(SubscriberAgreement.CreatedWhen)),

                (Rule: IsNotSame(
                        first: inputSubscriberAgreement.CreatedBy,
                        second: storageSubscriberAgreement.CreatedBy,
                        secondName: nameof(SubscriberAgreement.CreatedBy)),
                    Parameter: nameof(SubscriberAgreement.CreatedBy)),

                (Rule: IsSame(
                        firstDate: inputSubscriberAgreement.UpdatedWhen,
                        secondDate: storageSubscriberAgreement.UpdatedWhen,
                        secondDateName: nameof(SubscriberAgreement.UpdatedWhen)),
                    Parameter: nameof(SubscriberAgreement.UpdatedWhen)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsGreaterThan(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static dynamic IsNotSame(string first, string second) => new
        {
            Condition = first != second,
            Message = $"Expected value to be '{first}' but found '{second}'."
        };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsNotSame(string first, string second, string secondName) => new
        {
            Condition = first != second,
            Message = $"Text is not the same as {secondName}"
        };

        private async ValueTask<dynamic> IsNotRecentAsync(DateTimeOffset date)
        {
            var (isNotRecent, startDate, endDate) = await IsDateNotRecentAsync(date);

            return new
            {
                Condition = isNotRecent,
                Message = $"Date is not recent. Expected a value between {startDate} and {endDate} but found {date}"
            };
        }

        private async ValueTask<(bool IsNotRecent, DateTimeOffset StartDate, DateTimeOffset EndDate)>
            IsDateNotRecentAsync(DateTimeOffset date)
        {
            int pastThreshold = 90;
            int futureThreshold = 0;
            DateTimeOffset currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            if (currentDateTime == default)
            {
                return (false, default, default);
            }

            DateTimeOffset startDate = currentDateTime.AddSeconds(-pastThreshold);
            DateTimeOffset endDate = currentDateTime.AddSeconds(futureThreshold);
            bool isNotRecent = date < startDate || date > endDate;

            return (isNotRecent, startDate, endDate);
        }

        private static void Validate<T>(
            Func<T> createException,
            params (dynamic Rule, string Parameter)[] validations)
            where T : Xeption
        {
            T invalidDataException = createException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDataException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDataException.ThrowIfContainsErrors();
        }
    }
}
