using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConsumptionManagerBackend.Services
{
    public class TokenService : ITokenService
    {
        private readonly int TokenValidTime = 15;//generated token is valid for 15 minutes
        private readonly int RefreshTokenValidTime = 10080;//refresh token is valid for one week
        private readonly AuthenticationSettings _settings;
        private readonly EnergySaverDbContext _context;

        public TokenService(AuthenticationSettings settings, EnergySaverDbContext context)
        {
            _settings = settings;
            _context = context;
        }

        public string CreateToken(UserCredentials cred)
        {
            //method used to create a new access token for a user
            //token is valid for 15 minutes
            //token has assigned a list of claims which identify its owner

            //user found here will be used to create claims to identify him
            var user = _context.user.FirstOrDefault(userFromDB => userFromDB.user_credentials_id == cred.user_credentials_id);
                
            var claims = new List<Claim>()//claims represent info about the user
            {
                new Claim(ClaimTypes.NameIdentifier,user.user_id.ToString()),
                new Claim(ClaimTypes.Name,user.user_name),
                new Claim(ClaimTypes.Surname,user.user_surname),
                new Claim(ClaimTypes.Email,cred.user_email)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer: _settings.Issuer, audience: _settings.Audience, claims: claims,
                expires: DateTime.Now.AddMinutes(TokenValidTime), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public string CreateRefreshToken()
        {
            //method used to create a refresh token which can be used to generate a new access token
            //this token is valid for 7 days and is stored in database under its owner
            //it does not contain list of claims
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.KeyForRefreshToken));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer: _settings.Issuer, audience: _settings.Audience,
                expires: DateTime.Now.AddMinutes(RefreshTokenValidTime), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public ClaimsPrincipal GetPrincipalFromOldToken(string oldToken)
        {
            //method used to get info about the user from the expired access token
            //this info is later used to find correct user in database and to compare provided refresh token with the one from database
            var validationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _settings.Issuer,
                ValidAudience = _settings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(oldToken, validationParameters, out SecurityToken validatedToken);

            return principal;


        }
    }
}
