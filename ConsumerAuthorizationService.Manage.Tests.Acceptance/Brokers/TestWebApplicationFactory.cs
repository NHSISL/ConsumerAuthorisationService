// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Attrify.InvisibleApi.Models;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;
using ConsumerAuthorizationService.Core.Models.Foundations.Consumers;
using ConsumerAuthorizationService.Core.Models.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Services.Foundations.Consumers;
using ConsumerAuthorizationService.Core.Services.Foundations.SubscriberAgreements;
using ConsumerAuthorizationService.Core.Services.Orchestrations.Accesses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Brokers
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        static TestWebApplicationFactory()
        {
            // Configure configuration *before* the app’s builder is used
            Program.TestConfigurationOverrides = builder =>
            {
                var testProjectPath =
                    Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

                // This runs inside Program.cs right after CreateBuilder(...)
                // This lets us override any configuration values for testing
                builder.Configuration
                    .AddJsonFile(
                        Path.Combine(testProjectPath, "appsettings.json"),
                        optional: true)
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        // Put your strong overrides here
                        //["AzureAd:TenantId"] = "TEST-TENANT",
                        //["AzureAd:Instance"] = "https://login.microsoftonline.com/",
                        //["AzureAd:Scopes"]   = "api://test/.default"
                    });
            };

            Program.ExcludeAppInsightsForTesting = true;
            Program.ExcludeMigrationsForTesting = true;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Make sure the app runs in a predictable test environment
            builder.UseEnvironment("Test");

            builder.ConfigureServices((context, services) =>
            {
                OverrideSecurityForTesting(services);
                MockServicesForTesting(services);
            });
        }

        private static void MockServicesForTesting(IServiceCollection services)
        {
            var accessOrchestrationServiceMock = new Mock<IAccessOrchestrationService>();

            accessOrchestrationServiceMock
                .Setup(service => service.ValidateAccess(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((string consumerUserId, string nhsNumber, Guid correlationId, CancellationToken _) =>
                    new Access
                    {
                        NhsNumber = nhsNumber,
                        ConsumerId = consumerUserId,
                        IsAccessAllowed = true,
                        CorrelationId = correlationId
                    });

            services.AddTransient<IAccessOrchestrationService>(_ => accessOrchestrationServiceMock.Object);

            var consumerServiceMock = new Mock<IConsumerService>();

            consumerServiceMock
                .Setup(service => service.AddConsumerAsync(It.IsAny<Consumer>()))
                .ReturnsAsync((Consumer consumer) => consumer);

            consumerServiceMock
                .Setup(service => service.RetrieveAllConsumersAsync())
                .ReturnsAsync(Enumerable.Empty<Consumer>().AsQueryable());

            consumerServiceMock
                .Setup(service => service.RetrieveConsumerByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Consumer { Id = id });

            consumerServiceMock
                .Setup(service => service.ModifyConsumerAsync(It.IsAny<Consumer>()))
                .ReturnsAsync((Consumer consumer) => consumer);

            consumerServiceMock
                .Setup(service => service.RemoveConsumerByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Consumer { Id = id });

            services.AddTransient<IConsumerService>(_ => consumerServiceMock.Object);

            var subscriberAgreementServiceMock = new Mock<ISubscriberAgreementService>();

            subscriberAgreementServiceMock
                .Setup(service => service.AddSubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()))
                .ReturnsAsync((SubscriberAgreement subscriberAgreement) => subscriberAgreement);

            subscriberAgreementServiceMock
                .Setup(service => service.RetrieveAllSubscriberAgreementsAsync())
                .ReturnsAsync(Enumerable.Empty<SubscriberAgreement>().AsQueryable());

            subscriberAgreementServiceMock
                .Setup(service => service.RetrieveSubscriberAgreementByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new SubscriberAgreement { Id = id });

            subscriberAgreementServiceMock
                .Setup(service => service.ModifySubscriberAgreementAsync(It.IsAny<SubscriberAgreement>()))
                .ReturnsAsync((SubscriberAgreement subscriberAgreement) => subscriberAgreement);

            subscriberAgreementServiceMock
                .Setup(service => service.RemoveSubscriberAgreementByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new SubscriberAgreement { Id = id });

            services.AddTransient<ISubscriberAgreementService>(_ => subscriberAgreementServiceMock.Object);
        }

        private static void OverrideSecurityForTesting(IServiceCollection services)
        {
            var invisibleApiKeyDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(InvisibleApiKey));

            InvisibleApiKey invisibleApiKey = null;

            if (invisibleApiKeyDescriptor != null)
            {
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    invisibleApiKey = serviceProvider.GetService<InvisibleApiKey>();
                }
            }

            // Remove existing authentication and authorization
            var authenticationDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(IAuthenticationSchemeProvider));

            if (authenticationDescriptor != null)
            {
                services.Remove(authenticationDescriptor);
            }

            // Override authentication and authorization
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultChallengeScheme = "TestScheme";
            })
            .AddScheme<CustomAuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options =>
            {
                options.InvisibleApiKey = invisibleApiKey;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("TestPolicy", policy => policy.RequireAssertion(_ => true));
            });
        }
    }
}
