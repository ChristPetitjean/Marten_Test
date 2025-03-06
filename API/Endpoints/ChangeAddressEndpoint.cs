using API.Events;
using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints;

public record ChangeAddressRequest(Guid Id, string Address);

public class ChangeAddressEndpoint(IDocumentSession session) : Endpoint<ChangeAddressRequest>
{
    private readonly IDocumentSession _session = session;

    public override void Configure()
    {
        this.Put("/houses/{id}/changeAddress");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(ChangeAddressRequest req, CancellationToken ct)
    {
        this._session.Events.Append(req.Id, new AddressChanged(req.Address));
        await this._session.SaveChangesAsync(ct);

        var house = await this._session.LoadAsync<House>(req.Id, ct);

        await this.SendOkAsync(house, ct);
    }
}