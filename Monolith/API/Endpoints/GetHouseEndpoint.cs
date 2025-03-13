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
        Get("/houses/{@id:guid}", x => new GetHouseRequest(x.Id));
        AllowAnonymous();

        // This is a way to clear the default accepts, which is application/json
        // It prevent the endpoint from accetpting a body with a DTO to overrides routes parameters
        Description(x => x.ClearDefaultAccepts());
    }

    public override async Task HandleAsync(GetHouseRequest req, CancellationToken ct)
    {
        var house = await _session.LoadAsync<House>(req.Id, ct);

        if (house == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(new(house.Id, house.Name, house.Address, house.NumberOfRooms, house.Rate), ct);
    }
}