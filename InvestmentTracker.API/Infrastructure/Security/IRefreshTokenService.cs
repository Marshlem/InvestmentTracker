namespace InvestmentTracker.API.Infrastructure.Security;

public interface IRefreshTokenService
{
    string Generate();
    string Hash(string token);
}