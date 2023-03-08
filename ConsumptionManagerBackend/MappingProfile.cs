using AutoMapper;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;

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
                .ForMember(destination => destination.electricity_tariff_id, map => map.MapFrom(baseClass => baseClass.ElectricityTariffId))
                .ForMember(destination => destination.user_surname, map => map.MapFrom(baseClass => baseClass.UserSurname))
                .ForMember(destination => destination.user_name, map => map.MapFrom(baseClass => baseClass.UserName))
                .ForMember(destination => destination.user_credentials_id, map => map.MapFrom(baseClass => baseClass.UserCredentialsId));
        }


    }
}
