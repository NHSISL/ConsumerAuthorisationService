// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsumerAuthorizationService.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        private static void AddSubscriberAgreementConfigurations(EntityTypeBuilder<SubscriberAgreement> model)
        {
            model
                .ToTable("SubscriberAgreements");

            model
                .HasIndex(subscriberAgreement =>
                    new { subscriberAgreement.ConsumerId, subscriberAgreement.SubscriberAgreementId })
                .IsUnique();

            model
                .Property(subscriberAgreement => subscriberAgreement.Id)
                .IsRequired();

            model
                .Property(subscriberAgreement => subscriberAgreement.ConsumerId)
                .IsRequired();

            model
                .Property(subscriberAgreement => subscriberAgreement.SubscriberAgreementId)
                .IsRequired();

            model
                .Property(subscriberAgreement => subscriberAgreement.CreatedBy)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(subscriberAgreement => subscriberAgreement.CreatedWhen)
                .IsRequired();

            model
                .Property(subscriberAgreement => subscriberAgreement.UpdatedBy)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(subscriberAgreement => subscriberAgreement.UpdatedWhen)
                .IsRequired();

            model
               .HasOne(subscriberAgreement => subscriberAgreement.Consumer)
               .WithMany(consumer => consumer.SubscriberAgreements)
               .HasForeignKey(subscriberAgreement => subscriberAgreement.ConsumerId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
