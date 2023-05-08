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
using Microsoft.EntityFrameworkCore.Design;

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
            //method used to get all available device categories from database
            var categories = _context.device_category
                                        .ProjectTo<DeviceCategoryDto>(_mapper.ConfigurationProvider)
                                        .ToList();
            if (categories.Count == 0)
                throw new NoElementFoundException("Nie znaleziono zadnych elementow.");

            return categories;
        }

        public List<ViewDeviceDto> GetDevices()
        {
            //method used to read all devices from database
            var devices = GetDevicesFromDB();
            if (devices.Count == 0)
                throw new NoElementFoundException("Nie znaleziono zadnych elementow.");

            return _mapper.Map<List<ViewDeviceDto>>(devices);
        }

        public List<ViewDeviceDto> GetDevicesForCategory(string category)
        {
            //mathod used to read all devices from database which are assigned to provided category
            if (string.IsNullOrEmpty(category))
                throw new WrongInputException("Prosze podac nazwe kategorii.");
            var devices = GetDevicesFromDB()
                          .Where(property => property.device_category.device_category_name.ToLower() == category.ToLower())
                          .ToList();
            if (devices.Count == 0)
                throw new NoElementFoundException("Nie znaleziono zadnych elementow.");

            return _mapper.Map<List<ViewDeviceDto>>(devices);
        }

        public ViewUserDeviceDto GetUserDevice(SearchForUserDeviceDto deviceToFind)
        {
            //method used to return data about specified user device
            if (string.IsNullOrEmpty(deviceToFind.DeviceName) || string.IsNullOrEmpty(deviceToFind.DeviceCategory))
                throw new NotAllDataProvidedException("Prosze podac nazwe urzadzenia i kategorii.");

            var device = GetUserDevicesFromDB().FirstOrDefault(property => property.device.device_name.ToLower() == deviceToFind.DeviceName.ToLower() &&
                                                                    property.device.device_category.device_category_name.ToLower() == deviceToFind.DeviceCategory.ToLower());
            if (device == null)
                throw new NoElementFoundException("Nie znaleziono zadnego urzadzenia dla podanych danych");
            return _mapper.Map<ViewUserDeviceDto>(device);
                                    
        }

        public List<ViewUserDeviceDto> GetUserDevices()
        {
            //method used to get all devices assigned to current user
            var devices = GetUserDevicesFromDB();
            if (devices.Count == 0)
                throw new NoElementFoundException("Nie znaleziono zadnych elementow");
            return _mapper.Map<List<ViewUserDeviceDto>>(devices);
        }

        public void AddUserDevice(AddUserDeviceDto deviceToAdd)
        {
            //method used to add new device to list of user devices
            //user needs to provide info about device name, category and max power of device

            bool nameAndCategoryProvided = string.IsNullOrEmpty(deviceToAdd.DeviceName) == false && string.IsNullOrEmpty(deviceToAdd.DeviceCategoryName) == false;
            bool maxPowerProvided = deviceToAdd.DeviceMaxPower <= 0;

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

            //get device id and add new user device to user account
            var device = GetDevicesFromDB().FirstOrDefault(property => property.device_name.ToLower() == deviceToAdd.DeviceName.ToLower() && property.device_category_id == categoryID);
            if (device == null)
                throw new NoElementFoundException("Nie znaleziono urzadzenia o podanych parametrach.");
            int deviceID = device.device_id;

            //check if user already owns device with the same name and category
            var deviceWithTheSameData =GetUserDevicesFromDB().FirstOrDefault(property => property.user_id == userID && property.device_id == deviceID);
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

        public void ChangeUserDeviceStatus(SearchForUserDeviceDto deviceToFind)
        {
            //method used to change user device status from active to inactive and opposite
            
            //check if user provided all required data
            if (string.IsNullOrEmpty(deviceToFind.DeviceName) || string.IsNullOrEmpty(deviceToFind.DeviceCategory))
                throw new NotAllDataProvidedException("Podaj nazwe oraz kategorie urzadzenia, ktorego status chcesz zmienic.");

            //if yes, check if user device for provided details exists
            var userDevice = GetUserDevicesFromDB().FirstOrDefault(property => property.device.device_name.ToLower() == deviceToFind.DeviceName.ToLower() &&
                                                                   property.device.device_category.device_category_name.ToLower() == deviceToFind.DeviceCategory.ToLower());
            //throw an error if user device does not exist
            if (userDevice == null)
                throw new NoElementFoundException("Nie znaleziono urzadzenia dla podanych wartosci.");
            //if exists, change its status
            userDevice.is_active = !userDevice.is_active;
            _context.SaveChanges();
        }

        public void AddDetailsToUserDevice(AddUserDeviceDetailsDto details)
        {
            //method used to add device details to user device

            //check if user provided all required details and if data is correct
            var device = CheckDeviceDetails(details);

            //check if power mode with the same power amount already exists for this device
            //or if power mode with the same number exists
            var modeWithTheSamePowerAmount = device.details.FirstOrDefault(property => property.device_power_in_mode == details.DevicePowerInMode ||
                                                                            property.device_mode_number == details.ModeNumber);
            if (modeWithTheSamePowerAmount != null)
                throw new WrongInputException("Tryb pracy o z podana moca lub o takim samym numerze juz istnieje");

            //create details that will be added to database
            DeviceDetails detailsToBeAdded = _mapper.Map<DeviceDetails>(details);
            detailsToBeAdded.user_device_id = device.user_device_id;

            //save it to db
            _context.device_details.Add(detailsToBeAdded);
            _context.SaveChanges();
        }

        public void UpdateUserDeviceDetails(AddUserDeviceDetailsDto details)
        {
            //method used to update power mode in user device
            //user can change power in specified mode or can change its description

            //check if user provided all required details and if data is correct
            var device = CheckDeviceDetails(details);

            //check if details with provided mode number exists
            var detailsToUpdate = device.details.FirstOrDefault(property => property.device_mode_number == details.ModeNumber);
            if (detailsToUpdate == null)
                throw new NoElementFoundException("Nie znaleziono danych dla podanego oznaczenia trybu pracy. Prosze sprawdzic poprawnosc danych.");

            detailsToUpdate.device_mode_description = details.DeviceModeDescription;
            detailsToUpdate.device_power_in_mode = details.DevicePowerInMode;
            _context.SaveChanges();
        }

        private List<Device>GetDevicesFromDB()
        {
            //method used to get all devices from database
            //this method is invoked in other methods which return list of devices
            var devices = _context.device.Include(device => device.device_category).ToList();
            return devices;
        }

        private List<UserDevice>GetUserDevicesFromDB()
        {
            //method used to get all devices assigned to user who sent the request
            //this method is later invoked in other methods which require access to all devices assigned to a user
            var userDevices = _context.user_device
                                        .Where(property => property.user_id == _userService.GetUserID())
                                        .Include(userDev => userDev.device)
                                        .Include(userDev => userDev.device.device_category)
                                        .Include(userDev =>userDev.details)
                                        .ToList();
            return userDevices;

        }

        private UserDevice CheckDeviceDetails(AddUserDeviceDetailsDto details)
        {
            //check if user provided all required details
            if (string.IsNullOrEmpty(details.DeviceName) || string.IsNullOrEmpty(details.DeviceCategory) || details.DevicePowerInMode <= 0 || details.ModeNumber <= 0)
                throw new NotAllDataProvidedException("Prosze podac wszystkie wymagane dane.");

            //if all data provided, check if user owns device with the same name and category as provided
            var device = GetUserDevicesFromDB().FirstOrDefault(property => property.device.device_name.ToLower() == details.DeviceName.ToLower() &&
                                                                property.device.device_category.device_category_name.ToLower() == details.DeviceCategory.ToLower());
            if (device == null)
                throw new NoElementFoundException("Nie znaleziono urzadzenia dla podanych danych.");

            //if user device exists, check if provided power amount is not greater than max power allowed for this device
            if (details.DevicePowerInMode > device.device_max_power)
                throw new WrongInputException("Podano wieksza moc niz maksymalna przewidziana dla urzadzenia. Prosze podac inne dane.");
            //return device in case all data is provided and correct
            return device;
        }



    }
}
