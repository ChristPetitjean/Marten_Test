using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints.GetAll;

public class GetAllHousesEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/houses");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var session = Resolve<IDocumentSession>();
        var houses = await session.Query<House>().ToListAsync(ct);

        await SendOkAsync(houses, ct);
    }
}