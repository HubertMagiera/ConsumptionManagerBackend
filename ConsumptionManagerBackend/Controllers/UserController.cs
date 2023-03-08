using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForUpdates;
using ConsumptionManagerBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsumptionManagerBackend.Controllers
{
    [Route("consumptionManager/user")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [Route("register")]
        public ActionResult RegisterNewUser(UserCredentialsDto userCredentials)
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
        public ActionResult ChangePassword(ChangePasswordDto changePassword)
        {
            _userService.ChangePassword(changePassword);
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
        [HttpPost]
        [Route("login")]
        public ActionResult<TokenModel> LoginUser(UserCredentialsDto loginUser)
        {
            return Ok(_userService.LoginUser(loginUser));

        }

        [HttpPost]
        [Route("refreshSession")]
        [Authorize]
        public ActionResult<TokenModel> RefreshSession(TokenModel model)
        {
            return Ok(_userService.RefreshSession(model));
        }

        [HttpPost]
        [Route("CreateUser")]
        public ActionResult CreateUser(AddUserDto addUser)
        {
            _userService.AddUserData(addUser);
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
    }
}
