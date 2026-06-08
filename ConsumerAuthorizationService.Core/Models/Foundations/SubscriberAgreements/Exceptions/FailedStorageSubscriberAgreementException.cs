// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions
{
    public class FailedStorageSubscriberAgreementException : Xeption
    {
        public FailedStorageSubscriberAgreementException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
