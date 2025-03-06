using API.Events;
using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints.Create;

public class CreateHouseEndpoint : Endpoint<CreateHouseRequest>
{
    private readonly IDocumentSession _session;
    public CreateHouseEndpoint(IDocumentSession session)
    {
        _session = session;
    }

    public override void Configure()
    {
        Post("/houses");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateHouseRequest req, CancellationToken ct)
    {
        var create = new NewHouseEnroled(req.Name, req.Address, req.NumberOfRooms);
        var streamId = _session.Events.StartStream<House>(create).Id;
        await _session.SaveChangesAsync(ct);

        var house = await _session.Events.AggregateStreamAsync<House>(streamId, token: ct);
        await SendOkAsync(house, ct);
    }
}