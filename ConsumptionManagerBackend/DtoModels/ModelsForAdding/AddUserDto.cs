﻿using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels.ModelsForAdding
{
    public class AddUserDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserSurname { get; set; }
        [Required]
        public int UserCredentialsId { get; set; }

        [Required]
        public string EnergySupplierName { get; set; }
        [Required]
        public string ElectricityTariffName { get; set; }

        [Required]
        public int CheaperEnergyLimit { get; set; }
    }
}
