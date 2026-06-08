// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Services.Foundations.SubscriberAgreements
{
    internal partial class SubscriberAgreementService
    {
        private delegate ValueTask<SubscriberAgreement> ReturningSubscriberAgreementFunction();
        private delegate ValueTask<IQueryable<SubscriberAgreement>> ReturningSubscriberAgreementsFunction();

        private async ValueTask<SubscriberAgreement> TryCatch(
            ReturningSubscriberAgreementFunction returningSubscriberAgreementFunction)
        {
            try
            {
                return await returningSubscriberAgreementFunction();
            }
            catch (NullSubscriberAgreementException nullSubscriberAgreementException)
            {
                throw await CreateAndLogValidationException(nullSubscriberAgreementException);
            }
            catch (InvalidSubscriberAgreementException invalidSubscriberAgreementException)
            {
                throw await CreateAndLogValidationException(invalidSubscriberAgreementException);
            }
            catch (NotFoundSubscriberAgreementException notFoundSubscriberAgreementException)
            {
                throw await CreateAndLogValidationException(notFoundSubscriberAgreementException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageSubscriberAgreementException =
                    new FailedStorageSubscriberAgreementException(
                        message: "Failed subscriber agreement storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedStorageSubscriberAgreementException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsSubscriberAgreementException =
                    new AlreadyExistsSubscriberAgreementException(
                        message: "Subscriber agreement with the same Id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationException(
                    alreadyExistsSubscriberAgreementException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidReferenceSubscriberAgreementException =
                    new InvalidReferenceSubscriberAgreementException(
                        message: "Invalid subscriber agreement reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw await CreateAndLogDependencyValidationException(invalidReferenceSubscriberAgreementException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedSubscriberAgreementException =
                    new LockedSubscriberAgreementException(
                        message: "Locked subscriber agreement record exception, please try again later",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationException(lockedSubscriberAgreementException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedStorageSubscriberAgreementException =
                    new FailedStorageSubscriberAgreementException(
                        message: "Failed subscriber agreement storage error occurred, contact support.",
                        innerException: databaseUpdateException);

                throw await CreateAndLogDependencyException(failedStorageSubscriberAgreementException);
            }
            catch (Exception exception)
            {
                var failedSubscriberAgreementException =
                    new FailedSubscriberAgreementException(
                        message: "Failed subscriber agreement service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedSubscriberAgreementException);
            }
        }

        private async ValueTask<IQueryable<SubscriberAgreement>> TryCatch(
            ReturningSubscriberAgreementsFunction returningSubscriberAgreementsFunction)
        {
            try
            {
                return await returningSubscriberAgreementsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageSubscriberAgreementException =
                    new FailedStorageSubscriberAgreementException(
                        message: "Failed subscriber agreement storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedStorageSubscriberAgreementException);
            }
            catch (Exception exception)
            {
                var failedSubscriberAgreementException =
                    new FailedSubscriberAgreementException(
                        message: "Failed subscriber agreement service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedSubscriberAgreementException);
            }
        }

        private async ValueTask<SubscriberAgreementValidationException> CreateAndLogValidationException(
            Xeption exception)
        {
            var subscriberAgreementValidationException =
                new SubscriberAgreementValidationException(
                    message: "Subscriber agreement validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(subscriberAgreementValidationException);

            return subscriberAgreementValidationException;
        }

        private async ValueTask<SubscriberAgreementServiceDependencyException> CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var subscriberAgreementServiceDependencyException =
                new SubscriberAgreementServiceDependencyException(
                    message: "Subscriber agreement dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(subscriberAgreementServiceDependencyException);

            return subscriberAgreementServiceDependencyException;
        }

        private async ValueTask<SubscriberAgreementServiceDependencyValidationException>
            CreateAndLogDependencyValidationException(Xeption exception)
        {
            var subscriberAgreementServiceDependencyValidationException =
                new SubscriberAgreementServiceDependencyValidationException(
                    message: "Subscriber agreement dependency validation occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(subscriberAgreementServiceDependencyValidationException);

            return subscriberAgreementServiceDependencyValidationException;
        }

        private async ValueTask<SubscriberAgreementServiceDependencyException> CreateAndLogDependencyException(
            Xeption exception)
        {
            var subscriberAgreementServiceDependencyException =
                new SubscriberAgreementServiceDependencyException(
                    message: "Subscriber agreement dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(subscriberAgreementServiceDependencyException);

            return subscriberAgreementServiceDependencyException;
        }

        private async ValueTask<SubscriberAgreementServiceException> CreateAndLogServiceException(
            Xeption exception)
        {
            var subscriberAgreementServiceException =
                new SubscriberAgreementServiceException(
                    message: "Subscriber agreement service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(subscriberAgreementServiceException);

            return subscriberAgreementServiceException;
        }
    }
}
