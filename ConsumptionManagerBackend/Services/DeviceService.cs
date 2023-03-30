using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
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
            var device = GetUserDevices().FirstOrDefault(property => property.DeviceName.ToLower() == deviceToFind.DeviceName.ToLower() &&
                                                                    property.DeviceCategory.ToLower() == deviceToFind.DeviceCategory.ToLower());
            if (device == null)
                throw new NoElementFoundException("Nie znaleziono zadnego urzadzenia dla podanych danych");
            return device;
                                    
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

        public void AddUserDevice(AddUserDeviceDto deviceToAdd)
        {
            //method used to add new device to list of user devices
            //user needs to provide info about device name, category and max power of device

            bool nameAndCategoryProvided = string.IsNullOrEmpty(deviceToAdd.DeviceName) == false && string.IsNullOrEmpty(deviceToAdd.DeviceCategoryName) == false;
            bool maxPowerProvided = deviceToAdd.DeviceMaxPower == 0;

            //if some of the data is missing, throw an exception
            if (nameAndCategoryProvided == false && maxPowerProvided == false)
                throw new NotAllDataProvidedException("Prosze podac wszystkie wymagane dane.");

            //get id of user who sent the request
            int userID = _userService.GetUserID();

            //get id of provided category
            var category = _context.device_category
                                .FirstOrDefault(property => property.device_category_name.ToLower() == deviceToAdd.DeviceCategoryName.ToLower());
            if (category == null)
                throw new WrongInputException("Nie znaleziono takiej kategorii jak podana. Prosze podac poprawna nazwe kategorii.");
            int categoryID = category.device_category_id;

            //if not, get device id and add new user device to user account
            var device = _context.device
                                 .FirstOrDefault(property => property.device_name.ToLower() == deviceToAdd.DeviceName.ToLower() && property.device_category_id == categoryID);
            if (device == null)
                throw new NoElementFoundException("Nie znaleziono urzadzenia o podanych parametrach.");
            int deviceID = device.device_id;

            //check if user already owns device with the same name and category
            var deviceWithTheSameData = _context.user_device
                                   .FirstOrDefault(property => property.user_id == userID && property.device_id == deviceID);
            //if yes, return appropriate error message
            if(deviceWithTheSameData != null)
            {
                if (deviceWithTheSameData.is_active)
                    throw new UserDeviceAlreadyExistsException("Posiadasz juz takie urzadzenie.");
                else if (deviceWithTheSameData.is_active == false)
                    throw new UserDeviceAlreadyExistsException("Posiadasz juz takie urzadzenie, ale jest ono nieaktywne. Mozesz je reaktywowac.");
            }

            //if no, proceed with inserting new user device
            var deviceToBeAdded = new UserDevice()
            {
                user_id = userID,
                device_max_power = deviceToAdd.DeviceMaxPower,
                device_id = deviceID,
                is_active = true
            };
            _context.user_device.Add(deviceToBeAdded);
            _context.SaveChanges();
            
        }
    }
}
