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
        this.Put("/houses/{id}/rename");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(RenameEndpointRequest req, CancellationToken ct)
    {
        this._session.Events.Append(req.Id, new HouseRenamed(req.Name));
        await this._session.SaveChangesAsync(ct);

        var house = await this._session.LoadAsync<House>(req.Id, ct);

        await this.SendOkAsync(house, ct);
    }
}