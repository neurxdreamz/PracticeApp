using Business_Logic.Security;
using Data_Logic.Entities;
using Data_Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository UserRepository;

        public AuthService (IUserRepository userRepository)
        {
            if (userRepository == null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }
            UserRepository = userRepository;
        }

        public void RegisterNewUser (string username, int RoleId, string RawPassword)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Логин не может быть пустым!", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(RawPassword) || RawPassword.Length < 4)
            {
                throw new ArgumentException("Пароль должен содержать минимум 4 символа!", nameof(RawPassword));
            }
            
            if (IsUserExist(username))
            {
                throw new InvalidOperationException("Пользователь с таким логином уже существует!");
            }

            string hashedPassword = PasswordHasher.HashPassword(RawPassword);

            var newUser = new User
            {
                Username = username,
                IdRole = RoleId
            };

            UserRepository.AddUser(newUser, hashedPassword);
        }

        public bool IsUserExist (string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            var user = UserRepository.GetUserByUsername(username);
            return user != null;
        }

        public User AuthenticateUser(string username, string rawPassword)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(rawPassword))
                throw new ArgumentException("Логин и пароль не могут быть пустыми.");

            var user = UserRepository.GetUserByUsername(username);

            if (user == null)
                throw new InvalidOperationException("Пользователь с таким логином не найден.");

         
            string inputHash = PasswordHasher.HashPassword(rawPassword);

           
            if (user.PasswordHash != inputHash)
                throw new InvalidOperationException("Неверный пароль.");

            return user;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return UserRepository.GetAllUsers();
        }

        public void DeleteUser(int id)
        {
            UserRepository.DeleteUser(id);
        }

        public void UpdateUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            
            var allUsers = UserRepository.GetAllUsers();
            User oldUser = null;

            foreach (var u in allUsers)
            {
                if (u.IdUser == user.IdUser)
                {
                    oldUser = u;
                    break;
                }
            }

        
            if (oldUser != null && user.PasswordHash != oldUser.PasswordHash)
            {
                if (string.IsNullOrWhiteSpace(user.PasswordHash) || user.PasswordHash.Length < 4)
                {
                    throw new ArgumentException("Новый пароль должен содержать минимум 4 символа!");
                }

                user.PasswordHash = PasswordHasher.HashPassword(user.PasswordHash);
            }

            UserRepository.UpdateUser(user);
        }
    }
    
}
