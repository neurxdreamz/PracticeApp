using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Text;

namespace Data_Logic.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string connectionString;

        public UserRepository()
        {
            connectionString = DataBaseConfig.GetConnectionString();
        }
        /// <summary>
        /// Получение всех пользователей
        /// </summary>
        public IEnumerable<User> GetAllUsers()
        {
            var users = new List<User>();

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT id_user, id_role, username FROM users";

                   using (var command = new OleDbCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(MapUserFromReader(reader));
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при получении списка пользователей из БД.", ex);
            }

            return users;
        }

        /// <summary>
        /// Получение пользователя по логину
        /// </summary>
        public User GetUserByUsername (string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username), "Логин не может быть пустым!");
            }
            try
            {
                using (var connection = OleDbConnection(connectionString))
                {
                    connection.Open();
                    return ExecuteUserSelectQuery(connection, username);
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception($"Ошибка при поиске пользователя: {username}", ex);
            }
        }

       /// <summary>
       /// Добавление нового пользователя пароля
       /// </summary>
       public void AddUser (User user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentNullException(passwordHash);
            }

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    InsertIntoUsersTable(connection, user);

                    
                    int newUserId = GetLastInsertedId(connection);

                   
                    InsertIntoPasswordsTable(connection, newUserId, passwordHash);
                }
            }
            
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при добавлении нового пользователя", ex);
            }
        }


        private User MapUserFromReader(OleDbDataReader reader)
        {

        }

        private User ExecuteUserSelectQuery(OleDbConnection connection, string username)
        {

        }
    }
}
