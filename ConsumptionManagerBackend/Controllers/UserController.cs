using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.Services;
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
            _userService.RegisterUser(userCredentials);
            return new ObjectResult(null){
                StatusCode = StatusCodes.Status201Created
            };
        }
    }
}
