using NUnit.Framework;
using Moq;
using Business_Logic.Services;
using Data_Logic.Repositories;
using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PracticeApp.Tests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            // Создаем "фейковый" репозиторий перед каждым тестом
            _userRepositoryMock = new Mock<IUserRepository>();
            _authService = new AuthService(_userRepositoryMock.Object);
        }

        [Test]
        public void RegisterNewUser_ShouldSuccess_WhenUserDoesNotExist()
        {
            // Arrange (Подготовка данных)
            string username = "new_user";
            int roleId = 2;
            string password = "password123";

            // Настраиваем фейковый метод: по умолчанию пользователя с таким именем нет
            _userRepositoryMock.Setup(r => r.GetUserByUsername(username)).Returns((User)null);

            // Act (Выполнение действия)
            _authService.RegisterNewUser(username, roleId, password);

            // Assert (Проверка результатов)
            // Проверяем, что метод AddUser в репозитории был вызван ровно 1 раз
            _userRepositoryMock.Verify(r => r.AddUser(
                It.Is<User>(u => u.Username == username && u.IdRole == roleId),
                It.IsAny<string>()),
                Times.Once);
        }

        [Test]
        public void RegisterNewUser_ShouldThrowException_WhenUserAlreadyExists()
        {
            // Arrange
            string username = "existing_admin";
            _userRepositoryMock.Setup(r => r.GetUserByUsername(username)).Returns(new User { Username = username });

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _authService.RegisterNewUser(username, 1, "12345"));

            // ИСПОЛЬЗУЕМ НОВЫЙ СИНТАКСИС: Is.EqualTo
            Assert.That(ex.Message, Is.EqualTo("Пользователь с таким логином уже существует!"));
        }

        [Test]
        public void RegisterNewUser_ShouldThrowException_WhenPasswordIsTooShort()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                _authService.RegisterNewUser("user1", 2, "123")); // Пароль меньше 4 символов

            // ИСПОЛЬЗУЕМ НОВЫЙ СИНТАКСИС: Does.Contain
            Assert.That(ex.Message, Does.Contain("Пароль должен содержать минимум 4 символа"));
        }

        [Test]
        public void AuthenticateUser_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            string username = "admin";
            string password = "password";
            // Внутренний хэшер выдаст одинаковую строку для одного и того же пароля
            string correctHash = Business_Logic.Security.PasswordHasher.HashPassword(password);

            var dbUser = new User { IdUser = 1, Username = username, PasswordHash = correctHash, IdRole = 1 };
            _userRepositoryMock.Setup(r => r.GetUserByUsername(username)).Returns(dbUser);

            // Act
            var result = _authService.AuthenticateUser(username, password);

            // Assert
            // ИСПОЛЬЗУЕМ НОВЫЙ СИНТАКСИС: Is.Not.Null и Is.EqualTo
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(username));
        }

        [Test]
        public void AuthenticateUser_ShouldThrowException_WhenPasswordIsWrong()
        {
            // Arrange
            string username = "admin";
            var dbUser = new User { IdUser = 1, Username = username, PasswordHash = "some_old_hash", IdRole = 1 };
            _userRepositoryMock.Setup(r => r.GetUserByUsername(username)).Returns(dbUser);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _authService.AuthenticateUser(username, "wrong_password"));

            // ИСПОЛЬЗУЕМ НОВЫЙ СИНТАКСИС: Is.EqualTo
            Assert.That(ex.Message, Is.EqualTo("Неверный пароль."));
        }

        [Test]
        public void UpdateUser_ShouldHashPassword_WhenPasswordIsChanged()
        {
            
            int userId = 5;
            var oldUserInDb = new User { IdUser = userId, Username = "user5", PasswordHash = "old_hash", IdRole = 2 };

            
            _userRepositoryMock.Setup(r => r.GetAllUsers()).Returns(new List<User> { oldUserInDb });

            
            var userFromForm = new User { IdUser = userId, Username = "user5", PasswordHash = "new_pass_123", IdRole = 2 };

          
            _authService.UpdateUser(userFromForm);

          
            Assert.That(userFromForm.PasswordHash, Is.Not.EqualTo("new_pass_123"));

            _userRepositoryMock.Verify(r => r.UpdateUser(It.Is<User>(u => u.IdUser == userId)), Times.Once);
        }
    }
}