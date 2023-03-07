using ConsumptionManagerBackend.DtoModels;

namespace ConsumptionManagerBackend.Services
{
    public interface IUserService
    {
        string LoginUser(UserCredentialsDto userCredentials);
        void RegisterUser(UserCredentialsDto userCredentials);

        void AddUserData(AddUserDto addUser);
    }
}
