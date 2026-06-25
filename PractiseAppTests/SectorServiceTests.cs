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
    public class SectorServiceTests
    {
        private Mock<ISectorRepository> _sectorRepositoryMock;
        private SectorService _sectorService;

        [SetUp]
        public void Setup()
        {
            _sectorRepositoryMock = new Mock<ISectorRepository>();
            _sectorService = new SectorService(_sectorRepositoryMock.Object);
        }

        [Test]
        public void GetAllSectors_ShouldReturnAllSectors()
        {
            // Arrange
            var expectedSectors = new List<Sector>
            {
                new Sector { IdSector = 1, SectorName = "Токарный", ManagerFullName = "Смирнов А.А." },
                new Sector { IdSector = 2, SectorName = "Фрезерный", ManagerFullName = "Козлов В.В." }
            };
            _sectorRepositoryMock.Setup(r => r.GetAllSectors()).Returns(expectedSectors);

            // Act
            var actualSectors = _sectorService.GetAllSectors().ToList();

            // Assert
            Assert.That(actualSectors, Is.Not.Null);
            Assert.That(actualSectors.Count, Is.EqualTo(2));
            Assert.That(actualSectors[0].SectorName, Is.EqualTo("Токарный"));
        }

        [Test]
        public void AddSector_ShouldCallRepositoryAddMethodOnce()
        {
            // Arrange
            var newSector = new Sector { SectorName = "Сборочный", ManagerFullName = "Николаев Н.Н." };

            // Act
            _sectorService.AddSector(newSector);

            // Assert
            _sectorRepositoryMock.Verify(r => r.AddSector(It.Is<Sector>(s => s.SectorName == "Сборочный")), Times.Once);
        }

        [Test]
        public void DeleteSector_ShouldCallRepositoryDeleteMethodOnce()
        {
            // Arrange
            int sectorId = 3;

            // Act
            _sectorService.DeleteSector(sectorId);

            // Assert
            _sectorRepositoryMock.Verify(r => r.DeleteSector(sectorId), Times.Once);
        }
    }
}