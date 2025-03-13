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
        Post("/houses/{houseId}/rate");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RateRequest req, CancellationToken ct)
    {
        _session.Events.Append(req.Id, new UserRate(req.Stars));
        await _session.SaveChangesAsync(ct);

        var house = await _session.LoadAsync<House>(req.Id, ct);

        await SendOkAsync(house, ct);
    }
}