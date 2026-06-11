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
    }
}
