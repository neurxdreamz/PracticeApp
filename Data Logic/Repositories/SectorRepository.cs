using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;

namespace Data_Logic.Repositories
{
    public class SectorRepository : ISectorRepository
    {
        private readonly string connectionString;

        public SectorRepository()
        {
            connectionString = DataBaseConfig.GetConnectionString();
        }

        /// <summary>
        /// Получение всех участков
        /// </summary>
        public IEnumerable<Sector> GetAllSectors()
        {
            var sectors = new List<Sector>();

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID_Участка, Участок, ФИО_начальника FROM Участок";

                    using (var command = new OleDbCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sectors.Add(MapSectorFromReader(reader));
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при получении списка участков из БД.", ex);
            }

            return sectors;
        }

        /// <summary>
        /// Получение участка по ID
        /// </summary>
        public Sector GetSectorById(int id)
        {
            if (id <= 0) throw new ArgumentException("Некорректный ID участка.", nameof(id));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID_Участка, Участок, ФИО_начальника FROM Участок WHERE ID_Участка = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapSectorFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception($"Ошибка при получении участка с ID {id}.", ex);
            }

            return null;
        }

        /// <summary>
        /// Добавление участка
        /// </summary>
        public void AddSector(Sector sector)
        {
            if (sector == null) throw new ArgumentNullException(nameof(sector));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Участок (Участок, ФИО_начальника) VALUES (@sectorName, @manager)";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@sectorName", sector.SectorName);
                        command.Parameters.AddWithValue("@manager", sector.ManagerFullName ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при добавлении нового участка.", ex);
            }
        }

        /// <summary>
        /// Обновление данных участка
        /// </summary>
        public void UpdateSector(Sector sector)
        {
            if (sector == null) throw new ArgumentNullException(nameof(sector));
            if (sector.IdSector <= 0) throw new ArgumentException("Невозможно обновить участок без корректного ID.");

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
        
                    string query = "UPDATE Участок SET Участок = @sectorName, ФИО_начальника = @manager WHERE ID_Участка = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@sectorName", sector.SectorName);
                        command.Parameters.AddWithValue("@manager", sector.ManagerFullName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@id", sector.IdSector);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при обновлении данных участка.", ex);
            }
        }

        /// <summary>
        /// Удаление участка
        /// </summary>
        public void DeleteSector(int id)
        {
            if (id <= 0) throw new ArgumentException("Некорректный ID участка.", nameof(id));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Участок WHERE ID_Участка = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при удалении участка.", ex);
            }
        }

        private Sector MapSectorFromReader(OleDbDataReader reader)
        {
            return new Sector
            {
                IdSector = Convert.ToInt32(reader["ID_Участка"]),
                SectorName = reader["Участок"].ToString(),
                ManagerFullName = reader["ФИО_начальника"] != DBNull.Value ? reader["ФИО_начальника"].ToString() : string.Empty
            };
        }
    }
}
