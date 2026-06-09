// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ConsumerAuthorizationService.Core.Models.Bases;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Audits
{
    public class Audit : IKey, IAudit
    {
        public Guid Id { get; set; }
        public string CorrelationId { get; set; }
        public string AuditType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public string LogLevel { get; set; } = "Information";
        public string CreatedBy { get; set; } = string.Empty;
        public DateTimeOffset CreatedWhen { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTimeOffset UpdatedWhen { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset? DeletedWhen { get; set; }
        public string? DeletionReason { get; set; }
    }
}
