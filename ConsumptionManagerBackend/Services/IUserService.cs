using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForUpdates;

namespace ConsumptionManagerBackend.Services
{
    public interface IUserService
    {
        string LoginUser(UserCredentialsDto userCredentials);
        void RegisterUser(UserCredentialsDto userCredentials);

        void AddUserData(AddUserDto addUser);

        void ChangePassword(ChangePasswordDto credentials);
    }
}
