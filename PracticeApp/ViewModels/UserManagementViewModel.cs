using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data_Logic.Entities;
using Business_Logic.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System;

namespace PracticeApp.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        public Action CloseAction { get; set; }

        [ObservableProperty]
        private ObservableCollection<User> _users;

        [ObservableProperty]
        private User _selectedUser;

        // Поля ввода для формы
        [ObservableProperty]
        private string _usernameInput;

        [ObservableProperty]
        private string _passwordInput;

        [ObservableProperty]
        private int _selectedRoleId;

        // Список ролей для выпадающего списка (точно как на скриншоте)
        public ObservableCollection<RoleItem> AvailableRoles { get; set; }

        public UserManagementViewModel(IAuthService authService)
        {
            _authService = authService;

            // Настраиваем роли в соответствии с ID в твоей системе
            AvailableRoles = new ObservableCollection<RoleItem>
            {
                new RoleItem { RoleId = 1, RoleName = "Администратор" },
                new RoleItem { RoleId = 3, RoleName = "Редактор" },
                new RoleItem { RoleId = 2, RoleName = "Наблюдатель" }
            };

            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                Users = new ObservableCollection<User>(_authService.GetAllUsers());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        partial void OnSelectedUserChanged(User value)
        {
            if (value != null)
            {
                UsernameInput = value.Username;
                PasswordInput = string.Empty; 
                SelectedRoleId = value.IdRole;
            }
            else
            {
                ClearForm();
            }
        }

        [RelayCommand]
        private void Apply()
        {
          
            if (string.IsNullOrWhiteSpace(UsernameInput) || SelectedRoleId == 0)
            {
                MessageBox.Show("Заполните логин и выберите роль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (SelectedUser == null)
                {
                   
                    if (string.IsNullOrWhiteSpace(PasswordInput))
                    {
                        MessageBox.Show("Для нового пользователя необходимо задать пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _authService.RegisterNewUser(UsernameInput, SelectedRoleId, PasswordInput);
                    MessageBox.Show("Пользователь успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                  
                    SelectedUser.Username = UsernameInput;
                    SelectedUser.IdRole = SelectedRoleId;

                 
                    if (!string.IsNullOrWhiteSpace(PasswordInput))
                    {
                        SelectedUser.PasswordHash = PasswordInput;
                    }

                    _authService.UpdateUser(SelectedUser);
                    MessageBox.Show("Данные пользователя обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LoadUsers();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedUser.IdUser == 1)
            {
                MessageBox.Show("Нельзя удалить главного администратора!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show($"Удалить пользователя '{SelectedUser.Username}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _authService.DeleteUser(SelectedUser.IdUser);
                    LoadUsers();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            ClearForm();
        }

        private void ClearForm()
        {
            SelectedUser = null;
            UsernameInput = string.Empty;
            PasswordInput = string.Empty;
            SelectedRoleId = 0;
        }
    }
}