using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class UserService : IUserService
    {
        private readonly Dictionary<long, ToDoUser> _users = new Dictionary<long, ToDoUser>();

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            if (_users.TryGetValue(telegramUserId, out var existingUser))
            {
                return existingUser;
            }

            var newUser = new ToDoUser(telegramUserId, telegramUserName);
            _users[telegramUserId] = newUser;
            return newUser;
        }

        public ToDoUser? GetUser(long telegramUserId)
        {
            _users.TryGetValue(telegramUserId, out var user);
            return user;
        }
    }
}
