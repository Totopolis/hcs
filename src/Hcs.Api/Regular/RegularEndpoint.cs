using FastEndpoints;

namespace Hcs.Api.Regular;

public sealed class RegularEndpoint : EndpointWithoutRequest
{
    public RegularEndpoint()
    {
    }

    public override void Configure()
    {
        Get("/regular");
        Policies(Constants.AllAuthenticsPolicy);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendAsync("You are regular user!");
    }
}
