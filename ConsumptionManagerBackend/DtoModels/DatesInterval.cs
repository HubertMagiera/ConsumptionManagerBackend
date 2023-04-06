using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels
{
    public class DatesInterval
    {
        [Required]
        public DateTime startDate { get; set; }

        [Required]
        public DateTime endDate { get; set; }
    }
}
