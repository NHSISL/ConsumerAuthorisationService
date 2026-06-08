// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions
{
    public class AlreadyExistsSubscriberAgreementException : Xeption
    {
        public AlreadyExistsSubscriberAgreementException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
