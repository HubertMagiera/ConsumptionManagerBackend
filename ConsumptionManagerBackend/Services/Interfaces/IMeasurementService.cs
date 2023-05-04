using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;

namespace ConsumptionManagerBackend.Services.Interfaces
{
    public interface IMeasurementService
    {
        int AddNewMeasurement(AddMeasurementDto measurement);

        List<MeasurementDto> GetAllMeasurements();

        List<MeasurementDto> MeasurementsForCategory(string category);

        void AddNewMeasurementWithSchedule(AddMeasurementWithScheduleDto measurement);

        List<MeasurementDto> MeasurementsBetweenDates(DatesInterval dates);

        void AddMeasurementsBasedOnSchedule(int userID);
    }
}
