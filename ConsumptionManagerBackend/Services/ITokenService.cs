using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels;
using System.Security.Claims;

namespace ConsumptionManagerBackend.Services
{
    public interface ITokenService
    {
        string CreateToken(UserCredentials user);
        string CreateRefreshToken();
        ClaimsPrincipal GetPrincipalFromOldToken(string oldToken);
    }
}
