// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses.Exceptions;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Services.Foundations.Accesses
{
    internal partial class AccessService
    {
        private delegate ValueTask<Access?> ReturningAccessFunction();

        private async ValueTask<Access?> TryCatch(ReturningAccessFunction returningAccessFunction)
        {
            try
            {
                return await returningAccessFunction();
            }
            catch (InvalidAccessException invalidAccessException)
            {
                throw await CreateAndLogValidationException(invalidAccessException);
            }
            catch (Exception exception)
            {
                var failedStorageAccessException =
                    new FailedStorageAccessException(
                        message: "Failed access storage error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogCriticalDependencyException(failedStorageAccessException);
            }
        }

        private async ValueTask<AccessValidationException> CreateAndLogValidationException(
            Xeption exception)
        {
            var accessValidationException =
                new AccessValidationException(
                    message: "Access validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(accessValidationException);

            return accessValidationException;
        }

        private async ValueTask<AccessDependencyException> CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var accessDependencyException =
                new AccessDependencyException(
                    message: "Access dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(accessDependencyException);

            return accessDependencyException;
        }
    }
}
