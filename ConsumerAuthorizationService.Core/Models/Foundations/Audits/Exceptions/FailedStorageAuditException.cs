// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Audits.Exceptions
{
    public class FailedStorageAuditException : Xeption
    {
        public FailedStorageAuditException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}