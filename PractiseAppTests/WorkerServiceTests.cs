using NUnit.Framework;
using Moq;
using Business_Logic.Services;
using Data_Logic.Repositories;
using Data_Logic.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PracticeApp.Tests
{
    [TestFixture]
    public class WorkerServiceTests
    {
        private Mock<IWorkerRepository> _workerRepositoryMock;
        private WorkerService _workerService;

        [SetUp]
        public void Setup()
        {
            _workerRepositoryMock = new Mock<IWorkerRepository>();
            // Убедись, что конструктор WorkerService принимает IWorkerRepository
            _workerService = new WorkerService(_workerRepositoryMock.Object);
        }

        [Test]
        public void GetAllWorkers_ShouldReturnAllWorkersFromRepository()
        {
            // Arrange
            var expectedWorkers = new List<Worker>
            {
                new Worker { IdWorker = 1, FullName = "Иванов И.И.", Grade = 4 },
                new Worker { IdWorker = 2, FullName = "Петров П.П.", Grade = 5 }
            };
            _workerRepositoryMock.Setup(r => r.GetAllWorkers()).Returns(expectedWorkers);

            // Act
            var actualWorkers = _workerService.GetAllWorkers().ToList();

            // Assert
            Assert.That(actualWorkers, Is.Not.Null);
            Assert.That(actualWorkers.Count, Is.EqualTo(2));
            Assert.That(actualWorkers[0].FullName, Is.EqualTo("Иванов И.И."));
        }

        [Test]
        public void AddWorker_ShouldCallRepositoryAddMethodOnce()
        {
            // Arrange: заполняем ВСЕ важные поля, чтобы пройти валидацию внутри сервиса
            var newWorker = new Worker
            {
                FullName = "Сидоров С.С.",
                Grade = 3,
                Specialty = "Токарь",    // Добавили специальность
                TariffRate = 500         // Добавили ставку
            };

            // Act
            _workerService.AddWorker(newWorker);

            // Assert
            _workerRepositoryMock.Verify(r => r.AddWorker(It.Is<Worker>(w => w.FullName == "Сидоров С.С.")), Times.Once);
        }

        [Test]
        public void DeleteWorker_ShouldCallRepositoryDeleteMethodOnce()
        {
            // Arrange
            int workerIdToDelete = 5;

            // Act
            _workerService.DeleteWorker(workerIdToDelete);

            // Assert
            _workerRepositoryMock.Verify(r => r.DeleteWorker(workerIdToDelete), Times.Once);
        }
    }
}