namespace capg_hv_backend.Infrastructure.IdentityProvider;

public sealed class IdentityProviderOptions
{
    public string Endpoint { get; set; } = null!;

    public string ClientId { get; set; } = null!;
}
