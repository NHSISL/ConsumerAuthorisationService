// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json;
using Attrify.Extensions;
using Attrify.InvisibleApi.Models;
using ConsumerAuthorizationService.Core.Brokers.Audits;
using ConsumerAuthorizationService.Core.Brokers.DateTimes;
using ConsumerAuthorizationService.Core.Brokers.Identifiers;
using ConsumerAuthorizationService.Core.Brokers.Loggings;
using ConsumerAuthorizationService.Core.Brokers.Securities;
using ConsumerAuthorizationService.Core.Brokers.Storages.Sql;
using ConsumerAuthorizationService.Core.Clients.Audits;
using ConsumerAuthorizationService.Core.Models.Foundations.Audits;
using ISL.Security.Client.Models.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

public partial class Program
{
    internal static Action<WebApplicationBuilder>? TestConfigurationOverrides { get; set; } = null;
    internal static bool ExcludeAppInsightsForTesting { get; set; } = false;
    internal static bool ExcludeMigrationsForTesting { get; set; } = false;

    internal static void ConfigurationOverridesForTesting(WebApplicationBuilder builder)
    {
        TestConfigurationOverrides?.Invoke(builder);
    }

    internal static void ConfigureApplicationInsightsTelemetry(WebApplicationBuilder builder)
    {
        if (ExcludeAppInsightsForTesting == false)
        {
            builder.Services.AddApplicationInsightsTelemetry();
        }
    }

    internal static void ConfigureServices(WebApplicationBuilder builder)
    {
        IConfiguration configuration = builder.Configuration;

        // ----------------- Authentication / Azure AD -----------------
        var azureAdSection = configuration.GetSection("AzureAd");

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(azureAdSection);

        var instance = configuration["AzureAd:Instance"];
        var tenantId = configuration["AzureAd:TenantId"];
        var scopes = configuration["AzureAd:Scopes"];

        var missingKeys = new List<string>();
        if (string.IsNullOrEmpty(instance)) missingKeys.Add("Instance");
        if (string.IsNullOrEmpty(tenantId)) missingKeys.Add("TenantId");
        if (string.IsNullOrEmpty(scopes)) missingKeys.Add("Scopes");

        if (missingKeys.Count > 0)
        {
            throw new InvalidOperationException(
                $"AzureAd configuration is incomplete. Missing keys: {string.Join(", ", missingKeys)}. " +
                "Please check appsettings.json.");
        }

        // ----------------- Core ASP.NET services -----------------
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthorization();
        builder.Services.AddDbContextFactory<StorageBroker>();
        builder.Services.AddDbContext<StorageBroker>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddEndpointsApiExplorer();

        // ----------------- Domain registrations -----------------
        AddProviders(builder.Services, configuration);
        AddBrokers(builder.Services, configuration);
        AddFoundationServices(builder.Services);
        AddOrchestrationServices(builder.Services, configuration);
        AddProcessingServices(builder.Services);
        AddCoordinationServices(builder.Services, configuration);
        AddClients(builder.Services);

        // IConfiguration registration (optional, but mirrors original)
        builder.Services.AddSingleton<IConfiguration>(configuration);

        JsonNamingPolicy jsonNamingPolicy = JsonNamingPolicy.CamelCase;

        builder.Services
            .AddControllers()
            .AddOData(options =>
            {
                options.AddRouteComponents("odata", GetEdmModel());
                options.Select().Filter().Expand().OrderBy().Count().SetMaxTop(100);
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = jsonNamingPolicy;
                options.JsonSerializerOptions.DictionaryKeyPolicy = jsonNamingPolicy;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.WriteIndented = true;
            });
    }

    internal static void ConfigurePipeline(WebApplication app)
    {
        // Resolve InvisibleApiKey from DI
        var invisibleApiKey = app.Services.GetRequiredService<InvisibleApiKey>();

        app.MapGet("/", () => Results.Ok(new
        {
            Name = "London FHIR Service API",
            Version = "1.0",
            Status = "Running"
        }));

        app.MapHealthChecks("/health");               // Basic liveness check
        app.MapHealthChecks("/health/ready");         // Readiness endpoint if needed
        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(configuration =>
            {
                configuration.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

                // Configure OAuth2 for Swagger UI
                configuration.OAuthClientId(app.Configuration["AzureAd:ClientId"]); // Use the application ClientId
                configuration.OAuthClientSecret("");
                configuration.OAuthUsePkce(); // Enable PKCE (Proof Key for Code Exchange)
                configuration.OAuthScopes(app.Configuration["AzureAd:Scopes"]); // Add required scopes
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseInvisibleApiMiddleware(invisibleApiKey);
        app.MapControllers();
        app.MapFallbackToFile("/index.html");
    }

    private static IEdmModel GetEdmModel()
    {
        ODataConventionModelBuilder builder = new();
        builder.EntitySet<Audit>("Audits");
        builder.EnableLowerCamelCase();
        return builder.GetEdmModel();
    }

    private static void AddProviders(IServiceCollection services, IConfiguration configuration)
    {
    }

    private static void AddBrokers(IServiceCollection services, IConfiguration configuration)
    {
        SecurityConfigurations securityConfigurations = new();
        services.AddSingleton(securityConfigurations);
        services.AddTransient<IAuditBroker, AuditBroker>();
        services.AddTransient<IDateTimeBroker, DateTimeBroker>();
        services.AddTransient<IIdentifierBroker, IdentifierBroker>();
        services.AddTransient<ILoggingBroker, LoggingBroker>();
        services.AddTransient<ISecurityAuditBroker, SecurityAuditBroker>();
        services.AddTransient<ISecurityBroker, SecurityBroker>();
        services.AddScoped<IStorageBroker>(
        serviceProvider => serviceProvider.GetRequiredService<StorageBroker>());
        services.AddScoped<IStorageBrokerFactory, StorageBrokerFactory>();
    }

    private static void AddFoundationServices(IServiceCollection services)
    {
        //services.AddTransient<IAuditService, AuditService>();
        //services.AddTransient<IConsumerAccessService, ConsumerAccessService>();
        //services.AddTransient<IConsumerService, ConsumerService>();
        //services.AddTransient<IStu3FhirReconciliationService, Stu3FhirReconciliationService>();
        //services.AddTransient<IOdsDataService, OdsDataService>();
        //services.AddTransient<IPdsDataService, PdsDataService>();
        //services.AddTransient<IProviderService, ProviderService>();
        //services.AddTransient<IFhirRecordService, FhirRecordService>();
        //services.AddTransient<IFhirRecordDifferenceService, FhirRecordDifferenceService>();
    }

    private static void AddProcessingServices(IServiceCollection services)
    {
    }

    private static void AddOrchestrationServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    private static void AddCoordinationServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    private static void AddClients(IServiceCollection services)
    {
        services.AddTransient<IAuditClient, AuditClient>();
    }
}
