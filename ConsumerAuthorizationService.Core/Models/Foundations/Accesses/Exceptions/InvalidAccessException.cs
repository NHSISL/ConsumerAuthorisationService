// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Accesses.Exceptions
{
    public class InvalidAccessException : Xeption
    {
        public InvalidAccessException(string message)
            : base(message)
        { }
    }
}
