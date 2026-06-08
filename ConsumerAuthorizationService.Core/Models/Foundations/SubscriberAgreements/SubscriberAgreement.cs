// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ConsumerAuthorizationService.Core.Models.Bases;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements
{
    public class SubscriberAgreement : IKey, IAudit
    {
        public Guid Id { get; set; }
        public string ConsumerId { get; set; }
        public string SubscriberAgreementId { get; set; }
        public string SubscriberAgreementName { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedWhen { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedWhen { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset? DeletedWhen { get; set; }
        public string? DeletionReason { get; set; }

        [BindNever]
        public Consumer Consumer { get; set; } = null!;
    }
}
