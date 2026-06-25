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
    public class ShiftServiceTests
    {
        private Mock<IShiftRepository> _shiftRepositoryMock;
        private ShiftService _shiftService;

        [SetUp]
        public void Setup()
        {
            _shiftRepositoryMock = new Mock<IShiftRepository>();
            _shiftService = new ShiftService(_shiftRepositoryMock.Object);
        }

        [Test]
        public void GetAllShifts_ShouldReturnAllShifts()
        {
            // Arrange
            var expectedShifts = new List<Shift>
            {
                new Shift { IdShift = 1, ShiftNumber = 1, Foreman = "Григорьев Г.Г." },
                new Shift { IdShift = 2, ShiftNumber = 2, Foreman = "Васильев В.В." }
            };
            _shiftRepositoryMock.Setup(r => r.GetAllShifts()).Returns(expectedShifts);

            // Act
            var actualShifts = _shiftService.GetAllShifts().ToList();

            // Assert
            Assert.That(actualShifts, Is.Not.Null);
            Assert.That(actualShifts.Count, Is.EqualTo(2));
            Assert.That(actualShifts[1].ShiftNumber, Is.EqualTo(2));
        }

        [Test]
        public void AddShift_ShouldCallRepositoryAddMethodOnce()
        {
            // Arrange
            var newShift = new Shift { ShiftNumber = 3, Foreman = "Дмитриев Д.Д." };

            // Act
            _shiftService.AddShift(newShift);

            // Assert
            _shiftRepositoryMock.Verify(r => r.AddShift(It.Is<Shift>(s => s.ShiftNumber == 3)), Times.Once);
        }

        [Test]
        public void DeleteShift_ShouldCallRepositoryDeleteMethodOnce()
        {
            // Arrange
            int shiftId = 2;

            // Act
            _shiftService.DeleteShift(shiftId);

            // Assert
            _shiftRepositoryMock.Verify(r => r.DeleteShift(shiftId), Times.Once);
        }
    }
}