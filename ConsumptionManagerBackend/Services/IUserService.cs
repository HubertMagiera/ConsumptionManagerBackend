using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForUpdates;

namespace ConsumptionManagerBackend.Services
{
    public interface IUserService
    {
        TokenModel LoginUser(UserCredentialsDto userCredentials);
        void RegisterUser(UserCredentialsDto userCredentials);

        void AddUserData(AddUserDto addUser);

        void ChangePassword(ChangePasswordDto credentials);

        TokenModel RefreshSession(TokenModel model);
    }
}
