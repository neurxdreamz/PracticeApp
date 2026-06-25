using NUnit.Framework;
using Moq;
using PracticeApp.ViewModels;
using Business_Logic.Services;
using Data_Logic.Entities;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PracticeApp.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {
        private Mock<IDetailService> _detailServiceMock;
        private Mock<IAuthService> _authServiceMock;
        private Mock<IWorkerService> _workerServiceMock;
        private Mock<ISectorService> _sectorServiceMock;
        private Mock<IShiftService> _shiftServiceMock;
        private Mock<IServiceProvider> _serviceProviderMock;

        [SetUp]
        public void Setup()
        {
            _detailServiceMock = new Mock<IDetailService>();
            _authServiceMock = new Mock<IAuthService>();
            _workerServiceMock = new Mock<IWorkerService>();
            _sectorServiceMock = new Mock<ISectorService>();
            _shiftServiceMock = new Mock<IShiftService>();
            _serviceProviderMock = new Mock<IServiceProvider>();

            // Базовая настройка: пустые списки по умолчанию, чтобы ViewModel при старте не падала в NullReferenceException
            _detailServiceMock.Setup(s => s.GetAllDetails()).Returns(new List<Detail>());
            _workerServiceMock.Setup(s => s.GetAllWorkers()).Returns(new List<Worker>());
            _sectorServiceMock.Setup(s => s.GetAllSectors()).Returns(new List<Sector>());
            _shiftServiceMock.Setup(s => s.GetAllShifts()).Returns(new List<Shift>());
        }


        [Test]
        public void SetupAccessRights_ShouldSetIsAdminTrue_WhenRoleIdIs1()
        {
            // Arrange
            var viewModel = new MainViewModel(
                _detailServiceMock.Object, _authServiceMock.Object, _workerServiceMock.Object,
                _sectorServiceMock.Object, _shiftServiceMock.Object, _serviceProviderMock.Object);

            // Act
            viewModel.SetupAccessRights(1); // Роль администратора

            // Assert
            Assert.That(viewModel.IsAdmin, Is.True);
            Assert.That(viewModel.IsEditor, Is.True);
            Assert.That(viewModel.CurrentUserRoleName, Is.EqualTo("Администратор"));
        }

        [Test]
        public void SetupAccessRights_ShouldSetIsAdminFalseAndIsEditorFalse_WhenRoleIdIs2()
        {
            // Arrange
            var viewModel = new MainViewModel(
                _detailServiceMock.Object, _authServiceMock.Object, _workerServiceMock.Object,
                _sectorServiceMock.Object, _shiftServiceMock.Object, _serviceProviderMock.Object);

            // Act
            viewModel.SetupAccessRights(2); // Роль наблюдателя

            // Assert
            Assert.That(viewModel.IsAdmin, Is.False);
            Assert.That(viewModel.IsEditor, Is.False);
            Assert.That(viewModel.CurrentUserRoleName, Is.EqualTo("Наблюдатель"));
        }
    }
}

      