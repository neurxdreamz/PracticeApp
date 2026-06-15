using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Logic.Repositories
{
    public interface IDetailRepository
    {
        IEnumerable<Detail> GetAllDetails();
        Detail GetDetailById(int id);
        void AddDetail(Detail detail);
        void UpdateDetail(Detail detail);
        void DeleteDetail(int id);
    }
}
