using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;

namespace ConsumptionManagerBackend.Services.Interfaces
{
    public interface IMeasurementService
    {
        void AddNewMeasurement(AddMeasurementDto measurement);

        List<MeasurementDto> GetAllMeasurements();

        List<MeasurementDto> MeasurementsForCategory(string category);

        void AddNewMeasurementWithSchedule(AddMeasurementWithScheduleDto measurement);

        List<MeasurementDto> MeasurementsBetweenDates(DateTime startDate, DateTime endDate);
    }
}
