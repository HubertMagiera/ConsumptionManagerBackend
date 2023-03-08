using AutoMapper;
using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForUpdates;
using ConsumptionManagerBackend.Exceptions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ConsumptionManagerBackend.Services
{
    public class UserService : IUserService
    {
        private readonly EnergySaverDbContext _context;
        private readonly IPasswordHasher<UserCredentials> _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UserService(EnergySaverDbContext context,IPasswordHasher<UserCredentials> passwordHasher,IMapper mapper, ITokenService tokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _tokenService = tokenService;
        }
        public void AddUserData(AddUserDto addUser)
        {
            //method used when registering new account
            //it adds user info like name, surname, tariff id and credentials id
            //in case user wants to change his tariff, another method needs to be used

            //validate if user provided all of required data
            if (addUser.ElectricityTariffId == null || addUser.UserCredentialsId == null ||
                addUser.UserName == null || addUser.UserSurname == null)
                throw new NotAllDataProvidedException("Prosze podac wszystkie wymagane dane");

            //if all data is provided, check if tariff id exists in db (TO BE ADDED)
            //CHECK IF CREDENTIALS ARE ALREADY IN USE (TO BE ADDED)
            //change database, add autoincrement to all primary keys


            //check if provided credentials id exists in database
            var credentials = _context.user_credentials.FirstOrDefault(creds => creds.user_credentials_id == addUser.UserCredentialsId);
            if (credentials == null)
                throw new UserNotFoundException("Prosze sprawdzic poprawnosc podanych danych");

            //if all is good, add user to db
            var userToBeAdded = _mapper.Map<User>(addUser);
            _context.user.Add(userToBeAdded);
            _context.SaveChanges();

        }

        public TokenModel LoginUser(UserCredentialsDto userCredentials)
        {
            //check if provided email is in db
            var creds = _context.user_credentials.FirstOrDefault(user => user.user_email == userCredentials.UserEmail);
            if (creds == null)
                throw new WrongCredentialsException("Prosze sprawdzic poprawnosc podanych danych logowania.");

            //check if provided password is the same as the one in db
            var validationResult = _passwordHasher.VerifyHashedPassword(creds, creds.user_password, userCredentials.UserPassword);
            if(validationResult != PasswordVerificationResult.Success)
                throw new WrongCredentialsException("Prosze sprawdzic poprawnosc podanych danych logowania.");

            //if provided credentials are ok, login user by creating tokens
            string accessToken = _tokenService.CreateToken(creds);
            string refreshToken = _tokenService.CreateRefreshToken();

            creds.refresh_token = refreshToken;//replace old refresh token with a new one
            _context.SaveChanges();

            return new TokenModel 
            { 
                AccessToken = accessToken, 
                RefreshToken = refreshToken 
            };
        }

        public int RegisterUser(UserCredentialsDto userCredentials)
        {
            bool emailFormat = validateEmailFormat(userCredentials.UserEmail);
            if (emailFormat == false)
                throw new IncorrectEmailException("Podany adres email ma niepoprawny format.");

            bool passwordMeetsRules = validatePasswordMeetsRules(userCredentials.UserPassword);
            if (passwordMeetsRules == false)
                throw new PasswordDoesNotMeetRulesException("Podane haslo nie spelnia wymogow bezpieczenstwa.");

            var user = _context.user_credentials.FirstOrDefault(user => user.user_email == userCredentials.UserEmail);
            if (user != null)
                throw new EmailNotUniqueException("Podany adres email jest juz przypisany do innego konta. Prosze podac inny adres email.");


            var credentialsToBeAdded = _mapper.Map<UserCredentials>(userCredentials);
            credentialsToBeAdded.user_password = _passwordHasher.HashPassword(credentialsToBeAdded,userCredentials.UserPassword);
            credentialsToBeAdded.refresh_token = _tokenService.CreateRefreshToken();

            _context.Add(credentialsToBeAdded);
            _context.SaveChanges();

            return credentialsToBeAdded.user_credentials_id;
            
        }
        public void ChangePassword(ChangePasswordDto credentials)
        {
            bool passwordMeetsRules = validatePasswordMeetsRules(credentials.UserNewPassword);
            if (passwordMeetsRules == false)
                throw new PasswordDoesNotMeetRulesException("Nowe haslo nie spelnia wymogow bezpieczenstwa.");
            var userCredentials = _context.user_credentials.FirstOrDefault(user => user.user_email == credentials.UserEmail);
            if (userCredentials == null)
                throw new WrongCredentialsException("Prosze sprawdzic poprawnosc podanych danych logowania.");

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(userCredentials, userCredentials.user_password, credentials.UserOldPassword);
            if(passwordVerificationResult != PasswordVerificationResult.Success)
                throw new WrongCredentialsException("Prosze sprawdzic poprawnosc podanych danych logowania.");

            userCredentials.user_password = _passwordHasher.HashPassword(userCredentials, credentials.UserNewPassword);
            _context.SaveChanges();

        }

        public TokenModel RefreshSession(TokenModel model)
        {
            ClaimsPrincipal principal = _tokenService.GetPrincipalFromOldToken(model.AccessToken);
            int userId = Convert.ToInt32(principal.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = _context.user.FirstOrDefault(userFromDb => userFromDb.user_id == userId);
            if (user == null)
                throw new UserNotFoundException("Nie odnaleziono uzytkownika, na ktory wskazuje podany token");

            //check if provided refresh token and refresh token from db are the same
            var userCredentials = _context.user_credentials.FirstOrDefault(cred => cred.user_credentials_id == user.user_credentials_id);
            if (userCredentials.refresh_token != model.RefreshToken)
                throw new RefreshTokenNotValidException("Podany refresh token jest nieodpowiedni");

            //if everything is okay, create new tokens
            string accessToken = _tokenService.CreateToken(userCredentials);
            string refreshToken = _tokenService.CreateRefreshToken();

            userCredentials.refresh_token = refreshToken;//replace refresh tokens in db
            _context.SaveChanges();
            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
        };

        }

        private bool validatePasswordMeetsRules(string password)
        {
            //password needs to be:
            //minimum 8 characters long
            //contain at least one upper case letter
            //contain at least one lower case letter
            //contain at least on digit
            //can not contain whitespaces
            //must contain one of the special characters

            if (password.Length < 8 || !password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit) || password.Any(char.IsWhiteSpace))
                return false;

            string specialCharacters = "!@#$%^&*+-/?<>;~`[]{}:,.|=_";//special characters. At least one of them needs to be present in provided password
            char[] specialCharactersArray = specialCharacters.ToCharArray();
            foreach (char ch in password)
            {
                if (specialCharactersArray.Contains(ch))//if char from password is visible in special characters list, return true
                    return true;
            }
            return false;
        }

        private bool validateEmailFormat(string emailAddress)
        {
            //this method validates if provided email address has correct format
            Regex pattern = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
            bool result = pattern.IsMatch(emailAddress);
            return result;
        }

        
    }
}
