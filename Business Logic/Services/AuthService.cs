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
    }
    
}
