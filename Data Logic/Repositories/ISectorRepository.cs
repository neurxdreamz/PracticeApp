using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Logic.Repositories
{
    public interface ISectorRepository
    {
        IEnumerable<Sector> GetAllSectors();
        Sector GetSectorById(int id);
        void AddSector(Sector sector);
        void UpdateSector(Sector sector);
        void DeleteSector(int id);
    }
}
