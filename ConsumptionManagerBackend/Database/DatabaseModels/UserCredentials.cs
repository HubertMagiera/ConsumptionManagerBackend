﻿using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class UserCredentials
    {
        public int user_credentials_id { get; set; }

        public string user_email { get; set; }

        public string user_password { get; set; }

        public string refresh_token { get; set; }

        public User user { get; set; }
    }
}
