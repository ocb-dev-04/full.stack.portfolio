﻿using Service.Diagnoses.Persistence.Context;
using Services.Diagnoses.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Service.Diagnoses.Persistence.Serializers;
using Service.Diagnoses.Persistence.Repositories;

namespace Service.Diagnoses.Persistence;

public static class PersistenceServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        DiagnosisSerializer.RegisterMappings();
        services.AddScoped<NoRelationalContext>();

        services.AddScoped<IDiagnosisRepository, DiagnosisRepository>();

        return services;
    }
}