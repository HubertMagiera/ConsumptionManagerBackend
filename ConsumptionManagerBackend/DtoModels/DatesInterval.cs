using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels
{
    public class DatesInterval
    {
        //class used when searching for the measurements between dates
        [Required]
        public DateTime startDate { get; set; }

        [Required]
        public DateTime endDate { get; set; }
    }
}
