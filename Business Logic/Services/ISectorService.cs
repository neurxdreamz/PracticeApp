using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.Services
{
    public interface ISectorService
    {
        IEnumerable<Sector> GetAllSectors();
        Sector GetSectorById(int id);
        void AddSector(Sector sector);
        void UpdateSector(Sector sector);
        void DeleteSector(int id);
    }
}
