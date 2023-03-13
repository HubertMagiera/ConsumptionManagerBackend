using AutoMapper;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;

namespace ConsumptionManagerBackend
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            //creating mapping profiles for classes
            //in case dto and non dto models have the same structure, CreateMap<base, destination>() is sufficient
            //in case names or strucutre is different, need to add .ForMember() to manually specify appropriate fields
            CreateMap<UserCredentialsDto, UserCredentials>()
                .ForMember(destination => destination.user_email, map => map.MapFrom(baseClass => baseClass.UserEmail))
                .ForMember(destination => destination.user_password, map => map.MapFrom(baseClass => baseClass.UserPassword));

            CreateMap<AddUserDto, User>()
                .ForMember(destination => destination.user_surname, map => map.MapFrom(baseClass => baseClass.UserSurname))
                .ForMember(destination => destination.user_name, map => map.MapFrom(baseClass => baseClass.UserName))
                .ForMember(destination => destination.user_credentials_id, map => map.MapFrom(baseClass => baseClass.UserCredentialsId));

            CreateMap<EnergySupplier, EnergySupplierWithTariffsDto>()
                .ForMember(destination => destination.EnergySupplierName, map => map.MapFrom(baseClass => baseClass.energy_supplier_name))
                .ForMember(destination => destination.tariffs, map => map.MapFrom(baseClass => baseClass.tariffs));

            CreateMap<ElectricityTariff, ElectricityTariffWithDetailsDto>()
                .ForMember(destination => destination.TariffName, map => map.MapFrom(baseClass => baseClass.tariff_name))
                .ForMember(destination => destination.TariffDescription, map => map.MapFrom(baseClass => baseClass.tariff_description))
                .ForMember(destination => destination.EnergySupplierName, map => map.MapFrom(baseClass => baseClass.energy_supplier.energy_supplier_name))
                .ForMember(destination => destination.TariffDetails, map => map.MapFrom(baseClass => baseClass.tariff_details));

            CreateMap<TariffDetails, TariffDetailsDto>()
                .ForMember(destination => destination.DayOfWeekName, map => map.MapFrom(baseClass => baseClass.day_of_week.day_name))
                .ForMember(destination => destination.StartTime, map => map.MapFrom(baseClass => baseClass.start_time))
                .ForMember(destination => destination.EndTime, map => map.MapFrom(baseClass => baseClass.end_time))
                .ForMember(destination => destination.PricePerKwh, map => map.MapFrom(baseClass => baseClass.price_per_kwh));

            CreateMap<EnergySupplier, EnergySupplierDto>()
                .ForMember(destination => destination.EnergySupplierName, map => map.MapFrom(baseClass => baseClass.energy_supplier_name));

            CreateMap<ElectricityTariff, ElectricityTariffDto>()
                .ForMember(destination => destination.TariffName, map => map.MapFrom(baseClass => baseClass.tariff_name))
                .ForMember(destination => destination.TariffDescription, map => map.MapFrom(baseClass => baseClass.tariff_description))
                .ForMember(destination => destination.EnergySupplierName, map => map.MapFrom(baseClass => baseClass.energy_supplier.energy_supplier_name));
        }


    }
}
