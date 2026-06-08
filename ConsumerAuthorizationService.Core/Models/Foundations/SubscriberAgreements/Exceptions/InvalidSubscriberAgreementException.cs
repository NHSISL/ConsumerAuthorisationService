// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions
{
    public class InvalidSubscriberAgreementException : Xeption
    {
        public InvalidSubscriberAgreementException(string message)
            : base(message)
        { }
    }
}
