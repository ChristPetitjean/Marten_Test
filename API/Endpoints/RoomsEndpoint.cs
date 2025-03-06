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
        this.Put("/houses/{id}/rooms");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(RoomsRequest req, CancellationToken ct)
    {
        if (req.Added > 0)
        {
            this._session.Events.Append(req.Id, new RoomsOpened(req.Added));
        }
        if (req.Removed > 0)
        {
            this._session.Events.Append(req.Id, new RoomsClosed(req.Removed));
        }
       
        await this._session.SaveChangesAsync(ct);

        var house = await this._session.LoadAsync<House>(req.Id, ct);

        await this.SendOkAsync(house, ct);
    }
}