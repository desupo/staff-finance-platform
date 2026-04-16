using Asp.Versioning;
using FundClaimsService.Application.Contracts;
using FundClaimsService.Application.Services;
using FundClaimsService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddFundClaimsService(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .ReportApiVersions()
    .Build();

var group = app.MapGroup("/api/v{version:apiVersion}/fund-claims")
    .WithApiVersionSet(versionSet);

group.MapGet("/", async (FundClaimService service, CancellationToken cancellationToken) =>
{
    var result = await service.ListAsync(cancellationToken);
    return Results.Ok(result);
});

group.MapGet("/{id:guid}", async (Guid id, FundClaimService service, CancellationToken cancellationToken) =>
{
    var result = await service.GetAsync(id, cancellationToken);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

group.MapPost("/", async (CreateFundClaimRequest request, FundClaimService service, CancellationToken cancellationToken) =>
{
    var result = await service.CreateAsync(request, cancellationToken);
    return Results.Created($"/api/v1/fund-claims/{result.Id}", result);
});

group.MapPut("/{id:guid}/review", async (Guid id, ReviewFundClaimRequest request, FundClaimService service, CancellationToken cancellationToken) =>
{
    var result = await service.ReviewAsync(id, request, cancellationToken);
    return Results.Ok(result);
});

app.Run();
