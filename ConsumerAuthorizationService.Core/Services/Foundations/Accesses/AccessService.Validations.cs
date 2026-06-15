// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses.Exceptions;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Services.Foundations.Accesses
{
    internal partial class AccessService
    {
        private static void ValidateAccessOnValidate(
            string nhsNumber,
            string consumerUserId,
            List<string> subscriberAgreementIds,
            Guid correlationId)
        {
            Validate(
                createException: () => new InvalidAccessException(
                    message: "Invalid access. Please correct the errors and try again."),

                (Rule: IsInvalid(nhsNumber), Parameter: nameof(nhsNumber)),
                (Rule: IsInvalid(consumerUserId), Parameter: nameof(consumerUserId)),
                (Rule: IsInvalid(subscriberAgreementIds), Parameter: nameof(subscriberAgreementIds)),
                (Rule: IsInvalid(correlationId), Parameter: nameof(correlationId)));
        }

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid<T>(List<T> list) => new
        {
            Condition = list is null,
            Message = "List is required"
        };

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
