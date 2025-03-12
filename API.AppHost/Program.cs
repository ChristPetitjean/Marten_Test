var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", false);

var postgres = builder.AddPostgres("postgres", username)
                      .WithDataVolume(isReadOnly: false)
                      .WithPgAdmin()
                      .WithPgWeb();

// This database is here to allow pgWeb to create a bookmark for fast connexion
// WARN: this does not create database but only register in aspire that this one should exists !
var database = postgres.AddDatabase(username.Resource.Value);

builder.AddProject<Projects.API>("API")
       .WithReference(database)
       .WaitFor(database);

builder.Build()
       .Run();