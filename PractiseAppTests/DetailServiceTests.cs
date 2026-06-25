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
    public class DetailServiceTests
    {
        private Mock<IDetailRepository> _detailRepositoryMock;
        private DetailService _detailService;

        [SetUp]
        public void Setup()
        {
            _detailRepositoryMock = new Mock<IDetailRepository>();
            _detailService = new DetailService(_detailRepositoryMock.Object);
        }

        [Test]
        public void GetAllDetails_ShouldReturnAllDetails()
        {
            // Arrange
            var expectedDetails = new List<Detail>
            {
                new Detail { IdRecord = 1, DetailName = "Шестерня", BatchVolume = 100 },
                new Detail { IdRecord = 2, DetailName = "Вал", BatchVolume = 50 }
            };
            _detailRepositoryMock.Setup(r => r.GetAllDetails()).Returns(expectedDetails);

            // Act
            var actualDetails = _detailService.GetAllDetails().ToList();

            // Assert
            Assert.That(actualDetails, Is.Not.Null);
            Assert.That(actualDetails.Count, Is.EqualTo(2));
            Assert.That(actualDetails[1].DetailName, Is.EqualTo("Вал"));
        }

        [Test]
        public void UpdateDetail_ShouldCallRepositoryUpdateMethodOnce()
        {
            // Arrange
            var detailToUpdate = new Detail
            {
                IdRecord = 1,
                DetailName = "Обновленная Шестерня",
                BatchVolume = 50,  
                TimeNorm = 5,
                SectorId = 12,
                WorkerId = 3,
                ShiftId = 3
                
            };

            // Act
            _detailService.UpdateDetail(detailToUpdate);

            // Assert
            _detailRepositoryMock.Verify(r => r.UpdateDetail(It.Is<Detail>(d => d.DetailName == "Обновленная Шестерня")), Times.Once);
        }

        [Test]
        public void DeleteDetail_ShouldCallRepositoryDeleteMethodOnce()
        {
            // Arrange
            int detailId = 10;

            // Act
            _detailService.DeleteDetail(detailId);

            // Assert
            _detailRepositoryMock.Verify(r => r.DeleteDetail(detailId), Times.Once);
        }
    }
}