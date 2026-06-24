using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;

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

                    // ИСПРАВЛЕНО: Выбираем Название_участка, ФИО_рабочего, Номер_смены из соседних таблиц
                    string query = @"
                        SELECT 
                            d.ID_Записи, 
                            d.Название_детали, 
                            d.Объём_партии, 
                            d.Норма_времени, 
                            d.Дата_изготовления,
                            s.Участок, 
                            w.ФИО_рабочего, 
                            sh.№_Смены
                        FROM (((Деталь AS d
                        INNER JOIN Участок AS s ON d.Участок = s.ID_Участка)
                        INNER JOIN Рабочий AS w ON d.Рабочий = w.ID_Рабочего)
                        INNER JOIN Смена AS sh ON d.Смена = sh.ID_Смены)";

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
                throw new Exception($"Сбой запроса: {ex.Message}");
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
                            d.ID_Записи, 
                            d.Название_детали, 
                            d.Объём_партии, 
                            d.Норма_времени, 
                            d.Дата_изготовления,
                            s.Участок, 
                            w.ФИО_рабочего, 
                            sh.№_Смены
                        FROM (((Деталь AS d
                        INNER JOIN Участок AS s ON d.Участок = s.ID_Участка)
                        INNER JOIN Рабочий AS w ON d.Рабочий = w.ID_Рабочего)
                        INNER JOIN Смена AS sh ON d.Смена = sh.ID_Смены)
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

                    // ИСПРАВЛЕНО: В таблице Деталь столбцы называются Участок, Рабочий, Смена (без ID_)
                    string query = "INSERT INTO Деталь (Название_детали, Объём_партии, Норма_времени, Дата_изготовления, Участок, Рабочий, Смена) VALUES (@name, @volume, @timeNorm, @date, @sector, @worker, @shift)";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", detail.DetailName);
                        command.Parameters.AddWithValue("@volume", detail.BatchVolume);
                        command.Parameters.AddWithValue("@timeNorm", detail.TimeNorm);
                        command.Parameters.AddWithValue("@date", detail.ManufactureDate.Date);
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

                    // ИСПРАВЛЕНО: В таблице Деталь столбцы называются Участок, Рабочий, Смена (без ID_)
                    string query = "UPDATE Деталь SET Название_детали = @name, Объём_партии = @volume, Норма_времени = @timeNorm, Дата_изготовления = @date, Участок = @sector, Рабочий = @worker, Смена = @shift WHERE ID_Записи = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", detail.DetailName);
                        command.Parameters.AddWithValue("@volume", detail.BatchVolume);
                        command.Parameters.AddWithValue("@timeNorm", detail.TimeNorm);
                        command.Parameters.AddWithValue("@date", detail.ManufactureDate.Date);
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
        /// Удаление данных детали
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

        private Detail MapDetailFromReader(OleDbDataReader reader)
        {
            return new Detail
            {
                IdRecord = Convert.ToInt32(reader["ID_Записи"]),
                DetailName = reader["Название_детали"].ToString(),
                BatchVolume = Convert.ToInt32(reader["Объём_партии"]),

                TimeNorm = Convert.ToInt32(reader["Норма_времени"]),
                ManufactureDate = Convert.ToDateTime(reader["Дата_изготовления"]),

                // ИСПРАВЛЕНО: Читаем точно по названиям из БД
                SectorName = reader["Участок"].ToString(),
                WorkerFullName = reader["ФИО_рабочего"].ToString(),
                ShiftNumber = Convert.ToInt32(reader["№_Смены"])
            };
        }
    }
}