using AutoMapper;
using ConsumptionManagerBackend.Database;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.DtoModels;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace ConsumptionManagerBackend.Services
{
    public class UserService : IUserService
    {
        private readonly EnergySaverDbContext _context;
        private readonly IPasswordHasher<UserCredentials> _passwordHasher;
        private readonly IMapper _mapper;
        public UserService(EnergySaverDbContext context,IPasswordHasher<UserCredentials> passwordHasher,IMapper mapper)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }
        public void AddUserData(AddUserDto addUser)
        {
            throw new NotImplementedException();
        }

        public string LoginUser(UserCredentialsDto userCredentials)
        {
            throw new NotImplementedException();
        }

        public void RegisterUser(UserCredentialsDto userCredentials)
        {
            bool emailFormat = validateEmailFormat(userCredentials.UserEmail);
            if (emailFormat == false)
                throw new Exception();

            bool passwordMeetsRules = validatePasswordMeetsRules(userCredentials.UserPassword);
            if (passwordMeetsRules == false)
                throw new Exception();

            var user = _context.user_credentials.FirstOrDefault(user => user.user_email == userCredentials.UserEmail);
            if (user != null)
                throw new Exception();


            var credentialsToBeAdded = _mapper.Map<UserCredentials>(userCredentials);
            credentialsToBeAdded.user_password = _passwordHasher.HashPassword(credentialsToBeAdded,userCredentials.UserPassword);

            _context.Add(credentialsToBeAdded);
            _context.SaveChanges();
            
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

            if (password.Length < 8 || !password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit) || !password.Any(char.IsWhiteSpace))
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
