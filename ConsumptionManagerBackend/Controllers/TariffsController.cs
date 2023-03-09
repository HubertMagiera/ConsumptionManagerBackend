using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using Microsoft.AspNetCore.Mvc;

namespace ConsumptionManagerBackend.Controllers
{
    [ApiController]
    [Route("consumptionManager/tariffs")]
    public class TariffsController:ControllerBase
    {
        [HttpGet]
        [Route("energySuppliers")]
        public ActionResult<EnergySupplierDto> GetAllEnergySuppliers()
        {

        }
    }
}
