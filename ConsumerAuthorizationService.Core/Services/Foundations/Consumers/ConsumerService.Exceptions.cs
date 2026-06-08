// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Services.Foundations.Consumers
{
    internal partial class ConsumerService
    {
        private delegate ValueTask<Consumer> ReturningConsumerFunction();
        private delegate ValueTask<IQueryable<Consumer>> ReturningConsumersFunction();

        private async ValueTask<Consumer> TryCatch(ReturningConsumerFunction returningConsumerFunction)
        {
            try
            {
                return await returningConsumerFunction();
            }
            catch (NullConsumerException nullConsumerException)
            {
                throw await CreateAndLogValidationException(nullConsumerException);
            }
            catch (InvalidConsumerServiceException invalidConsumerServiceException)
            {
                throw await CreateAndLogValidationException(invalidConsumerServiceException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageConsumerException =
                    new FailedStorageConsumerException(
                        message: "Failed consumer storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedStorageConsumerException);
            }
            catch (NotFoundConsumerException notFoundConsumerException)
            {
                throw await CreateAndLogValidationException(notFoundConsumerException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsConsumerException =
                    new AlreadyExistsConsumerException(
                        message: "Consumer with the same Id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationException(alreadyExistsConsumerException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidReferenceConsumerException =
                    new InvalidReferenceConsumerException(
                        message: "Invalid consumer reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw await CreateAndLogDependencyValidationException(invalidReferenceConsumerException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedConsumerException =
                    new LockedConsumerException(
                        message: "Locked consumer record exception, please try again later",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationException(lockedConsumerException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedStorageConsumerException =
                    new FailedStorageConsumerException(
                        message: "Failed consumer storage error occurred, contact support.",
                        innerException: databaseUpdateException);

                throw await CreateAndLogDependencyException(failedStorageConsumerException);
            }
            catch (Exception exception)
            {
                var failedConsumerException =
                    new FailedConsumerException(
                        message: "Failed consumer service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedConsumerException);
            }
        }

        private async ValueTask<IQueryable<Consumer>> TryCatch(
            ReturningConsumersFunction returningConsumersFunction)
        {
            try
            {
                return await returningConsumersFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageConsumerException =
                    new FailedStorageConsumerException(
                        message: "Failed consumer storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedStorageConsumerException);
            }
            catch (Exception exception)
            {
                var failedConsumerException =
                    new FailedConsumerException(
                        message: "Failed consumer service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedConsumerException);
            }
        }

        private async ValueTask<ConsumerValidationException> CreateAndLogValidationException(Xeption exception)
        {
            var consumerValidationException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerValidationException);

            return consumerValidationException;
        }

        private async ValueTask<ConsumerDependencyException> CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var consumerDependencyException =
                new ConsumerDependencyException(
                    message: "Consumer dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(consumerDependencyException);

            return consumerDependencyException;
        }

        private async ValueTask<ConsumerDependencyValidationException> CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var consumerDependencyValidationException =
                new ConsumerDependencyValidationException(
                    message: "Consumer dependency validation occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerDependencyValidationException);

            return consumerDependencyValidationException;
        }

        private async ValueTask<ConsumerDependencyException> CreateAndLogDependencyException(
            Xeption exception)
        {
            var consumerDependencyException =
                new ConsumerDependencyException(
                    message: "Consumer dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerDependencyException);

            return consumerDependencyException;
        }

        private async ValueTask<ConsumerServiceException> CreateAndLogServiceException(
            Xeption exception)
        {
            var consumerServiceException =
                new ConsumerServiceException(
                    message: "Consumer service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerServiceException);

            return consumerServiceException;
        }
    }
}
