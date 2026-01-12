namespace InvestmentTracker.API.DTOs.Authentification;
public sealed class LoginRequest
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}