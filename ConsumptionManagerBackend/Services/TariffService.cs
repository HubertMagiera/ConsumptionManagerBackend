using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using ConsumptionManagerBackend.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
            //projectTo method allows to get only those columns which are used in dto model instead of collecting all columns from db
            var tariffsForSupplier = _context.electricity_tariff
                                      .Where(tariff => tariff.energy_supplier_id == energySupplier.energy_supplier_id)                                      
                                      .ProjectTo<ElectricityTariffDto>(_mapper.ConfigurationProvider)
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
        
        public List<ElectricityTariffWithDetailsDto> GetDetailsForElectricityTariff(ElectricityTariffModel tariff)
        {
            //method used for receiving details for electricity tariff

            //validation if both reqiured fields are provided
            if (string.IsNullOrEmpty(tariff.ElectricityTariffName) || string.IsNullOrEmpty(tariff.EnergySupplierName))
                throw new WrongInputException("Prosze podac nazwe taryfy oraz nazwe dostawcy pradu.");
            //dokonczyc
            var tariffDetails = _context.electricity_tariff
                                .ProjectTo<ElectricityTariffWithDetailsDto>(_mapper.ConfigurationProvider)
                                .ToList();

            return tariffDetails;
        }
    }
}
