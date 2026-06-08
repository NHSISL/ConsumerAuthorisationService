// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Consumers.Exceptions
{
    public class ConsumerDependencyException : Xeption
    {
        public ConsumerDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}