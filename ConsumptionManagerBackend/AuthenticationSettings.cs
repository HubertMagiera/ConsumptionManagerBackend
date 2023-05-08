namespace ConsumptionManagerBackend
{
    public class AuthenticationSettings
    {
        //class used to store an info about the definition of a good access token
        //this class has only one instance which is created in Program.cs by binding values from appsettings.json file to properties of this class
        public string Key { get; set; }

        public string KeyForRefreshToken { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
    }
}
