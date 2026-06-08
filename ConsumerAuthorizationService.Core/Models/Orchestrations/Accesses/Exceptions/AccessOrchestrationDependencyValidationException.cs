// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Orchestrations.Accesses.Exceptions
{
    public class AccessOrchestrationDependencyValidationException : Xeption
    {
        public AccessOrchestrationDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
