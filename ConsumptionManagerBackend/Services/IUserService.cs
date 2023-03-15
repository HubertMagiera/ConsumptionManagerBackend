using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForUpdates;
using System.Security.Claims;

namespace ConsumptionManagerBackend.Services
{
    public interface IUserService
    {
        TokenModel LoginUser(UserCredentialsDto userCredentials);
        int RegisterUser(UserCredentialsDto userCredentials);

        TokenModel AddUserData(AddUserDto addUser);

        void ChangePassword(ChangePasswordDto credentials);

        TokenModel RefreshSession(TokenModel model);

        int GetUserID();

        ClaimsPrincipal GetUser();
    }
}
