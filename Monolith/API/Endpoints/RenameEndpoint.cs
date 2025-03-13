using API.Events;
using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints;

public record RenameEndpointRequest(Guid Id, string Name);

public class RenameEndpoint(IDocumentSession session) : Endpoint<RenameEndpointRequest>
{
    private readonly IDocumentSession _session = session;

    public override void Configure()
    {
        Put("/houses/{id}/rename");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RenameEndpointRequest req, CancellationToken ct)
    {
        _session.Events.Append(req.Id, new HouseRenamed(req.Name));
        await _session.SaveChangesAsync(ct);

        var house = await _session.LoadAsync<House>(req.Id, ct);

        await SendOkAsync(house, ct);
    }
}