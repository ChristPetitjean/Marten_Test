using API.Projections;
using Marten;
using Weasel.Core;
using FastEndpoints;
using FastEndpoints.Swagger;
using Marten.Events.Projections;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Aspire services
builder.AddServiceDefaults();

//connectionName must match ASPIRE Host database name (usually same as username)
builder.AddNpgsqlDataSource(connectionName: "houses-db"); 

builder.Services.AddOpenApi();

builder.Services.AddMarten(options =>
                           {
                               // Disable the absurdly verbose Npgsql logging
                               options.DisableNpgsqlLogging = true;
                               
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
                           })
       .UseNpgsqlDataSource() // For mapping to previously created postgres service
       .UseLightweightSessions();

builder.Services
       .AddFastEndpoints()
       .SwaggerDocument();

var app = builder.Build();

//Aspire endpoints
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => { options.Servers = []; });
}

app.UseHttpsRedirection();

app.UseFastEndpoints()
   .UseSwaggerGen();

app.Run();