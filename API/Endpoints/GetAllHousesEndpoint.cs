using System.Runtime.CompilerServices;
using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints;

public record GetAllHousesResponseHouse(Guid Id, string Name, string Address, int NumberOfRooms, decimal? Stars);

public class GetAllHousesEndpoint(IDocumentSession session) : EndpointWithoutRequest
{
    private readonly IDocumentSession _session = session;

    public override void Configure()
    {
        Get("/houses");
        AllowAnonymous();

        Description(b => b
        .Produces<IEnumerable<GetAllHousesResponseHouse>>());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendEventStreamAsync("houses-received", GetDataStream(ct), ct);
    }

    private async IAsyncEnumerable<object> GetDataStream([EnumeratorCancellation] CancellationToken ct)
    {
        var houses = _session.Query<House>().ToAsyncEnumerable(ct);

        await foreach (var house in houses)
        {
            yield return new GetAllHousesResponseHouse(house.Id, house.Name, house.Address, house.NumberOfRooms, house.Rate);
        }
    }
}