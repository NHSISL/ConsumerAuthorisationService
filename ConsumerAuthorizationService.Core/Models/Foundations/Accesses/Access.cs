// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsumerAuthorizationService.Core.Models.Foundations.Accesses
{
    public class Access
    {
        [JsonPropertyName("nhsNumber")]
        public string NhsNumber { get; set; }

        [JsonPropertyName("consumerId")]
        public string ConsumerId { get; set; }

        [JsonPropertyName("consumerOrgCode")]
        public string ConsumerOrgCode { get; set; }

        [JsonPropertyName("isAccessAllowed")]
        public bool IsAccessAllowed { get; set; }

        [JsonPropertyName("allowedViaInformationSharingAgreements")]
        public List<string> AllowedViaInformationSharingAgreements { get; set; }

        [JsonPropertyName("allowedViaOrganisations")]
        public List<string> AllowedViaOrganisations { get; set; }

        [JsonPropertyName("reasons")]
        public List<AccessReason> Reasons { get; set; }

        [JsonPropertyName("correlationId")]
        public Guid CorrelationId { get; set; }
    }
}
