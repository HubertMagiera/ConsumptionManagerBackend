namespace ConsumptionManagerBackend
{
    public class AuthenticationSettings
    {
        public string Key { get; set; }

        public string KeyForRefreshToken { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
    }
}
