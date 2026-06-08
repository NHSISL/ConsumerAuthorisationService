// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Orchestrations.Accesses.Exceptions
{
    public class ForbiddenAccessOrchestrationException : Xeption
    {
        public ForbiddenAccessOrchestrationException(string message)
            : base(message)
        { }
    }
}
