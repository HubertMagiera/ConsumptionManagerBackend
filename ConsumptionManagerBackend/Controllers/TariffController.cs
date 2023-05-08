using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using ConsumptionManagerBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConsumptionManagerBackend.Controllers
{
    [ApiController]
    [Route("consumptionManager/tariff")]
    public class TariffController:ControllerBase
    {
        private readonly ITariffService _tariffService;
        public TariffController(ITariffService tariffService)
        {
            _tariffService = tariffService;
        }

        [HttpGet]
        [Route("energySuppliers")]
        //this endpoint does not require authorization because it can be used while creating new account
        public ActionResult<EnergySupplierDto> GetAllEnergySuppliers()
        {
            return Ok(_tariffService.GetEnergySuppliers());
        }

        [HttpGet]
        [Route("supplierTariffs/{name}")]
        //this endpoint does not require authorization because it can be used while creating new account
        public ActionResult<List<ElectricityTariffDto>> GetElectricityTariffsForSupplier([FromRoute] string name)
        {
            return Ok(_tariffService.GetElectricityTariffsForEnergySupplier(name));
        }

        [HttpGet]
        [Route("tariffDetails")]
        //this endpoint does not require authorization because it can be used while creating new account
        public ActionResult<List<ElectricityTariffWithDetailsDto>> GetTariffDetails([FromBody] ElectricityTariffModel model)
        {
            return Ok(_tariffService.GetDetailsForElectricityTariff(model));
        }
    }
}
