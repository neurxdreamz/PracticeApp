using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Logic.Repositories
{
    public interface IUserRepository
    {
        User GetUserByUsername(string username);
        IEnumerable<User> GetAllUsers();
        void AddUser(User user, string passwordHash);
        void DeleteUser(int userId);
    }
}
