using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.DtoModels.ModelsForSearching;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using ConsumptionManagerBackend.Exceptions;

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
        public List<DeviceCategoryDto> GetCategories()
        {
            var categories = _context.device_category
                                        .ProjectTo<DeviceCategoryDto>(_mapper.ConfigurationProvider)
                                        .ToList();
            if (categories.Count == 0)
                throw new NoElementFoundException("Nie znaleziono zadnych elementow.");

            return categories;
        }

        public List<ViewDeviceDto> GetDevices()
        {
            var devices = _context.device
                                    .ProjectTo<ViewDeviceDto>(_mapper.ConfigurationProvider)
                                    .ToList();
            if (devices.Count ==0)
                throw new NoElementFoundException("Nie znaleziono zadnych elementow.");

            return devices;
        }

        public List<ViewDeviceDto>GetDevicesForCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                throw new WrongInputException("Prosze podac nazwe kategorii.");
            var devices = _context.device
                                    .Where(property => property.device_category.device_category_name.ToLower() == category.ToLower())
                                    .ProjectTo<ViewDeviceDto>(_mapper.ConfigurationProvider)
                                    .ToList();
            if (devices.Count == 0)
                throw new NoElementFoundException("Nie znaleziono zadnych elementow.");

            return devices;
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
