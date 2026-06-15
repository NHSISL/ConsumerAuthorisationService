// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using ConsumerAuthorizationService.Core.Models.Bases;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Consumers
{
    public class Consumer : IKey, IAudit
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactNotes { get; set; } = string.Empty;
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset? ActiveTo { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTimeOffset CreatedWhen { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTimeOffset UpdatedWhen { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset? DeletedWhen { get; set; }
        public string? DeletionReason { get; set; }

        [BindNever]
        public List<SubscriberAgreement> SubscriberAgreements { get; set; } = new List<SubscriberAgreement>();
    }
}
