﻿using Shared.Domain.Settings;
using Services.Patients.Presentation;
using Services.Patients.Persistence;
using Services.Patients.Application;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.Common.Helper;
using Common.Services;
using Shared.Consul.Configuration;

namespace Services.Patients.Api.Extensions;

internal static class Services
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        IConfiguration configuration = builder.Configuration;

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        services.AddResponseCompression(options =>
        {
            options.Providers.Add<GzipCompressionProvider>();
            options.EnableForHttps = true;
        });

        services.AddOptions<RelationalDatabaseSettings>()
            .BindConfiguration(nameof(RelationalDatabaseSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<CacheDatabaseSettings>()
            .BindConfiguration(nameof(CacheDatabaseSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<MessageQueueSettings>()
            .BindConfiguration(nameof(MessageQueueSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddPersistenceServices()
            .AddApplicationServices()
            .AddPresentationServices();

        services.AddSharedCommonProviders()
            .AddHashingServices();

        services.AddSwaggerGen()
            .AddConsulServices()
            .AddHealthCheck()
            .AddTelemetries(configuration);
    }

    
    private static IServiceCollection AddHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddNpgSql(
                sp => sp.GetRequiredService<IOptions<RelationalDatabaseSettings>>().Value.ConnectionString,
                name: "PostgreSQL",
                tags: new[] { "database", "relational" }
            );
            //.AddMongoDb(
            //    sp => sp.GetRequiredService<IOptions<NoRelationalDatabaseSettings>>().Value.ConnectionString,
            //    name: "MongoDB",
            //    tags: new[] { "database", "no relational" })
            //.AddRedis(
            //    sp => sp.GetRequiredService<IOptions<CacheDatabaseSettings>>().Value.ConnectionString,
            //    name: "Redis",
            //    tags: new[] { "database", "in memory" })
            //.AddRabbitMQ(
            //    services.BuildServiceProvider().GetRequiredService<IOptions<MessageQueueSettings>>().Value.Url,
            //    name: "RabbitMQ",
            //    tags: new[] { "queue", "messaging" }
            //);

        return services;
    }

    private static IServiceCollection AddTelemetries(this IServiceCollection services, IConfiguration configuration)
    {
        string? otlpEndpointEnv = configuration.GetValue<string>("OTEL_EXPORTER_OTLP_ENDPOINT");
        ArgumentNullException.ThrowIfNullOrEmpty(otlpEndpointEnv);

        Uri otlpEndpoint = new Uri(otlpEndpointEnv);

        Action<OtlpExporterOptions> otlpAction =
            options => options.Endpoint = otlpEndpoint;

        services.AddOpenTelemetry()
            .WithMetrics(opt =>
                opt.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ApplicationServices.Doctors.Api"))
                   .AddMeter("Service_Doctors_OpenRemoteManage")
                   .AddAspNetCoreInstrumentation()
                   .AddRuntimeInstrumentation()
                   .AddProcessInstrumentation()
                   .AddPrometheusExporter()
                   .AddOtlpExporter(otlpAction)
            )
            .WithTracing(opt =>
                opt.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ApplicationServices.Doctors.Api"))
                   .AddAspNetCoreInstrumentation()
                   .AddEntityFrameworkCoreInstrumentation()
                   .AddHttpClientInstrumentation()
                   .AddOtlpExporter(otlpAction)
            );

        services.AddMetrics();

        return services;
    }
}