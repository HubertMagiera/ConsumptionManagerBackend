using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using ConsumptionManagerBackend.Services;
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
        [Route("devices")]
        //[Authorize]
        public ActionResult<List<ViewDeviceDto>>GetAllDevices()
        {
            return Ok(_deviceService.GetDevices());
        }

        [HttpGet]
        [Route("devices/{category}")]
        //[Authorize]
        public ActionResult<List<ViewDeviceDto>> GetDevicesForCategory([FromRoute] string category)
        {
            return Ok(_deviceService.GetDevicesForCategory(category));
        }
    }
}
