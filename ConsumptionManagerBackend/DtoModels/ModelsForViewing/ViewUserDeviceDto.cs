namespace ConsumptionManagerBackend.DtoModels.ModelsForViewing
{
    public class ViewUserDeviceDto
    {
        public string DeviceName { get; set; }

        public int DeviceMaxPower { get; set; }

        public string DeviceCategory { get; set; }

        public bool IsActive { get; set; }

        public List<ViewUserDeviceDetailsDto> Details { get; set; }
    }
}
