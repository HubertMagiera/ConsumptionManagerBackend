using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForSearching;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using ConsumptionManagerBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsumptionManagerBackend.Controllers
{
    [ApiController]
    [Route("consumptionManager/device")]
    public class DeviceController:ControllerBase
    {
        private readonly IDeviceService _deviceService;
        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        [Route("categories")]
        //[Authorize]
        public ActionResult<List<DeviceCategoryDto>> GetDeviceCategories()
        {
            return Ok(_deviceService.GetCategories());
        }

        [HttpGet]
        [Route("")]
        //[Authorize]
        public ActionResult<List<ViewDeviceDto>>GetAllDevices()
        {
            return Ok(_deviceService.GetDevices());
        }

        [HttpGet]
        [Route("{category}")]
        //[Authorize]
        public ActionResult<List<ViewDeviceDto>> GetDevicesForCategory([FromRoute] string category)
        {
            return Ok(_deviceService.GetDevicesForCategory(category));
        }
        [HttpGet]
        [Route("myDevices")]
        [Authorize]
        public ActionResult<List<ViewDeviceDto>> GetUserDevices()
        {
            return Ok(_deviceService.GetUserDevices());
        }
        [HttpGet]
        [Route("myDevice")]
        [Authorize]
        public ActionResult<ViewDeviceDto> GetUserDevice([FromBody] SearchForUserDeviceDto deviceToFind)
        {
            return Ok(_deviceService.GetUserDevice(deviceToFind));
        }

        [HttpPost]
        [Route("myDevices/addNew")]
        [Authorize]
        public ActionResult AddNewUserDevice([FromBody] AddUserDeviceDto deviceToBeAdded)
        {
            _deviceService.AddUserDevice(deviceToBeAdded);
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status201Created
        };
        }
    }
}
