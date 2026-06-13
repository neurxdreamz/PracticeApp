using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;

namespace Data_Logic.Repositories
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly string connectionString;

        public ShiftRepository()
        {
            connectionString = DataBaseConfig.GetConnectionString();
        }

        /// <summary>
        /// Получение всех смен
        /// </summary>
        public IEnumerable<Shift> GetAllShifts()
        {
            var shifts = new List<Shift>();

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                  
                    string query = "SELECT ID_Смены, [№_Смены], Бригадир FROM Смена";

                    using (var command = new OleDbCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            shifts.Add(MapShiftFromReader(reader));
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при получении списка смен из БД.", ex);
            }

            return shifts;
        }

        /// <summary>
        /// Получение смены по ID
        /// </summary>
        public Shift GetShiftById(int id)
        {
            if (id <= 0) throw new ArgumentException("Некорректный ID смены.", nameof(id));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID_Смены, [№_Смены], Бригадир FROM Смена WHERE ID_Смены = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapShiftFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception($"Ошибка при получении смены с ID {id}.", ex);
            }

            return null;
        }

        /// <summary>
        /// Добавление смены
        /// </summary>
        public void AddShift(Shift shift)
        {
            if (shift == null) throw new ArgumentNullException(nameof(shift));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Смена ([№_Смены], Бригадир) VALUES (@shiftNumber, @foreman)";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@shiftNumber", shift.ShiftNumber);
                        command.Parameters.AddWithValue("@foreman", shift.Foreman ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при добавлении новой смены.", ex);
            }
        }

        /// <summary>
        /// Обновление данных смены
        /// </summary>
        public void UpdateShift(Shift shift)
        {
            if (shift == null) throw new ArgumentNullException(nameof(shift));
            if (shift.IdShift <= 0) throw new ArgumentException("Невозможно обновить смену без корректного ID.");

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Смена SET [№_Смены] = @shiftNumber, Бригадир = @foreman WHERE ID_Смены = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@shiftNumber", shift.ShiftNumber);
                        command.Parameters.AddWithValue("@foreman", shift.Foreman ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@id", shift.IdShift);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при обновлении данных смены.", ex);
            }
        }

        /// <summary>
        /// Удаление смены
        /// </summary>
        public void DeleteShift(int id)
        {
            if (id <= 0) throw new ArgumentException("Некорректный ID смены.", nameof(id));

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Смена WHERE ID_Смены = @id";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw new Exception("Ошибка при удалении смены.", ex);
            }
        }

        private Shift MapShiftFromReader(OleDbDataReader reader)
        {
            return new Shift
            {
                IdShift = Convert.ToInt32(reader["ID_Смены"]),

                ShiftNumber = reader["№_Смены"] != DBNull.Value ? Convert.ToInt32(reader["№_Смены"]) : 0,

                Foreman = reader["Бригадир"] != DBNull.Value ? reader["Бригадир"].ToString() : string.Empty
            };
        }
    }
}
