using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.DtoModels.ModelsForSearching;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using ConsumptionManagerBackend.Exceptions;
using ConsumptionManagerBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConsumptionManagerBackend.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly EnergySaverDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public DeviceService(EnergySaverDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
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
            if (devices.Count == 0)
                throw new NoElementFoundException("Nie znaleziono zadnych elementow.");

            return devices;
        }

        public List<ViewDeviceDto> GetDevicesForCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                throw new WrongInputException("Prosze podac nazwe kategorii.");
            var devices = GetDevices()
                          .Where(property => property.CategoryName.ToLower() == category.ToLower())
                          .ToList();
            if (devices.Count == 0)
                throw new NoElementFoundException("Nie znaleziono zadnych elementow.");

            return devices;
        }

        public ViewUserDeviceDto GetUserDevice(SearchForUserDeviceDto deviceToFind)
        {
            if (string.IsNullOrEmpty(deviceToFind.DeviceName) || string.IsNullOrEmpty(deviceToFind.DeviceCategory))
                throw new NotAllDataProvidedException("Prosze podac nazwe urzadzenia i kategorii.");
            var device = _context.user_device
                                    .Include(property => property.details)
                                    .Include(property => property.device)
                                    .Include(property => property.user)
                                    .Include(property => property.device.device_category)
                                    .FirstOrDefault(property => property.device.device_name == deviceToFind.DeviceName
                                        && property.device.device_category.device_category_name == deviceToFind.DeviceCategory
                                        && property.user_id == _userService.GetUserID());
            if (device == null)
                throw new NoElementFoundException("Nie znaleziono zadnego urzadzenia dla podanych danych");
            return _mapper.Map<ViewUserDeviceDto>(device);
                                    
        }

        public List<ViewUserDeviceDto> GetUserDevices()
        {
            var devices = _context.user_device
                                    .Where(property => property.user_id == _userService.GetUserID())
                                    .ProjectTo<ViewUserDeviceDto>(_mapper.ConfigurationProvider)
                                    .ToList();
            if (devices.Count == 0)
                throw new NoElementFoundException("Nie znaleziono zadnych elementow");
            return devices;
        }
    }
}
