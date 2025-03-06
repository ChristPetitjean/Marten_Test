using API.Events;
using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints.ChangeAddress;

public class ChangeAddressEndpoint(IDocumentSession session) : Endpoint<ChangeAddressRequest>
{
    private readonly IDocumentSession _session = session;

    public override void Configure()
    {
        Put("/houses/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ChangeAddressRequest req, CancellationToken ct)
    {
        _session.Events.Append(req.Id, new AddressChanged(req.Address));
        await session.SaveChangesAsync(ct);

        var house = await _session.LoadAsync<House>(req.Id, ct);

        await SendOkAsync(house, ct);
    }
}