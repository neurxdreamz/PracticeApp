using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.Services
{
    public interface IWorkerService
    {
        IEnumerable<Worker> GetAllWorkers();
        Worker GetWorkerById(int id);
        void AddWorker(Worker worker);
        void UpdateWorker(Worker worker);
        void DeleteWorker(int id);
    }
}
