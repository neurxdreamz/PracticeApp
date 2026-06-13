using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;

namespace Data_Logic.Repositories
{
    public class WorkerRepository : IWorkerRepository
    {
        private readonly string connectionString;

        public WorkerRepository()
        {
            connectionString = DataBaseConfig.GetConnectionString();
        }

        /// <summary>
        /// Получение всех рабочих
        /// </summary>
        public IEnumerable<Worker> GetAllWorkers()
        {
            var workers = new List<Worker>();

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID_Рабочего, ФИО_рабочего, Специальность, Разряд, Тарифная_ставка FROM Рабочий";

                    using (var command = new OleDbCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            workers.Add(MapWorkerFromReader(reader));
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при получении списка рабочих из БД.", ex);
            }

            return workers;
        }

        /// <summary>
        /// Получение рабочего по ID
        /// </summary>
        public Worker GetWorkerById(int id)
        {
            if (id <= 0) throw new ArgumentException("Некорректный ID рабочего.", nameof(id));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID_Рабочего, ФИО_рабочего, Специальность, Разряд, Тарифная_ставка FROM Рабочий WHERE ID_Рабочего = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapWorkerFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception($"Ошибка при получении рабочего с ID {id}.", ex);
            }

            return null;
        }

        /// <summary>
        /// Добавление рабочего
        /// </summary>
        public void AddWorker(Worker worker)
        {
            if (worker == null) throw new ArgumentNullException(nameof(worker));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Рабочий (ФИО_рабочего, Специальность, Разряд, Тарифная_ставка) VALUES (@fullName, @specialty, @grade, @tariff)";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@fullName", worker.FullName);
                        command.Parameters.AddWithValue("@specialty", worker.Specialty ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@grade", worker.Grade);
                        command.Parameters.AddWithValue("@tariff", worker.TariffRate);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при добавлении нового рабочего.", ex);
            }
        }
        
        /// <summary>
        /// Обновление данных рабочего
        /// </summary>
        public void UpdateWorker(Worker worker)
        {
            if (worker == null) throw new ArgumentNullException(nameof(worker));
            if (worker.IdWorker <= 0) throw new ArgumentException("Невозможно обновить рабочего без корректного ID.");

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    // Важно: порядок параметров в запросе должен строго совпадать с порядком их добавления в command
                    string query = "UPDATE Рабочий SET ФИО_рабочего = @fullName, Специальность = @specialty, Разряд = @grade, Тарифная_ставка = @tariff WHERE ID_Рабочего = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@fullName", worker.FullName);
                        command.Parameters.AddWithValue("@specialty", worker.Specialty ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@grade", worker.Grade);
                        command.Parameters.AddWithValue("@tariff", worker.TariffRate);
                        command.Parameters.AddWithValue("@id", worker.IdWorker);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при обновлении данных рабочего.", ex);
            }
        }

        /// <summary>
        /// Удаление рабочего
        /// </summary>
        public void DeleteWorker(int id)
        {
            if (id <= 0) throw new ArgumentException("Некорректный ID рабочего.", nameof(id));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Рабочий WHERE ID_Рабочего = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при удалении рабочего.", ex);
            }
        }


        private Worker MapWorkerFromReader(OleDbDataReader reader)
        {
            return new Worker
            {
                IdWorker = Convert.ToInt32(reader["ID_Рабочего"]),
                FullName = reader["ФИО_рабочего"].ToString(),
                Specialty = reader["Специальность"] != DBNull.Value ? reader["Специальность"].ToString() : string.Empty,
                Grade = reader["Разряд"] != DBNull.Value ? Convert.ToInt32(reader["Разряд"]) : 0,
                TariffRate = reader["Тарифная_ставка"] != DBNull.Value ? Convert.ToDecimal(reader["Тарифная_ставка"]) : 0m
            };
        }
    }

}
