using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.Services
{
    public interface IShiftService
    {
        IEnumerable<Shift> GetAllShifts();
        Shift GetShiftById(int id);
        void AddShift(Shift shift);
        void UpdateShift(Shift shift);
        void DeleteShift(int id);
    }
}
