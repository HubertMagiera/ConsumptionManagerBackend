﻿using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForUpdates;
using System.Security.Claims;

namespace ConsumptionManagerBackend.Services.Interfaces
{
    public interface IUserService
    {
        TokenModel LoginUser(UserCredentialsDto userCredentials);
        int RegisterUser(UserCredentialsDto userCredentials);

        TokenModel AddUserData(AddUserDto addUser);

        void ChangeTariff(ChangeSupplierAndTariffDto tariff);

        void ChangePassword(ChangePasswordDto credentials);

        TokenModel RefreshSession(TokenModel model);

        int GetUserID();

        ClaimsPrincipal GetUser();
    }
}
