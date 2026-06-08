// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions
{
    public class SubscriberAgreementValidationException : Xeption
    {
        public SubscriberAgreementValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
