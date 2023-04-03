using AutoMapper;
using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using ConsumptionManagerBackend.Exceptions;
using ConsumptionManagerBackend.Services.Interfaces;

namespace ConsumptionManagerBackend.Services
{
    public class MeasurementService : IMeasurementService
    {
        private readonly EnergySaverDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public MeasurementService(EnergySaverDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public void AddNewMeasurement(AddMeasurementDto measurement)
        {
            throw new NotImplementedException();
        }

        public void AddNewMeasurementWithSchedule(AddMeasurementWithScheduleDto measurement)
        {
            throw new NotImplementedException();
        }

        public List<MeasurementDto> GetAllMeasurements()
        {
            return _mapper.Map<List<MeasurementDto>>(GetMeasurementsFromDb());
        }

        public List<MeasurementDto> MeasurementsBetweenDates(DateTime startDate, DateTime endDate)
        {
            if (DateTime.Compare(startDate, endDate) > 0)
                throw new WrongInputException("Data zakonczenia pomiaru nie moze byc wczesniejsza od daty jego rozpoczecia.");

            var measurements = GetMeasurementsFromDb();
            measurements = measurements.Where(property => property.measurement_start_date >= startDate && property.measurement_end_date <= endDate).ToList();

            if (measurements.Count == 0)
                throw new NoElementFoundException("Nie zarejestrowano zadnego pomiaru pomiedzy wskazanymi datami.");

            return _mapper.Map<List<MeasurementDto>>(measurements);
        }

        public List<MeasurementDto> MeasurementsForCategory(string category)
        {
            var measurements = GetMeasurementsFromDb();
            measurements = measurements.Where(property => property.userDevice.device.device_category.device_category_name.ToLower() == category.ToLower()).ToList();

            if (measurements.Count == 0)
                throw new NoElementFoundException("Nie odnaleziono pomiarow dla wskazanej kategorii urzadzen.");

            return _mapper.Map<List<MeasurementDto>>(measurements);
        }

        private List<Measurement> GetMeasurementsFromDb()
        {
            int userID = _userService.GetUserID();

            var measurements = _context.measurement.Where(property => property.user_id == userID).ToList();

            if (measurements.Count == 0)
                throw new NoElementFoundException("Uzytkownik nie ma zarejestrowanego zadnego pomiaru.");

            return measurements;
        }
    }
}
