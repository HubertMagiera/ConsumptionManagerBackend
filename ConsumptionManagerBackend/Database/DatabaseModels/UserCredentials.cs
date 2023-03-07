using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class UserCredentials
    {
        [Required]
        public int user_credentials_id { get; set; }

        [Required]
        public string user_email { get; set; }

        [Required]
        public string user_password { get; set; }
    }
}
