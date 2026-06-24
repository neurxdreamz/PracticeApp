using Data_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.Services
{
    public interface IAuthService
    {
        void RegisterNewUser (string username, int RoleId, string RawPassword);
        bool IsUserExist(string username);
        User AuthenticateUser(string username, string rawPassword);
    }
}
