using ConsumptionManagerBackend.DtoModels.ModelsForSearching;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;

namespace ConsumptionManagerBackend.Services
{
    public class DeviceService : IDeviceService
    {
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
