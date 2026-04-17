using Azure.Identity;
using Asp.Versioning;
using LoanService.Application.Contracts;
using LoanService.Application.Services;
using LoanService.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    var keyVaultUriValue = builder.Configuration["KeyVault:Uri"];
    if (string.IsNullOrWhiteSpace(keyVaultUriValue))
    {
        var keyVaultName = builder.Configuration["KeyVault:Name"];
        if (!string.IsNullOrWhiteSpace(keyVaultName))
        {
            keyVaultUriValue = $"https://{keyVaultName}.vault.azure.net/";
        }
    }

    if (Uri.TryCreate(keyVaultUriValue, UriKind.Absolute, out var keyVaultUri))
    {
        builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
    }
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddLoanService(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
    });
}

app.UseHttpsRedirection();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .ReportApiVersions()
    .Build();

var group = app.MapGroup("/api/v{version:apiVersion}/loans")
    .WithApiVersionSet(versionSet);

group.MapGet("/", async (LoanApplicationService service, CancellationToken cancellationToken) =>
{
    var result = await service.ListAsync(cancellationToken);
    return Results.Ok(result);
});

group.MapGet("/{id:guid}", async (Guid id, LoanApplicationService service, CancellationToken cancellationToken) =>
{
    var result = await service.GetAsync(id, cancellationToken);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

group.MapPost("/", async (CreateLoanApplicationRequest request, LoanApplicationService service, CancellationToken cancellationToken) =>
{
    var result = await service.CreateAsync(request, cancellationToken);
    return Results.Created($"/api/v1/loans/{result.Id}", result);
});

group.MapPut("/{id:guid}/review", async (Guid id, ReviewLoanApplicationRequest request, LoanApplicationService service, CancellationToken cancellationToken) =>
{
    var result = await service.ReviewAsync(id, request, cancellationToken);
    return Results.Ok(result);
});

app.Run();
