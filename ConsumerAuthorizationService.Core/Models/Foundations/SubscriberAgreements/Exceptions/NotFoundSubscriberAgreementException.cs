// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions
{
    public class NotFoundSubscriberAgreementException : Xeption
    {
        public NotFoundSubscriberAgreementException(string message)
            : base(message)
        { }
    }
}
