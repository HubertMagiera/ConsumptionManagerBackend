using AutoMapper;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels;

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
        }


    }
}
