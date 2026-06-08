// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Accesses.Exceptions
{
    public class AccessDependencyValidationException : Xeption
    {
        public AccessDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
