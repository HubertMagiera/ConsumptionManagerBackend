using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels.ModelsForUpdates
{
    public class ChangePasswordDto
    {
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string UserOldPassword { get; set; }
        [Required]
        public string UserNewPassword { get; set;}
    }
}
