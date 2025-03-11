using API.Events;
using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints;

public record CreateHouseRequest(string Name, string Address, int NumberOfRooms);

public class CreateHouseEndpoint(IDocumentSession session) : Endpoint<CreateHouseRequest>
{
    private readonly IDocumentSession _session = session;

    public override void Configure()
    {
        Post("/houses");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateHouseRequest req, CancellationToken ct)
    {
        var create = new NewHouseEnrolled(req.Name, req.Address, req.NumberOfRooms);
        var streamId = _session.Events.StartStream<House>(create).Id;
        await _session.SaveChangesAsync(ct);

        var house = await _session.Events.AggregateStreamAsync<House>(streamId, token: ct);
        await SendOkAsync(house, ct);
    }
}