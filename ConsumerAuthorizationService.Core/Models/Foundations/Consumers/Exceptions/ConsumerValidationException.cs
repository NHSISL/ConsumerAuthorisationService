// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Consumers.Exceptions
{
    public class ConsumerValidationException : Xeption
    {
        public ConsumerValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}