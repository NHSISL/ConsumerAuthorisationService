// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;

namespace ConsumerAuthorizationService.Core.Brokers.Storages.Sql
{
    public partial interface IStorageBroker
    {
        ValueTask<SubscriberAgreement> InsertSubscriberAgreementAsync(SubscriberAgreement subscriberAgreement);
        ValueTask<IQueryable<SubscriberAgreement>> SelectAllSubscriberAgreementsAsync();
        ValueTask<SubscriberAgreement> SelectSubscriberAgreementByIdAsync(Guid subscriberAgreementId);
        ValueTask<SubscriberAgreement> UpdateSubscriberAgreementAsync(SubscriberAgreement subscriberAgreement);
        ValueTask<SubscriberAgreement> DeleteSubscriberAgreementAsync(SubscriberAgreement subscriberAgreement);
    }
}
