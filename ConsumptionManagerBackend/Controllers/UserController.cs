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
            return Ok(_userService.LoginUser(loginUser));

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
    }
}
