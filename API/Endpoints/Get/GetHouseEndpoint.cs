using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints.Get;

public class GetHouseEndpoint : Endpoint<GetHouseRequest>
{
    public override void Configure()
    {
        Get("/houses/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetHouseRequest req, CancellationToken ct)
    {
        using var session = Resolve<IDocumentSession>();
        var house = await session.LoadAsync<House>(req.Id, ct);

        if (house == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(house, ct);
    }
}