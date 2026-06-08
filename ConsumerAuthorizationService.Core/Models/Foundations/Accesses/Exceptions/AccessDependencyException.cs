// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Accesses.Exceptions
{
    public class AccessDependencyException : Xeption
    {
        public AccessDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
