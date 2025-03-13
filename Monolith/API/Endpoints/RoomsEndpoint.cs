using API.Events;
using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints;

public record RoomsRequest(Guid Id, int Added, int Removed);

public class RoomsEndpoint(IDocumentSession session) : Endpoint<RoomsRequest>
{
    private readonly IDocumentSession _session = session;

    public override void Configure()
    {
        Put("/houses/{id}/rooms");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RoomsRequest req, CancellationToken ct)
    {
        if (req.Added > 0)
        {
            _session.Events.Append(req.Id, new RoomsOpened(req.Added));
        }
        if (req.Removed > 0)
        {
            _session.Events.Append(req.Id, new RoomsClosed(req.Removed));
        }

        await _session.SaveChangesAsync(ct);

        var house = await _session.LoadAsync<House>(req.Id, ct);

        await SendOkAsync(house, ct);
    }
}