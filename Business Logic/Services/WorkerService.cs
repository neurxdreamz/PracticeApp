using Data_Logic.Entities;
using Data_Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly IWorkerRepository workerRepository;

        public WorkerService(IWorkerRepository WorkerRepository)
        {
            workerRepository = WorkerRepository ?? throw new ArgumentNullException(nameof(WorkerRepository));
        }

        public IEnumerable<Worker> GetAllWorkers()
        {
            return workerRepository.GetAllWorkers();
        }

        public Worker GetWorkerById(int id)
        {
            if (id < 0)
            {
                throw new ArgumentException("ID рабочего должен быть положительным числом!", nameof(id));
            }

            return workerRepository.GetWorkerById(id);
        }
        
        public void AddWorker (Worker worker)
        {
            ValidateWorker(worker);
            workerRepository.AddWorker(worker);
        }

        public void UpdateWorker (Worker worker)
        {
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(worker));
            }

            if (worker.IdWorker < 0)
            {
                throw new ArgumentException("Ошибка! Некорректный Id", nameof(worker.IdWorker));
            }

            ValidateWorker(worker);
            workerRepository.UpdateWorker(worker);
        }

        public void DeleteWorker(int id)
        {
            workerRepository.DeleteWorker(id);
        }

        private void ValidateWorker(Worker worker)
        {
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(worker));
            }

            if (string.IsNullOrWhiteSpace(worker.FullName))
            {
                throw new ArgumentException("ФИО рабочего должно быть заполнено");
            }

            if (worker.Grade < 1 || worker.Grade > 6)
                throw new ArgumentException("Разряд рабочего должен быть в диапазоне от 1 до 6.");

           
            if (worker.TariffRate < 0)
                throw new ArgumentException("Тарифная ставка не может быть отрицательной.");

            if (string.IsNullOrWhiteSpace(worker.Specialty))
                throw new ArgumentException("Специальность рабочего обязательна для заполнения.");
        }

    }
}
