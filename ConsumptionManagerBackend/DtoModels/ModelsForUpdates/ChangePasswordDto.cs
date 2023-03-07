namespace ConsumptionManagerBackend.DtoModels.ModelsForUpdates
{
    public class ChangePasswordDto
    {
        public string UserEmail { get; set; }
        public string UserOldPassword { get; set; }
        public string UserNewPassword { get; set;}
    }
}
