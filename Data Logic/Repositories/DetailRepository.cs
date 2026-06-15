using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;

namespace Data_Logic.Repositories
{
    public class DetailRepository : IDetailRepository
    {
        private readonly string connectionString;

        public DetailRepository()
        {
            connectionString = DataBaseConfig.GetConnectionString();
        }
    

    /// <summary>
    /// Получение всех деталей
    /// </summary>
    public IEnumerable<Detail> GetAllDetails()
        {
            var details = new List<Detail>();

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            d.ID_Записи, d.Название_детали, d.Объём_партии, 
                            d.Участок, d.Рабочий, d.Смена,
                            s.Участок AS Название_Участка, 
                            w.ФИО_рабочего, 
                            sh.[№_Смены]
                        FROM (((Деталь d
                        LEFT JOIN Участок s ON d.Участок = s.ID_Участка)
                        LEFT JOIN Рабочий w ON d.Рабочий = w.ID_Рабочего)
                        LEFT JOIN Смена sh ON d.Смена = sh.ID_Смены)";

                    using (var command = new OleDbCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            details.Add(MapDetailFromReader(reader));
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при получении списка деталей из БД.", ex);
            }

            return details;
        }

        /// <summary>
        /// Получение детали по id
        /// </summary>
        public Detail GetDetailById(int id)
        {
            if (id <= 0) throw new ArgumentException("Некорректный ID детали.", nameof(id));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            d.ID_Записи, d.Название_детали, d.Объём_партии, 
                            d.Участок, d.Рабочий, d.Смена,
                            s.Участок AS Название_Участка, 
                            w.ФИО_рабочего, 
                            sh.[№_Смены]
                        FROM (((Деталь d
                        LEFT JOIN Участок s ON d.Участок = s.ID_Участка)
                        LEFT JOIN Рабочий w ON d.Рабочий = w.ID_Рабочего)
                        LEFT JOIN Смена sh ON d.Смена = sh.ID_Смены)
                        WHERE d.ID_Записи = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapDetailFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception($"Ошибка при получении детали с ID {id}.", ex);
            }

            return null;
        }

        ///<summary>
        ///Добавление детали
        /// </summary>
        public void AddDetail(Detail detail)
        {
            if (detail == null) throw new ArgumentNullException(nameof(detail));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Деталь (Название_детали, Объём_партии, Участок, Рабочий, Смена) VALUES (@name, @volume, @sector, @worker, @shift)";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", detail.DetailName);
                        command.Parameters.AddWithValue("@volume", detail.BatchVolume);
                        command.Parameters.AddWithValue("@sector", detail.SectorId);
                        command.Parameters.AddWithValue("@worker", detail.WorkerId);
                        command.Parameters.AddWithValue("@shift", detail.ShiftId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при добавлении новой детали.", ex);
            }
        }

        /// <summary>
        /// Обновление данных детали
        ///</summary>
        public void UpdateDetail(Detail detail)
        {
            if (detail == null) throw new ArgumentNullException(nameof(detail));
            if (detail.IdRecord <= 0) throw new ArgumentException("Невозможно обновить деталь без корректного ID.");

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    // Напоминаю: параметры в OleDb строго по порядку их появления в строке SQL
                    string query = "UPDATE Деталь SET Название_детали = @name, Объём_партии = @volume, Участок = @sector, Рабочий = @worker, Смена = @shift WHERE ID_Записи = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", detail.DetailName);
                        command.Parameters.AddWithValue("@volume", detail.BatchVolume);
                        command.Parameters.AddWithValue("@sector", detail.SectorId);
                        command.Parameters.AddWithValue("@worker", detail.WorkerId);
                        command.Parameters.AddWithValue("@shift", detail.ShiftId);
                        command.Parameters.AddWithValue("@id", detail.IdRecord);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при обновлении данных детали.", ex);
            }
        }

        /// <summary>
        /// Обновление данных детали
        /// </summary>
        public void DeleteDetail(int id)
        {
            if (id <= 0) throw new ArgumentException("Некорректный ID детали.", nameof(id));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Деталь WHERE ID_Записи = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при удалении детали.", ex);
            }
        }

        // ==========================================
        // ПРИВАТНЫЕ МЕТОДЫ (МАППИНГ)
        // ==========================================
        private Detail MapDetailFromReader(OleDbDataReader reader)
        {
            var detail = new Detail
            {
                IdRecord = Convert.ToInt32(reader["ID_Записи"]),
                DetailName = reader["Название_детали"].ToString(),
                BatchVolume = reader["Объём_партии"] != DBNull.Value ? Convert.ToInt32(reader["Объём_партии"]) : 0,

                SectorId = reader["Участок"] != DBNull.Value ? Convert.ToInt32(reader["Участок"]) : 0,
                WorkerId = reader["Рабочий"] != DBNull.Value ? Convert.ToInt32(reader["Рабочий"]) : 0,
                ShiftId = reader["Смена"] != DBNull.Value ? Convert.ToInt32(reader["Смена"]) : 0
            };


            if (HasColumn(reader, "Название_Участка"))
                detail.SectorName = reader["Название_Участка"] != DBNull.Value ? reader["Название_Участка"].ToString() : string.Empty;

            if (HasColumn(reader, "ФИО_рабочего"))
                detail.WorkerFullName = reader["ФИО_рабочего"] != DBNull.Value ? reader["ФИО_рабочего"].ToString() : string.Empty;

            if (HasColumn(reader, "№_Смены"))
                detail.ShiftNumber = reader["№_Смены"] != DBNull.Value ? Convert.ToInt32(reader["№_Смены"]) : 0;

            return detail;
        }

        private bool HasColumn(OleDbDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
    

