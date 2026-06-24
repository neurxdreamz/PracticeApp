using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;

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

                    // ИСПРАВЛЕНО: Добавлен JOIN, чтобы вытягивать пароли для формы редактирования
                    string query = @"
                        SELECT u.id_user, u.id_role, u.username, p.password_hash 
                        FROM users u
                        LEFT JOIN passwords p ON u.id_user = p.id_user";

                    using (var command = new OleDbCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                IdUser = Convert.ToInt32(reader["id_user"]),
                                IdRole = Convert.ToInt32(reader["id_role"]),
                                Username = reader["username"].ToString(),
                                // Проверка на пустой пароль (на случай если в базе есть юзер без пароля)
                                PasswordHash = reader["password_hash"] != DBNull.Value ? reader["password_hash"].ToString() : ""
                            });
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
        public User GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username), "Логин не может быть пустым!");

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
        /// Добавление нового пользователя
        /// </summary>
        public void AddUser(User user, string passwordHash)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentNullException(nameof(passwordHash));

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
        /// НОВЫЙ МЕТОД: Обновление пользователя
        /// </summary>
        public void UpdateUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.IdUser <= 0) throw new ArgumentException("Некорректный ID");

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    // 1. Обновляем логин и роль
                    string queryUsers = "UPDATE users SET id_role = @role, username = @username WHERE id_user = @id";
                    using (var cmdUsers = new OleDbCommand(queryUsers, connection))
                    {
                        cmdUsers.Parameters.AddWithValue("@role", user.IdRole);
                        cmdUsers.Parameters.AddWithValue("@username", user.Username);
                        cmdUsers.Parameters.AddWithValue("@id", user.IdUser);
                        cmdUsers.ExecuteNonQuery();
                    }

                    // 2. Обновляем пароль
                    string queryPass = "UPDATE passwords SET password_hash = @hash WHERE id_user = @id";
                    using (var cmdPass = new OleDbCommand(queryPass, connection))
                    {
                        cmdPass.Parameters.AddWithValue("@hash", user.PasswordHash);
                        cmdPass.Parameters.AddWithValue("@id", user.IdUser);
                        cmdPass.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при обновлении пользователя", ex);
            }
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        public void DeleteUser(int UserId)
        {
            if (UserId <= 0) throw new ArgumentException("Некорректнй ID пользователя", nameof(UserId));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    // ИСПРАВЛЕНО: Сначала удаляем пароль (зависимая таблица), иначе Access не даст удалить юзера
                    string queryPass = "DELETE FROM passwords WHERE id_user = @id";
                    using (var cmdPass = new OleDbCommand(queryPass, connection))
                    {
                        cmdPass.Parameters.AddWithValue("@id", UserId);
                        cmdPass.ExecuteNonQuery();
                    }

                    // Затем удаляем самого пользователя
                    string queryUsers = "DELETE FROM users WHERE id_user = @id";
                    using (var cmdUsers = new OleDbCommand(queryUsers, connection))
                    {
                        cmdUsers.Parameters.AddWithValue("@id", UserId);
                        cmdUsers.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при удалении пользователя", ex);
            }
        }

        // --- Вспомогательные методы (оставлены без изменений) ---

        private User ExecuteUserSelectQuery(OleDbConnection connection, string username)
        {
            string query = @"
                SELECT u.id_user, u.id_role, u.username, p.password_hash 
                FROM users u
                INNER JOIN passwords p ON u.id_user = p.id_user
                WHERE u.username = @username";

            using (var command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            IdUser = Convert.ToInt32(reader["id_user"]),
                            IdRole = Convert.ToInt32(reader["id_role"]),
                            Username = reader["username"].ToString(),
                            PasswordHash = reader["password_hash"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        private void InsertIntoUsersTable(OleDbConnection connection, User user)
        {
            string query = "INSERT INTO users (id_role, username) VALUES (@role, @username)";
            using (var command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@role", user.IdRole);
                command.Parameters.AddWithValue("@username", user.Username);
                command.ExecuteNonQuery();
            }
        }

        private void InsertIntoPasswordsTable(OleDbConnection connection, int UserId, string PasswordHash)
        {
            string query = "INSERT INTO passwords (id_user, password_hash) VALUES (@id, @hash)";
            using (var command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", UserId);
                command.Parameters.AddWithValue("@hash", PasswordHash);
                command.ExecuteNonQuery();
            }
        }

        private int GetLastInsertedId(OleDbConnection connection)
        {
            string query = "SELECT @@IDENTITY";
            using (var command = new OleDbCommand(query, connection))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}