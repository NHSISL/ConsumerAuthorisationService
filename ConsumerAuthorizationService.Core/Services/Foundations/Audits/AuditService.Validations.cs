// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Audits;
using ConsumerAuthorizationService.Core.Models.Foundations.Audits.Exceptions;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Services.Foundations.Audits
{
    internal partial class AuditService
    {
        private async ValueTask ValidateAuditOnAddAsync(Audit audit)
        {
            ValidateAuditIsNotNull(audit);
            string currentUserId = await this.securityAuditBroker.GetUserIdAsync();

            Validate(
                createException: () => new InvalidAuditException(
                    message: "Invalid audit. Please correct the errors and try again."),

                (Rule: IsInvalid(audit.Id), Parameter: nameof(Audit.Id)),
                (Rule: IsInvalid(audit.AuditType), Parameter: nameof(Audit.AuditType)),
                (Rule: IsInvalid(audit.Title), Parameter: nameof(Audit.Title)),
                (Rule: IsInvalid(audit.CreatedWhen), Parameter: nameof(Audit.CreatedWhen)),
                (Rule: IsInvalid(audit.CreatedBy), Parameter: nameof(Audit.CreatedBy)),
                (Rule: IsInvalid(audit.UpdatedWhen), Parameter: nameof(Audit.UpdatedWhen)),
                (Rule: IsInvalid(audit.UpdatedBy), Parameter: nameof(Audit.UpdatedBy)),

                (Rule: IsNotSame(
                    first: currentUserId,
                    second: audit.CreatedBy),
                Parameter: nameof(Audit.CreatedBy)),

                (Rule: IsNotSame(
                    firstDate: audit.UpdatedWhen,
                    secondDate: audit.CreatedWhen,
                    secondDateName: nameof(Audit.CreatedWhen)),
                Parameter: nameof(Audit.UpdatedWhen)),

                (Rule: IsNotSame(
                    first: audit.UpdatedBy,
                    second: audit.CreatedBy,
                    secondName: nameof(Audit.CreatedBy)),
                Parameter: nameof(Audit.UpdatedBy)),

                (Rule: await IsNotRecentAsync(audit.CreatedWhen), Parameter: nameof(Audit.CreatedWhen)));
        }

        private void ValidateOnBulkAddAudits(List<Audit> audits)
        {
            if (audits is null)
            {
                throw new NullAuditException(message: "Audits is null.");
            }
        }

        private async ValueTask ValidateAuditOnModifyAsync(Audit audit)
        {
            ValidateAuditIsNotNull(audit);
            string currentUserId = await this.securityAuditBroker.GetUserIdAsync();

            Validate(
                createException: () => new InvalidAuditException(
                    message: "Invalid audit. Please correct the errors and try again."),

                (Rule: IsInvalid(audit.Id), Parameter: nameof(Audit.Id)),
                (Rule: IsInvalid(audit.AuditType), Parameter: nameof(Audit.AuditType)),
                (Rule: IsInvalid(audit.Title), Parameter: nameof(Audit.Title)),
                (Rule: IsInvalid(audit.CreatedWhen), Parameter: nameof(Audit.CreatedWhen)),
                (Rule: IsInvalid(audit.CreatedBy), Parameter: nameof(Audit.CreatedBy)),
                (Rule: IsInvalid(audit.UpdatedWhen), Parameter: nameof(Audit.UpdatedWhen)),
                (Rule: IsInvalid(audit.UpdatedBy), Parameter: nameof(Audit.UpdatedBy)),

                (Rule: IsNotSame(
                    first: currentUserId,
                    second: audit.UpdatedBy),
                Parameter: nameof(Audit.UpdatedBy)),

                (Rule: IsSame(
                    firstDate: audit.UpdatedWhen,
                    secondDate: audit.CreatedWhen,
                    secondDateName: nameof(Audit.CreatedWhen)),
                Parameter: nameof(Audit.UpdatedWhen)),

                (Rule: await IsNotRecentAsync(audit.UpdatedWhen), Parameter: nameof(audit.UpdatedWhen)));
        }

        private async ValueTask ValidateAgainstStorageAuditOnDeleteAsync(
            Audit audit,
            Audit maybeAudit)
        {
            ValidateAuditIsNotNull(audit);
            string currentUserId = await this.securityAuditBroker.GetUserIdAsync();

            Validate(
                createException: () => new InvalidAuditException(
                    message: "Invalid audit. Please correct the errors and try again."),

                (Rule: IsNotSame(
                    audit.CreatedWhen,
                    maybeAudit.CreatedWhen,
                    nameof(maybeAudit.CreatedWhen)),
                 Parameter: nameof(Audit.CreatedWhen)),

                (Rule: IsNotSame(
                    audit.CreatedBy,
                    maybeAudit.CreatedBy,
                    nameof(maybeAudit.CreatedBy)),
                 Parameter: nameof(Audit.CreatedBy)),

                (Rule: IsNotSame(
                    maybeAudit.UpdatedWhen,
                    audit.UpdatedWhen,
                    nameof(Audit.UpdatedWhen)),
                 Parameter: nameof(Audit.UpdatedWhen)),

                (Rule: IsNotSame(
                    currentUserId,
                    audit.UpdatedBy,
                    nameof(Audit.UpdatedBy)),
                 Parameter: nameof(Audit.UpdatedBy))
            );
        }

        public void ValidateAuditId(Guid auditId) =>
            Validate(
                createException: () => new InvalidAuditException(
                    message: "Invalid audit. Please correct the errors and try again."),

                (Rule: IsInvalid(auditId), Parameter: nameof(Audit.Id)));

        private static void ValidateStorageAudit(Audit maybeAudit, Guid auditId)
        {
            if (maybeAudit is null)
            {
                throw new NotFoundAuditException($"Couldn't find audit with auditId: {auditId}.");
            }
        }

        private static void ValidateAuditIsNotNull(Audit audit)
        {
            if (audit is null)
            {
                throw new NullAuditException(message: "Audit is null.");
            }
        }

        private static void ValidateAgainstStorageAuditOnModify(Audit inputAudit, Audit storageAudit)
        {
            Validate(
                createException: () => new InvalidAuditException(
                    message: "Invalid audit. Please correct the errors and try again."),

                (Rule: IsNotSame(
                    firstDate: inputAudit.CreatedWhen,
                    secondDate: storageAudit.CreatedWhen,
                    secondDateName: nameof(Audit.CreatedWhen)),
                Parameter: nameof(Audit.CreatedWhen)),

                (Rule: IsNotSame(
                    first: inputAudit.CreatedBy,
                    second: storageAudit.CreatedBy,
                    secondName: nameof(Audit.CreatedBy)),
                Parameter: nameof(Audit.CreatedBy)),

                (Rule: IsSame(
                    firstDate: inputAudit.UpdatedWhen,
                    secondDate: storageAudit.UpdatedWhen,
                    secondDateName: nameof(Audit.UpdatedWhen)),
                Parameter: nameof(Audit.UpdatedWhen)));
        }

        private static dynamic IsInvalid(List<Audit> audits) => new
        {
            Condition = audits == null,
            Message = "Audits is required"
        };

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

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            string first,
            string second) => new
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

        private static dynamic IsNotSame(
            Guid firstId,
            Guid secondId,
            string secondIdName) => new
            {
                Condition = firstId != secondId,
                Message = $"Id is not the same as {secondIdName}"
            };

        private static dynamic IsNotSame(
           string first,
           string second,
           string secondName) => new
           {
               Condition = first != second,
               Message = $"Text is not the same as {secondName}"
           };

        private async ValueTask<dynamic> IsNotRecentAsync(DateTimeOffset date) => new
        {
            Condition = await IsDateNotRecentAsync(date),
            Message = "Date is not recent"
        };

        private async ValueTask<bool> IsDateNotRecentAsync(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
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