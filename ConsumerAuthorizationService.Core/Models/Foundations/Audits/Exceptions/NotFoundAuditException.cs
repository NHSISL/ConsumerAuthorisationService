// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Audits.Exceptions
{
    public class NotFoundAuditException : Xeption
    {
        public NotFoundAuditException(string message)
            : base(message)
        { }
    }
}