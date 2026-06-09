// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Clients.Audits;
using ConsumerAuthorizationService.Core.Models.Foundations.Audits;

namespace ConsumerAuthorizationService.Core.Brokers.Audits
{
    public class AuditBroker : IAuditBroker
    {
        private readonly IAuditClient auditClient;

        public AuditBroker(IAuditClient auditClient) =>
            this.auditClient = auditClient;

        public async ValueTask BulkLogAsync(List<Audit> audits) =>
            await auditClient.BulkLogAuditsAsync(audits);

        public async ValueTask<Audit> LogAsync(
            string auditType,
            string title,
            string message,
            string correlationId,
            string? fileName,
            string logLevel = "Information")
        {
            return await auditClient.LogAuditAsync(auditType, title, message, correlationId, fileName, logLevel);
        }

        public async ValueTask<Audit> LogInformationAsync(
            string auditType,
            string title,
            string message,
            string correlationId,
            string? fileName)
        {
            return await auditClient.LogAuditAsync(auditType, title, message, correlationId, fileName, "Information");
        }

        public async ValueTask<Audit> LogWarningAsync(
            string auditType,
            string title,
            string message,
            string correlationId,
            string? fileName)
        {
            return await auditClient.LogAuditAsync(auditType, title, message, correlationId, fileName, "Warning");
        }

        public async ValueTask<Audit> LogErrorAsync(
            string auditType,
            string title,
            string message,
            string correlationId,
            string? fileName)
        {
            return await auditClient.LogAuditAsync(auditType, title, message, correlationId, fileName, "Error");
        }

        public async ValueTask<Audit> LogCriticalAsync(
            string auditType,
            string title,
            string message,
            string correlationId,
            string? fileName)
        {
            return await auditClient.LogAuditAsync(auditType, title, message, correlationId, fileName, "Critical");
        }
    }
}
