using Data_Logic.Entities;
using Data_Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.Services
{
    public class DetailService : IDetailService
    {
        private readonly IDetailRepository detailRepository;

        
        public DetailService(IDetailRepository DetailRepository)
        {
            detailRepository = DetailRepository ?? throw new ArgumentNullException(nameof(DetailRepository));
        }

        public IEnumerable<Detail> GetAllDetails()
        {
            return detailRepository.GetAllDetails();
        }

        public Detail GetDetailById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID детали должен быть положительным числом.", nameof(id));

            return detailRepository.GetDetailById(id);
        }

        public void AddDetail(Detail detail)
        {
            ValidateDetail(detail); 
            detailRepository.AddDetail(detail);
        }

        public void UpdateDetail(Detail detail)
        {
            if (detail == null)
                throw new ArgumentNullException(nameof(detail));

            if (detail.IdRecord <= 0)
                throw new ArgumentException("Невозможно обновить запись: некорректный ID.", nameof(detail.IdRecord));

            ValidateDetail(detail); 
            detailRepository.UpdateDetail(detail);
        }

        public void DeleteDetail(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID детали должен быть положительным числом.", nameof(id));

            detailRepository.DeleteDetail(id);
        }



        /// <summary>
        /// Централизованная проверка бизнес-правил для сущности "Деталь"
        /// </summary>
        private void ValidateDetail(Detail detail)
        {
            if (detail == null)
                throw new ArgumentNullException(nameof(detail));

          
            if (string.IsNullOrWhiteSpace(detail.DetailName))
                throw new ArgumentException("Название детали обязательно для заполнения.");

           
            if (detail.BatchVolume <= 0)
                throw new ArgumentException("Объем партии должен быть больше нуля.");

            if (detail.TimeNorm <= 0)
                throw new ArgumentException("Норма времени должна быть больше нуля.");

            if (detail.SectorId <= 0)
                throw new ArgumentException("Необходимо выбрать участок, на котором производилась деталь.");

            if (detail.WorkerId <= 0)
                throw new ArgumentException("Необходимо назначить рабочего, ответственного за деталь.");

            if (detail.ShiftId <= 0)
                throw new ArgumentException("Необходимо указать смену.");
        }
    }
}
