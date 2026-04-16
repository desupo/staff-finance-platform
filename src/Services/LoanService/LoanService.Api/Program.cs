using Asp.Versioning;
using LoanService.Application.Contracts;
using LoanService.Application.Services;
using LoanService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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
    app.UseSwaggerUI();
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
