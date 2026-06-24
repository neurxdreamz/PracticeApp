using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Logic.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();
        User GetUserByUsername(string username);
        void AddUser(User user, string passwordHash); 
        void UpdateUser(User user);                   
        void DeleteUser(int id);
    }
}
