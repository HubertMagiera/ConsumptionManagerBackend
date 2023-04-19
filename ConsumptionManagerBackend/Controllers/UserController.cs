﻿using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForUpdates;
using ConsumptionManagerBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsumptionManagerBackend.Controllers
{
    [Route("consumptionManager/user")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMeasurementService _measurementService;
        public UserController(IUserService userService, IMeasurementService measurementService)
        {
            _userService = userService;
            _measurementService = measurementService;
        }
        [HttpPost]
        [Route("register")]
        public ActionResult RegisterNewUser([FromBody] UserCredentialsDto userCredentials)
        {
            int id =_userService.RegisterUser(userCredentials);
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status201Created,
                Value = id
            };
        }

        [HttpPost]
        [Route("changePassword")]
        [Authorize]
        public ActionResult ChangePassword([FromBody] ChangePasswordDto changePassword)
        {
            _userService.ChangePassword(changePassword);
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
        [HttpPost]
        [Route("login")]
        public ActionResult<TokenModel> LoginUser([FromBody] UserCredentialsDto loginUser)
        {
            var tokens = _userService.LoginUser(loginUser);
            _measurementService.AddMeasurementsBasedOnSchedule();
            return Ok(tokens);

        }

        [HttpPost]
        [Route("refreshSession")]
        [Authorize]
        public ActionResult<TokenModel> RefreshSession([FromBody] TokenModel model)
        {
            return Ok(_userService.RefreshSession(model));
        }

        [HttpPost]
        [Route("createUser")]
        public ActionResult<TokenModel> CreateUser([FromBody] AddUserDto addUser)
        {
            return Ok(_userService.AddUserData(addUser));
        }

        [HttpPut]
        [Route("changeTariff")]
        [Authorize]
        public ActionResult ChangeTariff([FromBody] ChangeSupplierAndTariffDto tariff)
        {
            _userService.ChangeTariff(tariff);
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status200OK,
                Value = "Informacje o taryfie zostaly zmienione."
            };
        }
    }
}
