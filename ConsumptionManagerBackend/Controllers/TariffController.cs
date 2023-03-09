using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using ConsumptionManagerBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConsumptionManagerBackend.Controllers
{
    [ApiController]
    [Route("consumptionManager/tariffs")]
    public class TariffController:ControllerBase
    {
        private readonly ITariffService _tariffService;
        public TariffController(ITariffService tariffService)
        {
            _tariffService = tariffService;
        }

        [HttpGet]
        [Route("energySuppliers")]
        public ActionResult<EnergySupplierDto> GetAllEnergySuppliers()
        {
            return Ok(_tariffService.GetEnergySuppliers());
        }

        [HttpGet]
        [Route("supplierTariffs/{name}")]
        public ActionResult<List<ElectricityTariffDto>> GetElectricityTariffsForSupplier([FromRoute] string name)
        {
            return Ok(_tariffService.GetElectricityTariffsForEnergySupplier(name));
        }
    }
}
