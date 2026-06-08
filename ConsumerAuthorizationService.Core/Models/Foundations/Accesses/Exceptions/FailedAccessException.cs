// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Accesses.Exceptions
{
    public class FailedAccessException : Xeption
    {
        public FailedAccessException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
