// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Consumers.Exceptions
{
    public class FailedStorageConsumerException : Xeption
    {
        public FailedStorageConsumerException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
