using API.Events;
using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints;

public record RateRequest(Guid Id, int Stars);

public class RateEndpoint(IDocumentSession session) : Endpoint<RateRequest>
{
    private readonly IDocumentSession _session = session;

    public override void Configure()
    {
        this.Post("/houses/{houseId}/rate");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(RateRequest req, CancellationToken ct)
    {
        this._session.Events.Append(req.Id, new UserRate(req.Stars));
        await this._session.SaveChangesAsync(ct);

        var house = await this._session.LoadAsync<House>(req.Id, ct);

        await this.SendOkAsync(house, ct);
    }
}