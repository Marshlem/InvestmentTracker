namespace InvestmentTracker.API.DTOs.Authentification;

public sealed class RefreshTokenRequest
{
    public string RefreshToken { get; init; } = default!;
}