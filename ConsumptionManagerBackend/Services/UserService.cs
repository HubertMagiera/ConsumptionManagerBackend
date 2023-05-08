using AutoMapper;
using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForAdding;
using ConsumptionManagerBackend.DtoModels.ModelsForUpdates;
using ConsumptionManagerBackend.Exceptions;
using ConsumptionManagerBackend.Services.Interfaces;
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
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService(EnergySaverDbContext context, IPasswordHasher<UserCredentials> passwordHasher, IMapper mapper, ITokenService tokenService, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _tokenService = tokenService;
            _contextAccessor = contextAccessor;
        }
        public TokenModel AddUserData(AddUserDto addUser)
        {
            //method used when registering new account
            //it adds user info like name, surname, tariff id and credentials id
            //in case user wants to change his tariff, another method needs to be used

            //validate if user provided all of required data
            if (string.IsNullOrEmpty(addUser.EnergySupplierName) || string.IsNullOrEmpty(addUser.ElectricityTariffName) || addUser.UserCredentialsId <= 0 ||
                string.IsNullOrEmpty(addUser.UserName) || string.IsNullOrEmpty(addUser.UserSurname))
                throw new NotAllDataProvidedException("Prosze podac wszystkie wymagane dane.");

            //check if provided chepaer energy limit = 2000, 2600 or 3000 kwh
            if (addUser.CheaperEnergyLimit != 2000 && addUser.CheaperEnergyLimit != 2600 && addUser.CheaperEnergyLimit != 3000)
                throw new WrongInputException("Prosze wybrac odpowiedni limit tanszej energii elektrycznej. 2000, 2600 lub 3000 KWh");

            //if all data is provided, check if tariff for provided data exists in db
            var electricityTariff = _context.electricity_tariff.FirstOrDefault(tariff => tariff.tariff_name.ToLower() == addUser.ElectricityTariffName.ToLower()
                                                                                && tariff.energy_supplier.energy_supplier_name.ToLower() == addUser.EnergySupplierName.ToLower());
            if (electricityTariff == null)
                throw new WrongInputException("Podana taryfa nie istnieje, prosze sprawdzic poprawnosc podanych danych.");

            //check if provided credentials id is created
            var credentials = _context.user_credentials.FirstOrDefault(creds => creds.user_credentials_id == addUser.UserCredentialsId);
            if (credentials == null)
                throw new UserNotFoundException("Prosze sprawdzic poprawnosc podanych danych.");

            //check if provided credentials id is already assigned to any user
            var userWithTheSameCredentials = _context.user.FirstOrDefault(credsId => credsId.user_credentials_id == addUser.UserCredentialsId);
            if (userWithTheSameCredentials != null)
                throw new CredentialsAlreadyInUseException("Prosze sprawdzic poprawnosc podanych danych.");

            //if all is good, add user to database
            var userToBeAdded = _mapper.Map<User>(addUser);
            userToBeAdded.electricity_tariff_id = electricityTariff.electricity_tariff_id;
            _context.user.Add(userToBeAdded);
            _context.SaveChanges();

            //after user is created, perform login and return tokens
            string accessToken = _tokenService.CreateToken(credentials);
            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = credentials.refresh_token
            };

        }

        public TokenModel LoginUser(UserCredentialsDto userCredentials)
        {
            //method used to login user
            //user needs to provide email address and password

            //check if provided email is in database
            var creds = _context.user_credentials.FirstOrDefault(user => user.user_email == userCredentials.UserEmail);
            if (creds == null)
                throw new WrongCredentialsException("Prosze sprawdzic poprawnosc podanych danych logowania.");

            //check if provided password is the same as the one in database
            var validationResult = _passwordHasher.VerifyHashedPassword(creds, creds.user_password, userCredentials.UserPassword);
            if (validationResult != PasswordVerificationResult.Success)
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
            //method used to create an entry in table "user credentials"
            //first step of creating new account. Second step is assigning info about the user with usage of method "AddUserData"

            //validate if provided email has correct format
            bool emailFormat = validateEmailFormat(userCredentials.UserEmail);
            if (emailFormat == false)
                throw new IncorrectEmailException("Podany adres email ma niepoprawny format.");

            //check if provided email address is already in use
            var user = _context.user_credentials.FirstOrDefault(user => user.user_email == userCredentials.UserEmail);
            if (user != null)
                throw new EmailNotUniqueException("Podany adres email jest juz przypisany do innego konta. Prosze podac inny adres email.");

            //validate if password meets rules
            bool passwordMeetsRules = validatePasswordMeetsRules(userCredentials.UserPassword);
            if (passwordMeetsRules == false)
                throw new PasswordDoesNotMeetRulesException("Podane haslo nie spelnia wymogow bezpieczenstwa.");

            //map provided data transfer object to the same type as database entity
            var credentialsToBeAdded = _mapper.Map<UserCredentials>(userCredentials);
            //hash provided password to safely store it in database
            credentialsToBeAdded.user_password = _passwordHasher.HashPassword(credentialsToBeAdded, userCredentials.UserPassword);
            credentialsToBeAdded.refresh_token = _tokenService.CreateRefreshToken();

            _context.Add(credentialsToBeAdded);
            _context.SaveChanges();

            return credentialsToBeAdded.user_credentials_id;

        }

        public void ChangeTariff(ChangeSupplierAndTariffDto tariff)
        {
            //method used to update info about energy supplier and tariff assigned to user account

            //check if all data is provided
            if (string.IsNullOrEmpty(tariff.TariffName) || string.IsNullOrEmpty(tariff.SupplierName))
                throw new NotAllDataProvidedException("Prosze podac wszystkie wymagane dane (nazwa dostawcy oraz nazwa taryfy).");

            //if yes, check if tariff for provided details exists in db
            var tariffFromDb = _context.electricity_tariff.FirstOrDefault(property => property.tariff_name.ToLower() == tariff.TariffName.ToLower() &&
                                                                            property.energy_supplier.energy_supplier_name.ToLower() == tariff.SupplierName.ToLower());
            if (tariffFromDb == null)
                throw new NoElementFoundException("Nie odnaleziono informacji o taryfach dla podanych danych. Prosze sprobowac ponownie.");
            //if found, get user id
            int userID = GetUserID();

            //get user from db and update info about tariff
            var user = _context.user.FirstOrDefault(property => property.user_id == userID);
            if (user == null)
                throw new NoElementFoundException("Nie odnaleziono informacji o uzytkowniku. Prosze zalogowac sie jeszcze raz i sprobowac ponownie.");
            user.electricity_tariff_id = tariffFromDb.electricity_tariff_id;

            _context.SaveChanges();
        }

        public void ChangePassword(ChangePasswordDto credentials)
        {
            //method used to change users password
            //user needs to provide his email, old password and a new one

            //check if provided new password meets requirements
            bool passwordMeetsRules = validatePasswordMeetsRules(credentials.UserNewPassword);
            if (passwordMeetsRules == false)
                throw new PasswordDoesNotMeetRulesException("Nowe haslo nie spelnia wymogow bezpieczenstwa.");

            //check if provided email address exists in db
            var userCredentials = _context.user_credentials.FirstOrDefault(user => user.user_email == credentials.UserEmail);
            if (userCredentials == null)
                throw new WrongCredentialsException("Prosze sprawdzic poprawnosc podanych danych logowania.");

            //check if provided old password and password from database are equal
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(userCredentials, userCredentials.user_password, credentials.UserOldPassword);
            if(passwordVerificationResult != PasswordVerificationResult.Success)
                throw new WrongCredentialsException("Prosze sprawdzic poprawnosc podanych danych logowania.");

            //update the password and store it as a result of hashing method
            userCredentials.user_password = _passwordHasher.HashPassword(userCredentials, credentials.UserNewPassword);
            _context.SaveChanges();

        }

        public TokenModel RefreshSession(TokenModel model)
        {
            //method used to refresh user session
            //as an input it is required to provide a old access token and a refresh token which is assigned to each user and stored in database

            //get user id from the old access token
            ClaimsPrincipal principal = _tokenService.GetPrincipalFromOldToken(model.AccessToken);
            int userId = Convert.ToInt32(principal.FindFirst(ClaimTypes.NameIdentifier).Value);

            //check if user with id read from old access token exists in database
            var user = _context.user.FirstOrDefault(userFromDb => userFromDb.user_id == userId);
            if (user == null)
                throw new UserNotFoundException("Nie odnaleziono uzytkownika, na ktory wskazuje podany token.");

            //check if provided refresh token and refresh token from database are the same
            var userCredentials = _context.user_credentials.FirstOrDefault(cred => cred.user_credentials_id == user.user_credentials_id);

            //if they are not equal, throw an error
            if (userCredentials.refresh_token != model.RefreshToken)
                throw new RefreshTokenNotValidException("Podany refresh token jest nieodpowiedni.");

            //if everything is okay, create new tokens
            string accessToken = _tokenService.CreateToken(userCredentials);
            string refreshToken = _tokenService.CreateRefreshToken();

            userCredentials.refresh_token = refreshToken;//replace refresh token in database
            _context.SaveChanges();
            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
        };

        }

        public int GetUserID()
        {
            //method used to gen an id of user who sent the request
            //works only if user is authenticated

            int id = Convert.ToInt32(_contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (id == 0)
                throw new NoElementFoundException("Nie znaleziono uzytkownika w bazie danych.");
            return id;
        }

        public ClaimsPrincipal GetUser()
        {
            //method used to get an info about the user who sent the request
            //works only if user is authenticated
            //returns claims assigned to access token

            var user = _contextAccessor.HttpContext.User;
            if (user == null)
                throw new NoElementFoundException("Nie znaleziono uzytkownika w bazie danych.");
            return user;
        }

        private bool validatePasswordMeetsRules(string password)
        {
            //password needs to be:
            //minimum 8 characters long
            //contain at least one upper case letter
            //contain at least one lower case letter
            //contain at least one digit
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
