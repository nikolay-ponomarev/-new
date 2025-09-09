using ConsoleApp2.Core.DataAccess;
using ConsoleApp2.Core.Entities;

namespace ConsoleApp2.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            var existingUser = _userRepository.GetUserByTelegramUserId(telegramUserId);
            if (existingUser != null)
            {
                return existingUser;
            }

            var newUser = new ToDoUser(telegramUserId, telegramUserName);
            _userRepository.Add(newUser);
            return newUser;
        }

        public ToDoUser? GetUser(long telegramUserId)
        {
            return _userRepository.GetUserByTelegramUserId(telegramUserId);
        }
    }
}