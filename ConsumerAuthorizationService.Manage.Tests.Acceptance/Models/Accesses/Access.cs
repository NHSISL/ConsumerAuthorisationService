// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.Accesses
{
    public class Access
    {
        public string NhsNumber { get; set; }
        public string ConsumerId { get; set; }
        public string ConsumerOrgCode { get; set; }
        public bool IsAccessAllowed { get; set; }
        public List<string> AllowedViaInformationSharingAgreements { get; set; }
        public List<string> AllowedViaOrganisations { get; set; }
        public List<AccessReason> Reasons { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
