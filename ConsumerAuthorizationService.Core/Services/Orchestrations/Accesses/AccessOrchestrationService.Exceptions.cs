// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses.Exceptions;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers.Exceptions;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements.Exceptions;
using ConsumerAuthorizationService.Core.Models.Orchestrations.Accesses.Exceptions;
using Xeptions;

namespace ConsumerAuthorizationService.Core.Services.Orchestrations.Accesses
{
    internal partial class AccessOrchestrationService
    {
        private delegate ValueTask<Access> ReturningAccessFunction();

        private async ValueTask<Access> TryCatch(ReturningAccessFunction returningAccessFunction)
        {
            try
            {
                return await returningAccessFunction();
            }
            catch (InvalidArgumentAccessOrchestrationException invalidArgumentAccessOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArgumentAccessOrchestrationException);
            }
            catch (UnauthorizedAccessOrchestrationException unauthorizedAccessOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(unauthorizedAccessOrchestrationException);
            }
            catch (ForbiddenAccessOrchestrationException forbiddenAccessOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(forbiddenAccessOrchestrationException);
            }
            catch (ConsumerValidationException consumerValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    consumerValidationException);
            }
            catch (ConsumerDependencyException consumerDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    consumerDependencyException);
            }
            catch (ConsumerDependencyValidationException consumerDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    consumerDependencyValidationException);
            }
            catch (ConsumerServiceException consumerServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    consumerServiceException);
            }
            catch (AccessValidationException accessValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    accessValidationException);
            }
            catch (AccessDependencyException accessDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    accessDependencyException);
            }
            catch (AccessDependencyValidationException accessDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    accessDependencyValidationException);
            }
            catch (AccessServiceException accessServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    accessServiceException);
            }
            catch (SubscriberAgreementValidationException subscriberAgreementValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    subscriberAgreementValidationException);
            }
            catch (SubscriberAgreementServiceDependencyValidationException subscriberAgreementServiceDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    subscriberAgreementServiceDependencyValidationException);
            }
            catch (SubscriberAgreementServiceDependencyException subscriberAgreementServiceDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    subscriberAgreementServiceDependencyException);
            }
            catch (SubscriberAgreementServiceException subscriberAgreementServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    subscriberAgreementServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceAccessOrchestrationException =
                    new FailedServiceAccessOrchestrationException(
                        message: "Failed access orchestration service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedServiceAccessOrchestrationException);
            }
        }

        private async ValueTask<AccessOrchestrationServiceException> CreateAndLogServiceExceptionAsync(
           Xeption exception)
        {
            var accessOrchestrationServiceException = new AccessOrchestrationServiceException(
                message: "Access orchestration service error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(accessOrchestrationServiceException);

            return accessOrchestrationServiceException;
        }

        private async ValueTask<AccessOrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var accessOrchestrationValidationException =
                new AccessOrchestrationValidationException(
                    message: "Access orchestration validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(accessOrchestrationValidationException);

            return accessOrchestrationValidationException;
        }

        private async ValueTask<AccessOrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var accessOrchestrationDependencyValidationException =
                new AccessOrchestrationDependencyValidationException(
                    message: "Access orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: (exception.InnerException as Xeption)!);

            await this.loggingBroker.LogErrorAsync(accessOrchestrationDependencyValidationException);

            return accessOrchestrationDependencyValidationException;
        }

        private async ValueTask<AccessOrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var accessOrchestrationDependencyException =
                new AccessOrchestrationDependencyException(
                    message: "Access orchestration dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: (exception.InnerException as Xeption)!);

            await this.loggingBroker.LogErrorAsync(accessOrchestrationDependencyException);

            return accessOrchestrationDependencyException;
        }
    }
}
