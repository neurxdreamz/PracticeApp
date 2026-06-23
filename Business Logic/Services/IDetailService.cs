using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.Services
{
    public interface IDetailService
    {
        IEnumerable<Detail> GetAllDetails();
        Detail GetDetailById(int id);
        void AddDetail(Detail detail);
        void UpdateDetail(Detail detail);
        void DeleteDetail(int id);
    }
}
