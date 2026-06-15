using Data_Logic.Entities;
using Data_Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.Services
{
    public class SectorService : ISectorService
    {
        private readonly ISectorRepository sectorRepository;

        public SectorService(ISectorRepository SectorRepository)
        {
            sectorRepository = SectorRepository ?? throw new ArgumentNullException(nameof(SectorRepository));
        }

        public IEnumerable<Sector> GetAllSectors()
        {
            return sectorRepository.GetAllSectors();
        }

        public Sector GetSectorById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID участка должен быть положительным числом.", nameof(id));

            return sectorRepository.GetSectorById(id);
        }

        public void AddSector(Sector sector)
        {
            ValidateSector(sector);
            sectorRepository.AddSector(sector);
        }

        public void UpdateSector(Sector sector)
        {
            if (sector == null)
                throw new ArgumentNullException(nameof(sector));

            if (sector.IdSector <= 0)
                throw new ArgumentException("Невозможно обновить запись: некорректный ID.", nameof(sector.IdSector));

            ValidateSector(sector);
            sectorRepository.UpdateSector(sector);
        }

        public void DeleteSector(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID участка должен быть положительным числом.", nameof(id));

          
            sectorRepository.DeleteSector(id);
        }

      
        private void ValidateSector(Sector sector)
        {
            if (sector == null)
                throw new ArgumentNullException(nameof(sector));

          
            if (string.IsNullOrWhiteSpace(sector.SectorName))
                throw new ArgumentException("Название участка обязательно для заполнения.");

            
            if (sector.SectorName.Trim().Length < 3)
                throw new ArgumentException("Название участка должно содержать минимум 3 символа.");

           
            if (!string.IsNullOrWhiteSpace(sector.ManagerFullName) && sector.ManagerFullName.Trim().Length < 4)
                throw new ArgumentException("ФИО начальника слишком короткое.");
        }
    }
}
    

