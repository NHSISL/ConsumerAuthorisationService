// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.SubscriberAgreements
{
    public class SubscriberAgreement
    {
        public Guid Id { get; set; }
        public Guid ConsumerId { get; set; }
        public string SubscriberAgreementId { get; set; } = string.Empty;
        public string SubscriberAgreementName { get; set; } = string.Empty;
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
