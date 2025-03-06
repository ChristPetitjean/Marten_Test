using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints;

public record GetAllHousesResponseHouse(Guid Id, string Name, string Address, int NumberOfRooms, decimal? Stars);

public record GetAllHousesResponse(IEnumerable<GetAllHousesResponseHouse> Houses);

public class GetAllHousesEndpoint(IDocumentSession session)  : EndpointWithoutRequest<GetAllHousesResponse>
{
    private readonly IDocumentSession _session = session;

    public override void Configure()
    {
        this.Get("/houses");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var houses = await this._session.Query<House>().ToListAsync(ct);

        await this.SendOkAsync(new(houses.Select(house => new GetAllHousesResponseHouse(house.Id, house.Name, house.Address, house.NumberOfRooms, house.Stars))), ct);
    }
}