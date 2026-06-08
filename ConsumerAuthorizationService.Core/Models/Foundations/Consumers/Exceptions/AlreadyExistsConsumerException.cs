// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Consumers.Exceptions
{
    public class AlreadyExistsConsumerException : Xeption
    {
        public AlreadyExistsConsumerException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
