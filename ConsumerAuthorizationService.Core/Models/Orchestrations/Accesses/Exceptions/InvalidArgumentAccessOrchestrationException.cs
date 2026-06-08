// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Orchestrations.Accesses.Exceptions
{
    public class InvalidArgumentAccessOrchestrationException : Xeption
    {
        public InvalidArgumentAccessOrchestrationException(string message)
            : base(message)
        { }
    }
}
