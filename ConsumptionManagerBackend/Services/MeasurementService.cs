﻿using AutoMapper;
using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using ConsumptionManagerBackend.Exceptions;
using ConsumptionManagerBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Threading;

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

        public void AddMeasurementsBasedOnSchedule(int userID)
        {
            //method used to check if measurements with created schedules should be refreshed

            //get user measurements which have schedules assigned
            var userMeasurements = _context.measurement.Where(property => property.user_id == userID).ToList();
            var schedules = _context.schedule.ToList();
            List<MeasurementsWithSchedule> measurments = new List<MeasurementsWithSchedule>();

            //create list of user measurements which have schedule created
            foreach(var measurement in userMeasurements)
            {
                var scheduleForMeasurement = schedules.FirstOrDefault(property => property.measurement_id == measurement.measurement_id);
                if (scheduleForMeasurement == null)
                    continue;
                measurments.Add(new MeasurementsWithSchedule
                {
                    Measurement = measurement,
                    Schedule = scheduleForMeasurement
                });
            }

            foreach(var measurement in measurments)
            {
                //get date difference between today and the date of measurement
                int howManyDays = (DateTime.Now - measurement.Measurement.measurement_added_date).Days;

                //if date diff is 0 days, go to next iteration
                if (howManyDays == 0)
                    continue;

                //else check how many measurements should be added
                //based on schedule frequency and date difference
                int howManyMeasurements;
                int frequency = measurement.Schedule.schedule_frequency;
                if (frequency == 1)
                    //if schedule frequency is set to every day, add as many measurements as date difference 
                    howManyMeasurements = howManyDays;
                //else get number of measurements to be added by division of date difference and schedule frequency
                else
                {
                    if (howManyDays < frequency)
                        break;
                    howManyMeasurements = Convert.ToInt32(Math.Floor((double)howManyDays / frequency));
                }
                
                //loop to add as many measurements as indicated in howManyMeasurements variable
                for(int i = 0;i < howManyMeasurements;i++)
                {
                    //get length of this measurement (hours)
                    var measurementLength = (measurement.Measurement.measurement_end_date - measurement.Measurement.measurement_start_date).TotalHours;

                    //get measurement to be added and adjust start, end and added date by adding schedule frequency to each of them
                    var measurementToAdd = new AddMeasurementAllData()
                    {
                        device_power_in_mode = Convert.ToInt32(measurement.Measurement.energy_used * 1000 / measurementLength),
                        user_id = measurement.Measurement.user_id,
                        user_device_id = measurement.Measurement.user_device_id,
                        measurement_start_date = measurement.Measurement.measurement_start_date.AddDays(frequency),
                        measurement_end_date = measurement.Measurement.measurement_end_date.AddDays(frequency),
                        measurement_added_date = measurement.Measurement.measurement_added_date.AddDays(frequency)
                    };

                    int newMeasurementID = RegisterMeasurement(measurementToAdd);

                    //update schedule, change measurement id which is assigned to it
                    measurement.Schedule.measurement_id = newMeasurementID;
                    _context.SaveChanges();

                    //manually change measurement which is indicated in current iteration
                    //this allows to assign correct dates in next iterations of this for loop
                    measurement.Measurement = _context.measurement.FirstOrDefault(property => property.measurement_id == newMeasurementID);
                }

            }
        }

        public int AddNewMeasurement(AddMeasurementDto measurement)
        {
            //method used to register new measurement in database
            //this method performs data validation and collects other data required to add new measurement
            //once the data is validated and collected it calls next method which registers the measurement in database

            //check if all data is provided
            bool deviceProvided = string.IsNullOrEmpty(measurement.Device.DeviceName) == false && string.IsNullOrEmpty(measurement.Device.DeviceCategory) == false &&
                                                        measurement.DeviceModeNumber != 0;
            bool datesProvided = measurement.StartDate != DateTime.MinValue && measurement.EndDate != DateTime.MinValue;

            if (deviceProvided == false || datesProvided == false)
                throw new NotAllDataProvidedException("Prosze podac wszystkie wymagane dane.");
            //check if end date is not earlier than start date
            bool datesCorrect = (DateTime.Compare(measurement.StartDate, measurement.EndDate) >= 0) == false;
            if (datesCorrect == false)
                throw new WrongInputException("Data zakonczenia pomiaru nie moze byc wczesniejsza od daty jego rozpoczecia.");

            int userID = _userService.GetUserID();

            //get device from db
            var userDevices = _context.user_device
                                        .Where(property => property.user_id == userID)
                                        .Include(userDev => userDev.device)
                                        .Include(userDev => userDev.device.device_category)
                                        .Include(userDev => userDev.details)
                                        .ToList();
            var device = userDevices.FirstOrDefault(property => property.device.device_name.ToLower() == measurement.Device.DeviceName.ToLower() &&
                                                                property.device.device_category.device_category_name.ToLower() == measurement.Device.DeviceCategory.ToLower() &&
                                                                property.user_id == userID);
            if (device == null)
                throw new NoElementFoundException("Nie odnaleziono urzadzenia dla podanych danych.");
            if (device.is_active == false)
                throw new WrongInputException("Wskazane urzadzenie jest oznaczone jako nieaktywne.");
            var deviceMode = device.details.FirstOrDefault(property => property.device_mode_number == measurement.DeviceModeNumber);
            if (deviceMode == null)
                throw new NoElementFoundException("Nie odnaleziono trybu pracy o podanym numerze.");

            var measurementToAdd = new AddMeasurementAllData()
            {
                device_power_in_mode = deviceMode.device_power_in_mode,
                user_id = userID,
                user_device_id = device.user_device_id,
                measurement_start_date = measurement.StartDate,
                measurement_end_date = measurement.EndDate,
                measurement_added_date = DateTime.Now
            };

            return RegisterMeasurement(measurementToAdd);
        }

        public void AddNewMeasurementWithSchedule(AddMeasurementWithScheduleDto measurement)
        {
            //method used to add new measurement with schedule

            //check if details about schedule are provided
            if (measurement.ScheduleFrequency <= 0 || measurement.ScheduleFrequency > 7)
                throw new WrongInputException("Jako czestotliwosc pomiaru prosze podac liczbe z zakresu od 1 do 7. (1 - pomiar powtarzany codziennie, 7 - raz w tygodniu)");

            int measurementID = AddNewMeasurement(measurement.Measurement);

            _context.schedule.Add(new Schedule
            {
                measurement_id = measurementID,
                schedule_frequency = measurement.ScheduleFrequency
            });
            _context.SaveChanges();
        }

        public List<MeasurementDto> GetAllMeasurements()
        {
            return _mapper.Map<List<MeasurementDto>>(GetMeasurementsFromDb());
        }

        public List<MeasurementDto> MeasurementsBetweenDates(DatesInterval dates)
        {
            //method used to return measurements which are between specified dates

            //check if both dates are provided
            if (dates.endDate == DateTime.MinValue || dates.startDate == DateTime.MinValue)
                throw new NotAllDataProvidedException("Prosze podac date poczatkowa i date koncowa.");

            //check if end date is not earlier than start date
            if (DateTime.Compare(dates.startDate, dates.endDate) > 0)
                throw new WrongInputException("Data zakonczenia pomiaru nie moze byc wczesniejsza od daty jego rozpoczecia.");
            
            //get measurements which are between specified dates
            var measurements = GetMeasurementsFromDb();
            measurements = measurements.Where(property => property.measurement_start_date >= dates.startDate && property.measurement_end_date <= dates.endDate).ToList();

            //check if any measurements are found
            if (measurements.Count == 0)
                throw new NoElementFoundException("Nie zarejestrowano zadnego pomiaru pomiedzy wskazanymi datami.");

            return _mapper.Map<List<MeasurementDto>>(measurements);
        }

        public List<MeasurementDto> MeasurementsForCategory(string category)
        {
            //method used to return measurements for specified category

            //check if category name is provided
            if (string.IsNullOrEmpty(category))
                throw new NotAllDataProvidedException("Prosze podac nazwe kategorii.");

            //get measurements for specified category
            var measurements = GetMeasurementsFromDb();
            measurements = measurements.Where(property => property.userDevice.device.device_category.device_category_name.ToLower() == category.ToLower()).ToList();

            //check if any measurements found
            if (measurements.Count == 0)
                throw new NoElementFoundException("Nie odnaleziono pomiarow dla wskazanej kategorii urzadzen.");

            return _mapper.Map<List<MeasurementDto>>(measurements);
        }

        private List<Measurement> GetMeasurementsFromDb()
        {
            //method used to return all measurements assigned for user who is logged in

            //get user id
            int userID = _userService.GetUserID();

            //get measurements for user who is logged in
            var measurements = _context.measurement.Where(property => property.user_id == userID)
                                                    .Include(d =>d.userDevice.device)
                                                    .Include(dc => dc.userDevice.device.device_category)
                                                    .ToList();

            //check if any measurements found
            if (measurements.Count == 0)
                throw new NoElementFoundException("Uzytkownik nie ma zarejestrowanego zadnego pomiaru.");

            return measurements;
        }

        private int RegisterMeasurement(AddMeasurementAllData measurement)
        {
            var user = _context.user.Include(tariff => tariff.electricity_tariff)
                                    .Include(tariffDetails => tariffDetails.electricity_tariff.tariff_details)
                                    .FirstOrDefault(property => property.user_id == measurement.user_id);
            if (user == null)
                throw new NoElementFoundException("Nie odnaleziono uzytkownika.");

            //calculate amount of energy used by the user in current year
            var userMeasurements = _context.measurement.Where(property => property.user_id == measurement.user_id).ToList();
            double consumptionInCurrentYear = userMeasurements.Where(property => property.measurement_start_date.Year == DateTime.Now.Year &&
                                                                  property.measurement_end_date.Year == DateTime.Now.Year)
                                                            .Sum(property => property.energy_used);

            //if user used more energy than his cheaper energy limit, method needs to use more expensive energy prices
            bool limitNotExceeded = true;
            if (consumptionInCurrentYear >= user.cheaper_energy_limit)
                limitNotExceeded = false;

            var userTariff = user.electricity_tariff;



            //get info about day from start date and end date
            var startDay = measurement.measurement_start_date.DayOfWeek;
            int startDayNumber;
            //if day number is 0 (sunday), change it to 7, to match data in database
            if (startDay == System.DayOfWeek.Sunday)
                startDayNumber = 7;
            else
                startDayNumber = (int)startDay;

            var endDay = measurement.measurement_end_date.DayOfWeek;
            int endDayNumber;
            if (endDay == System.DayOfWeek.Sunday)
                endDayNumber = 7;
            else
                endDayNumber = (int)endDay;

            var startTime = new TimeSpan(measurement.measurement_start_date.TimeOfDay.Hours, measurement.measurement_start_date.TimeOfDay.Minutes, measurement.measurement_start_date.TimeOfDay.Seconds);
            var endTime = new TimeSpan(measurement.measurement_end_date.TimeOfDay.Hours, measurement.measurement_end_date.TimeOfDay.Minutes, measurement.measurement_end_date.TimeOfDay.Seconds);

            #region variables used in loop
            bool finished = false;
            int currentDay = startDayNumber;
            TimeSpan startTimeForCurrentIteration = startTime;
            double energyUsed = 0;
            double totalPrice = 0;
            int iteration = 0;
            #endregion

            //loop to count price and energy used for this measurement
            while (finished == false)
            {
                var currentDetails = userTariff.tariff_details.FirstOrDefault(property => property.day_of_week_id == currentDay &&
                                                                                TimeSpan.Parse(property.start_time) <= startTimeForCurrentIteration &&
                                                                                TimeSpan.Parse(property.end_time) > startTimeForCurrentIteration);

                double amountOfHours = 0;

                //check if current day is equal to day when measurement ends
                if (currentDetails.day_of_week_id == endDayNumber)
                {
                    //check if measurement end time is earlier than end time for current tariff details end time
                    if (endTime <= TimeSpan.Parse(currentDetails.end_time))
                    {
                        //if first iteration, get amount of hours by subtracting measurement start time and end time
                        if (iteration == 0)
                        {
                            amountOfHours = endTime.Subtract(startTime).TotalHours;
                        }
                        else
                        {
                            //if not first iteration, get amount of hours by subtracting measurement end time and start time for current tariff details
                            amountOfHours = endTime.Subtract(TimeSpan.Parse(currentDetails.start_time)).TotalHours;
                        }
                        //last iteration, set boolean value to true to end loop
                        finished = true;
                    }
                    //if measurement end time is not earlier than end time for current tariff details end time
                    else
                    {
                        //if first iteration, get amount of hours by subtracting measurement start time and end time for current tariff details
                        if (iteration == 0)
                        {
                            amountOfHours = TimeSpan.Parse(currentDetails.end_time).Subtract(startTime).TotalHours;
                        }
                        else
                        {
                            //if not first iteration, get amount of hours by subtracting current tariff details end time and start time for current tariff details
                            amountOfHours = TimeSpan.Parse(currentDetails.end_time).Subtract(TimeSpan.Parse(currentDetails.start_time)).TotalHours;
                        }

                    }
                }
                //if current day is not equal to day when measurement ends
                else
                {
                    //if first iteration, get amount of hours by subtracting measurement start time and end time for current tariff details
                    if (iteration == 0)
                    {
                        amountOfHours = TimeSpan.Parse(currentDetails.end_time).Subtract(startTime).TotalHours;
                    }
                    else
                    {
                        //if not first iteration, get amount of hours by subtracting current tariff details end time and start time for current tariff details
                        amountOfHours = TimeSpan.Parse(currentDetails.end_time).Subtract(TimeSpan.Parse(currentDetails.start_time)).TotalHours;
                    }

                }

                //if user exceeded his cheaper enegy limit for current year, use more expensive price
                double pricePerKwh;
                if (limitNotExceeded)
                    pricePerKwh = currentDetails.price_per_kwh;
                else
                    pricePerKwh = currentDetails.price_per_kwh_after_limit;

                //add amount of energy used in current tariff details period to sum of energy used in earlier time periods
                energyUsed += amountOfHours * measurement.device_power_in_mode;

                //add price of energy used in current tariff details period to sum of prices for earlier periods
                totalPrice += (amountOfHours * measurement.device_power_in_mode) / 1000 * pricePerKwh;

                //if end time in current tariff details indicates end of the day, increment number of day and set start time for next time period to midnight
                if (currentDetails.end_time == "23:59:59")
                {
                    if (currentDay == 7)
                        currentDay = 1;
                    else
                        currentDay++;

                    startTimeForCurrentIteration = TimeSpan.Parse("00:00:00");
                }
                //otherwise set start time for next time period as a end time for current time period and add 1 second to it to match values stored in database
                else
                {
                    startTimeForCurrentIteration = TimeSpan.Parse(currentDetails.end_time).Add(TimeSpan.Parse("00:00:01"));
                }

                iteration++;
            }

            //change Wh to KWh
            energyUsed /= 1000;

            //save data to db
            var measurementToAdd = new Measurement()
            {
                energy_used = energyUsed,
                measurement_start_date = measurement.measurement_start_date,
                measurement_end_date = measurement.measurement_end_date,
                measurement_added_date = measurement.measurement_added_date,
                user_id = measurement.user_id,
                user_device_id = measurement.user_device_id,
                price_of_used_energy = totalPrice
            };
            _context.measurement.Add(measurementToAdd);
            _context.SaveChanges();

            return measurementToAdd.measurement_id;
        }
    }
}
