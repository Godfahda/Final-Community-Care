
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using CommunityCare.Core.Models;
using CommunityCare.Core.Services;
using CommunityCare.Core.Security;
using CommunityCare.Data.Repositories;
namespace CommunityCare.Data.Services
{
    public class UserServiceDb : IUserService
    {
        private readonly DatabaseContext  ctx;

        public UserServiceDb()
        {
            ctx = new DatabaseContext(); 
        }

        public void Initialise()
        {
           //database recreation
            ctx.Initialise(); 
        }

        // ------------------ User Related Operations ------------------------

        // retrieve list of Users
        public List<User> GetUsers()
        {
            return ctx.Users.ToList();
        }

        // Retrive User by Id and related Schedules
        public User GetUser(int id)
        {
            return ctx.Users
                      .Include(s => s.Schedules)
                      .FirstOrDefault(s => s.Id == id);
                      
        }

        //Default method to add a new user
        public User AddUser(string name, string email, string password, Role role)
        {   
            //Check if user with email exist
            var existing = GetUserByEmail(email);
            if (existing != null)
            {
                return null;
            }
            //create a new user
            var user = new User
            {
                Name = name,
                Email = email,
                Password = Hasher.CalculateHash(password),
                Role = role
            };
            ctx.Users.Add(user);
            ctx.SaveChanges();
            return user; // return newly added User
        }

         public User AddUser(string name, string surname, string address, int phoneNumber, string county, string email, string password, int companyNumber,  int accessNI, Role role)
        {   
            //Check if user with email exist
            var existing = GetUserByEmail(email);
            if (existing != null)
            {
                return null;
            }
            //create a new user
            var user = new User
            {
                Name = name,
                Surname = surname,
                Address = address,
                PhoneNumber = phoneNumber,
                County = county,
                Email = email,
                Password = Hasher.CalculateHash(password),
                CompanyNumber = companyNumber,
                AccessNI = accessNI,
                Role = role
            };
            ctx.Users.Add(user);
            ctx.SaveChanges();
            return user; // return newly added User
        }

        // Add a new User (Volunteer) checking a User with same email does not exist
        public User AddUserV(string name, string surname, int phoneNumber, string county, string email, string password, int accessNI, Role role, bool validator)
        {   
            //Check if user with email exist  
            var existing = GetUserByEmail(email);
            if (existing != null)
            {
                return null;
            }

            //create new volunteer
            var user = new User
            {
                Name = name,
                Surname = surname,
                PhoneNumber = phoneNumber,
                County = county,
                Email = email,
                Password = Hasher.CalculateHash(password), // can hash if required 
                AccessNI = accessNI,
                Role = role,
                Validator = validator
            };
            ctx.Users.Add(user);
            ctx.SaveChanges();
            return user; // return newly added User
        }

        // Add a new User (Institution) checking a User with same email does not exist
        public User AddUserI(string name, string address, int phoneNumber, string county, string email, string password, int companyNumber, Role role, bool validator)
        {
            var existing = GetUserByEmail(email);
            if (existing != null)
            {
                return null;
            }

            var user = new User
            {
                Name = name,
                Address = address,
                PhoneNumber = phoneNumber,
                County = county,
                Email = email,
                Password = Hasher.CalculateHash(password), // can hash if required 
                CompanyNumber = companyNumber,
                Role = role,
                Validator = validator
            };
            ctx.Users.Add(user);
            ctx.SaveChanges();
            return user; // return newly added User
        }

            // Delete the User identified by Id returning true if deleted and false if not found
            public bool DeleteUser(int id)
        {
            var s = GetUser(id);
            if (s == null)
            {
                return false;
            }
            ctx.Users.Remove(s);
            ctx.SaveChanges();
            return true;
        }

        // Update the User with the details in updated 
        public User UpdateUser(User updated)
        {
            // verify the User exists
            var User = GetUser(updated.Id);
            if (User == null)
            {
                return null;
            }
            // verify email address is registered or available to this user
            if (!IsEmailAvailable(updated.Email, updated.Id))
            {
                return null;
            }

            if (User.Role == Role.institution)
            // update the details of the volunteer retrieved and save
            {
                User.Name = updated.Name;
                User.Address = updated.Address;
                User.PhoneNumber = updated.PhoneNumber;
                User.County = updated.County;
                User.Email = updated.Email;
               // User.Password = Hasher.CalculateHash(updated.Password);
                User.CompanyNumber = updated.CompanyNumber;
                User.Role = updated.Role;
                User.Validator = updated.Validator;
            }

            else if (User.Role == Role.volunteer)
            // update the details of the institution retrieved and save
            {
                User.Name = updated.Name;
                User.Surname = updated.Surname;
                User.PhoneNumber = updated.PhoneNumber;
                User.County = updated.County;
                User.Email = updated.Email;
                //User.Password = Hasher.CalculateHash(updated.Password);
                User.AccessNI = updated.AccessNI;
                User.Role = updated.Role;
                User.Validator = updated.Validator;
            }
            ctx.SaveChanges();          
            return User;
        }

        // Find a user with specified email address
        public User GetUserByEmail(string email)
        {
            return ctx.Users.FirstOrDefault(u => u.Email == email);
        }

        // Verify if email is available or registered to specified user
        public bool IsEmailAvailable(string email, int userId)
        {
            return ctx.Users.FirstOrDefault(u => u.Email == email && u.Id != userId) == null;
        }

        public IList<User> GetUsersQuery(Func<User,bool> q)
        {
            return ctx.Users.Where(q).ToList();
        }

        public User Authenticate(string email, string password)
        {
            // retrieve the user based on the EmailAddress (assumes EmailAddress is unique)
            var user = GetUserByEmail(email);

            // Verify the user exists and Hashed User password matches the password provided
            return (user != null && Hasher.ValidateHash(user.Password, password)) ? user : null;
          
        }


         public IList<User> GetAllUsers()
        {
            return ctx.Users
                    //  .Include(s => s.User)
                      .ToList();
        }

        //==================================== Schedule Management ======================================
      
        public Schedule CreateSchedule(int userId, string note, DateTime scheduleTime, string residentName)
        {
            var user = GetUser(userId);
            if (user == null) return null;


            var schedule = new Schedule
            {
                HomeName = user.Name,
                HomeLocation = user.Address,
                ScheduleNote = note,
                UserId = userId,
                ScheduleTime = scheduleTime,
                ResidentName = residentName,
                IsActive = true,

            };
            ctx.Schedules.Add(schedule);
            ctx.SaveChanges();
            return schedule;
        }
        public Schedule GetSchedule(int id)
        {
            //Used to return schedules and related user or null if not found
            return ctx.Schedules
                      .Include(u => u.User)  
                      .FirstOrDefault(u => u.Id == id);
        }

        public Schedule CloseSchedule(int id)
        {
            var schedule = GetSchedule(id);
            //if schedule does not exist, it returns a null
            if (schedule == null || !schedule.IsActive) return null;

            //If schedule is active, close it
            schedule.IsActive = false;

            ctx.SaveChanges();//Commit changes to database
            return schedule;
        }

        public bool DeleteSchedule(int id)
        {
            //Query database for schedule
            var schedule = GetSchedule(id);
            if (schedule == null) return false;

            //remove schedule
            var result = ctx.Schedules.Remove(schedule);

            ctx.SaveChanges();
            return true;
        }
        //Retrieves all schedules and associated users
        public IList<Schedule> GetAllSchedules()
        {
            return ctx.Schedules
                      .Include(s => s.User)
                      .ToList();
        }

        public IList<Schedule> GetOpenSchedules()
        {
            //return all unclaimed/vacant schedules and associated users
            return ctx.Schedules
                      .Include(s => s.User)
                      .Where(s => s.IsActive)
                      .ToList();
        }

          public Schedule UpdateSchedule(Schedule updated)
        {
            // verify the User exists
            var Schedule = GetSchedule(updated.Id);
           if (Schedule == null) return updated;
         
           // if (Schedule == null) return false;

            Schedule.ScheduleNote = updated.ScheduleNote;
            Schedule.ScheduleTime = updated.ScheduleTime;
            Schedule.HomeLocation = updated.HomeLocation;
            Schedule.HomeName = updated.HomeName;
            Schedule.ResidentName = updated.ResidentName;
            Schedule.VolunteerName = updated.VolunteerName;
            Schedule.VolunteerSurname = updated.VolunteerSurname;
            Schedule.VolunteerPhoneNumber = updated.VolunteerPhoneNumber;
            Schedule.IsActive = updated.IsActive;

            ctx.SaveChanges();          
            return Schedule;
        }

          public Schedule CreateSchedule(int userId,DateTime scheduleTime,String homeLocation,String homeName,String scheduleNote,String residentName) 
          {
            var Schedule = new Schedule
            {
                UserId = userId,
                ScheduleTime = scheduleTime,
                HomeLocation = homeLocation,
                HomeName = homeName,
                ScheduleNote = scheduleNote,
                ResidentName = residentName,
               
            };
            ctx.Schedules.Add(Schedule);
            ctx.SaveChanges();
            return Schedule; // return newly added Schedule
          }

    }
}