using InvestmentTracker.API.Models;

namespace InvestmentTracker.API.Infrastructure.Security;

public interface ITokenService
{
    string GenerateAccessToken(User user);
}
