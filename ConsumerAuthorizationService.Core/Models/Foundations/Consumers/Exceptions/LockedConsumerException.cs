// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Consumers.Exceptions
{
    public class LockedConsumerException : Xeption
    {
        public LockedConsumerException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}