// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Consumers.Exceptions
{
    public class InvalidConsumerServiceException : Xeption
    {
        public InvalidConsumerServiceException(string message)
            : base(message)
        { }
    }
}
