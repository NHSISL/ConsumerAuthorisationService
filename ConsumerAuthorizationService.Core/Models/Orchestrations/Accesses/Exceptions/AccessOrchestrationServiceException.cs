// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Orchestrations.Accesses.Exceptions
{
    public class AccessOrchestrationServiceException : Xeption
    {
        public AccessOrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
