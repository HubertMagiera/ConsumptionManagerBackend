using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using ConsumptionManagerBackend.Exceptions;
using ConsumptionManagerBackend.Services.Interfaces;
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
                                    .ToList();
            return suppliersToReturn;
        }
        
        public List<ElectricityTariffWithDetailsDto> GetDetailsForElectricityTariff(ElectricityTariffModel tariff)
        {
            //method used for receiving details for electricity tariff

            //validation if both reqiured fields are provided
            if (string.IsNullOrEmpty(tariff.ElectricityTariffName) || string.IsNullOrEmpty(tariff.EnergySupplierName))
                throw new WrongInputException("Prosze podac nazwe taryfy oraz nazwe dostawcy pradu.");

            //select tariff details for provided data
            var tariffDetails = _context.electricity_tariff
                                .Where(property => property.energy_supplier.energy_supplier_name.ToLower() == tariff.EnergySupplierName.ToLower() 
                                        && property.tariff_name.ToLower() == tariff.ElectricityTariffName.ToLower())
                                .ProjectTo<ElectricityTariffWithDetailsDto>(_mapper.ConfigurationProvider)
                                .ToList();
            //if list is null it means that input is not correct, so throw an error with appropriate message
            if (!tariffDetails.Any())
                throw new WrongInputException("Nie znaleziono zadnych danych dla podanych wartosci. Prosze sprawdzic poprawnosc wprowadzonych danych");

            return tariffDetails;
        }
    }
}
