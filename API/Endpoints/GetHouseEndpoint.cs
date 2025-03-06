using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints;

public record GetHouseRequest(Guid Id);

public record GetHouseResponse(Guid Id, string Name, string Address, int NumberOfRooms, decimal? Stars);

public class GetHouseEndpoint(IDocumentSession session) : Endpoint<GetHouseRequest, GetHouseResponse>
{
    private readonly IDocumentSession _session = session;

    public override void Configure()
    {
        this.Get("/houses/{id}");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(GetHouseRequest req, CancellationToken ct)
    {
        var house = await this._session.LoadAsync<House>(req.Id, ct);

        if (house == null)
        {
            await this.SendNotFoundAsync(ct);
            return;
        }

        await this.SendOkAsync(new (house.Id, house.Name, house.Address, house.NumberOfRooms, house.Stars), ct);
    }
}