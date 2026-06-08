// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Accesses.Exceptions
{
    public class AccessServiceException : Xeption
    {
        public AccessServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
