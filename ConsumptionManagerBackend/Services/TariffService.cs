using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using ConsumptionManagerBackend.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ConsumptionManagerBackend.Services
{
    public class TariffService : ITariffService
    {
        private readonly EnergySaverDbContext _context;
        private readonly IMapper _mapper;   
        public TariffService(EnergySaverDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public List<ElectricityTariffDto> GetElectricityTariffsForEnergySupplier(string energySupplierName)
        {
            //method used to get all tariffs available for specified energy supplier
            if (string.IsNullOrEmpty(energySupplierName))
                throw new WrongInputException("Prosze podac nazwe dostawcy pradu");

            //validation if snergy supplier with provided name exists in db
            var energySupplier = _context.energy_supplier.FirstOrDefault(supplier => supplier.energy_supplier_name.ToLower() == energySupplierName.ToLower());
            if (energySupplier == null)
                throw new WrongInputException("Prosze podac poprawna nazwe dostawcy");

            //if validation is successfull, return all tariffs for energy supplier (without their details)
            //as no tracking method disables tracking changes for list elements, it allows to boost performance (good for view only scenarios)
            //projectTo method allows to get only those columns which are used in dto model instead of collecting all columns from db
            var tariffsForSupplier = _context.electricity_tariff
                                      .Where(tariff => tariff.energy_supplier_id == energySupplier.energy_supplier_id)                                      
                                      .ProjectTo<ElectricityTariffDto>(_mapper.ConfigurationProvider)
                                      .AsNoTracking() 
                                      .ToList();
            return tariffsForSupplier;
        }

        public List<EnergySupplierDto> GetEnergySuppliers()
        {
            var suppliersToReturn = _context.energy_supplier
                                    .ProjectTo<EnergySupplierDto>(_mapper.ConfigurationProvider)
                                    .AsNoTracking()
                                    .ToList();
            return suppliersToReturn;
        }
        //TO BE ADDED - GET DETAILS FOR SPECIFIED TARIFF AND ALSO VALIDATION IN ADD USER METHOD OF USER SERVICE CLASS
    }
}
