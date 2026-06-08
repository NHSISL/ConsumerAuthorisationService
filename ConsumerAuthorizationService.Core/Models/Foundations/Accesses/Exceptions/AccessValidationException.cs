// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Accesses.Exceptions
{
    public class AccessValidationException : Xeption
    {
        public AccessValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
