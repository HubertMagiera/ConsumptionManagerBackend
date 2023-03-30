using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForSearching;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;

namespace ConsumptionManagerBackend.Services.Interfaces
{
    public interface IDeviceService
    {
        List<DeviceCategoryDto> GetCategories();

        List<ViewDeviceDto> GetDevices();

        List<ViewUserDeviceDto> GetUserDevices();

        ViewUserDeviceDto GetUserDevice(SearchForUserDeviceDto deviceToFind);

        List<ViewDeviceDto> GetDevicesForCategory(string category);

        void AddUserDevice(AddUserDeviceDto deviceToAdd);
    }
}
