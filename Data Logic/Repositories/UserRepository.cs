using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Text;
using System.Threading.Tasks.Sources;

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
                using (var connection = new OleDbConnection(connectionString))
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

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        public void DeleteUser (int UserId)
        {
            if (UserId <= 0)
            {
                throw new ArgumentException("Некорректнй ID пользователя", nameof(UserId));
            }

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM users WHERE id_user = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", UserId);
                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при удалении пользователя", ex);
            }
        }


        private User ExecuteUserSelectQuery(OleDbConnection connection, string username)
        {
            string query = "SELECT id_user, id_role, username FROM users WHERE username = @username";

            using (var command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapUserFromReader(reader);
                    }
                }
            }
            return null;
        }

        private User MapUserFromReader(OleDbDataReader reader)
        {

        }
    }
}
