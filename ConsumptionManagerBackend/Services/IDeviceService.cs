using ConsumptionManagerBackend.DtoModels.ModelsForSearching;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;

namespace ConsumptionManagerBackend.Services
{
    public interface IDeviceService
    {
        List<string> GetCategories();

        List<ViewDeviceDto> GetDevices();

        List<ViewUserDeviceDto> GetUserDevices();

        ViewUserDeviceDto GetUserDevice(SearchForUserDeviceDto deviceToFind);


    }
}
