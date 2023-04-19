using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using ConsumptionManagerBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ConsumptionManagerBackend.Controllers
{
    [ApiController]
    [Route("consumptionManager/measurement")]
    public class MeasurementController: ControllerBase
    {
        private readonly IMeasurementService _measurementService;

        public MeasurementController(IMeasurementService measurementService)
        {
            _measurementService = measurementService;
        }

        [HttpGet]
        [Route("all")]
        [Authorize]
        public ActionResult<List<MeasurementDto>> AllMeasurements()
        {
            return Ok(_measurementService.GetAllMeasurements());
        }

        [HttpGet]
        [Route("all/{category}")]
        [Authorize]
        public ActionResult<List<MeasurementDto>> MeasurementsForCategory([FromRoute] string category)
        {
            return Ok(_measurementService.MeasurementsForCategory(category));
        }

        [HttpGet]
        [Route("all/betweenDates")]
        [Authorize]
        public ActionResult<List<MeasurementDto>> MeasurementsBetweenDates([FromBody] DatesInterval dates)
        {
            return Ok(_measurementService.MeasurementsBetweenDates(dates));
        }

        [HttpPost]
        [Route("newMeasurement")]
        [Authorize]
        public ActionResult AddNewMeasurement([FromBody] AddMeasurementDto measurement)
        {
            _measurementService.AddNewMeasurement(measurement);
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status201Created,
                Value = "Zarejestrowano nowy pomiar."
            };
        }

        [HttpPost]
        [Route("newMeasurementWithSchedule")]
        [Authorize]
        public ActionResult AddNewMeasurementWithSchedule([FromBody] AddMeasurementWithScheduleDto measurement)
        {
            _measurementService.AddNewMeasurementWithSchedule(measurement);
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status201Created,
                Value = "Zarejestrowano nowy pomiar."
            };
        }
    }
}
