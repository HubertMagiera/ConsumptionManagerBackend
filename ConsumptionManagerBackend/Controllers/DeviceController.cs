using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForSearching;
using ConsumptionManagerBackend.DtoModels.ModelsForUpdates;
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
        [Authorize]
        public ActionResult<List<DeviceCategoryDto>> GetDeviceCategories()
        {
            return Ok(_deviceService.GetCategories());
        }

        [HttpGet]
        [Route("all")]
        [Authorize]
        public ActionResult<List<ViewDeviceDto>>GetAllDevices()
        {
            return Ok(_deviceService.GetDevices());
        }

        [HttpGet]
        [Route("{category}")]
        [Authorize]
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
                StatusCode = StatusCodes.Status201Created,
                Value = "Dodano nowe urzadzenie"
            };
        }

        [HttpPut]
        [Route("myDevices/changeStatus")]
        [Authorize]
        public ActionResult ChangeUserDeviceStatus([FromBody] SearchForUserDeviceDto deviceToUpdate)
        {
            _deviceService.ChangeUserDeviceStatus(deviceToUpdate);
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status200OK,
                Value = "Status urzadzenia zostal zmieniony."
            };
        }

        [HttpPost]
        [Route("myDevice/addDeviceDetails")]
        [Authorize]
        public ActionResult AddDeviceDetails([FromBody] AddUserDeviceDetailsDto details)
        {
            _deviceService.AddDetailsToUserDevice(details);
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status201Created,
                Value = "Dodano nowy tryb pracy dla tego urzadzenia."
            };
        }

        [HttpPut]
        [Route("myDevice/updateDeviceDetails")]
        [Authorize]
        public ActionResult DeleteDeviceDetails([FromBody] AddUserDeviceDetailsDto details)
        {
            _deviceService.UpdateUserDeviceDetails(details);
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status200OK,
                Value = "Zaktualizowano tryb pracy urzadzenia."
            };
        }
    }
}
