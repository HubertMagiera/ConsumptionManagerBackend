using ConsumptionManagerBackend.DtoModels;
using System.Security.Claims;

namespace ConsumptionManagerBackend.Services
{
    public interface ITokenService
    {
        string CreateToken(UserCredentialsDto user);
        string CreateRefreshToken();
        ClaimsPrincipal GetPrincipalFromOldToken(string oldToken);
    }
}
