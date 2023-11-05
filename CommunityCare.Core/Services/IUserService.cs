using System;
using System.Collections.Generic;

using CommunityCare.Core.Models;

namespace CommunityCare.Core.Services
{
    // This interface describes the operations that a UserService class implementation should provide
    public interface IUserService
    {
        // Initialise the repository - only to be used during development 
        void Initialise();

        // ---------------- User Management --------------
        List<User> GetUsers();
        User GetUser(int id);
        User GetUserByEmail(string email);
        bool IsEmailAvailable(string email, int userId);
        User AddUserV(string name, string surname, int phoneNumber, string county, string email, string password, int accessNI, Role role, bool validator);
        User AddUserI(string name, string address, int phoneNumber, string county, string email, string password, int companyNumber, Role role, bool validator );
        User AddUser(string name, string surname, string address, int phoneNumber, string county, string email, string password, int companyNumber,  int accessNI, Role role);
        User AddUser(string name, string email, string password, Role role);
        User UpdateUser(User updated);
        bool DeleteUser(int id);
        User Authenticate(string email, string password);



        //--------------------------- Schedule MAnagement ----------------------
      /*  Schedule CreateSchedule(int userId, string note);*/
        Schedule CreateSchedule(int userId, string note, DateTime scheduleTime, string residentName);
        Schedule CreateSchedule(int userId,DateTime scheduleTime,String homeLocation,String homeName,String scheduleNote,String residentName);
        Schedule GetSchedule(int id);
        Schedule CloseSchedule (int id);
        Schedule UpdateSchedule(Schedule updated);
        bool DeleteSchedule (int id);
        IList<Schedule> GetOpenSchedules();
        
        IList<Schedule> GetAllSchedules();

        //------------------------- Profile Management ------------------------
        IList<User> GetAllUsers();

    }
}
