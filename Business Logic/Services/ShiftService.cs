using Data_Logic.Entities;
using Data_Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository shiftRepository;

        public ShiftService(IShiftRepository ShiftRepository)
        {
            shiftRepository = ShiftRepository ?? throw new ArgumentNullException(nameof(ShiftRepository));
        }

        public IEnumerable<Shift> GetAllShifts()
        {
            return shiftRepository.GetAllShifts();
        }

        public Shift GetShiftById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID смены должен быть положительным числом.", nameof(id));

            return shiftRepository.GetShiftById(id);
        }

        public void AddShift(Shift shift)
        {
            ValidateShift(shift);
            shiftRepository.AddShift(shift);
        }

        public void UpdateShift(Shift shift)
        {
            if (shift == null)
                throw new ArgumentNullException(nameof(shift));

            if (shift.IdShift <= 0)
                throw new ArgumentException("Невозможно обновить запись: некорректный ID.", nameof(shift.IdShift));

            ValidateShift(shift);
            shiftRepository.UpdateShift(shift);
        }

        public void DeleteShift(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID смены должен быть положительным числом.", nameof(id));

            shiftRepository.DeleteShift(id);
        }

        /// <summary>
        /// Централизованная проверка бизнес-правил для сущности "Смена"
        /// </summary>
        private void ValidateShift(Shift shift)
        {
            if (shift == null)
                throw new ArgumentNullException(nameof(shift));

            


           
            if (!string.IsNullOrWhiteSpace(shift.Foreman) && shift.Foreman.Trim().Length < 4)
                throw new ArgumentException("ФИО бригадира слишком короткое. Введите полные данные.");
        }
    }
}

