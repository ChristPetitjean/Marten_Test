using API.Models;
using FastEndpoints;
using Marten;

namespace API.Endpoints.AddStars;

public class AddStarsEndpoint : Endpoint<AddStarsRequest>
{
    public override void Configure()
    {
        Post("/houses/{houseId}/stars");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddStarsRequest req, CancellationToken ct)
    {
        using var session = Resolve<IDocumentSession>();
        var house = await session.LoadAsync<House>(req.HouseId, ct);

        if (house == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        session.Store(house with { Stars = req.Stars});
        await session.SaveChangesAsync(ct);

        await SendOkAsync(house, ct);
    }
}