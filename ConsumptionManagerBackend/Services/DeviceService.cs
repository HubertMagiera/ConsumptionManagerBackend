using AutoMapper;
using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.DtoModels.ModelsForSearching;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;

namespace ConsumptionManagerBackend.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly EnergySaverDbContext _context;
        private readonly IMapper _mapper;

        public DeviceService(EnergySaverDbContext context, IMapper mapper)
        {
            _context= context;
            _mapper= mapper;
        }
        public List<string> GetCategories()
        {
            throw new NotImplementedException();
        }

        public List<ViewDeviceDto> GetDevices()
        {
            throw new NotImplementedException();
        }

        public ViewUserDeviceDto GetUserDevice(SearchForUserDeviceDto deviceToFind)
        {
            throw new NotImplementedException();
        }

        public List<ViewUserDeviceDto> GetUserDevices()
        {
            throw new NotImplementedException();
        }
    }
}
