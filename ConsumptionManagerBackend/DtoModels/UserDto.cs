using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels
{
    public class UserDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserSurname { get; set; }

        public UserCredentialsDto UserCredentials { get; set; }
    }
}
