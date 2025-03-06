using API.Projections;
using Marten;
using Weasel.Core;
using FastEndpoints;
using Marten.Events.Projections;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("Marten")!);

    // Specify that we want to use STJ as our serializer
    options.UseSystemTextJsonForSerialization();
    
    // Register all our projections
    options.Projections.Add<HouseProjection>(ProjectionLifecycle.Inline);
    
    options.Events.UseOptimizedProjectionRebuilds = true; 

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
}).UseLightweightSessions();

builder.Services.AddFastEndpoints()
.AddSwaggerDocument();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseFastEndpoints();

app.Run();
