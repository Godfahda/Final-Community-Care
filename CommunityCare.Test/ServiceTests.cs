
using Xunit;
using CommunityCare.Core.Models;
using CommunityCare.Core.Services;

using System;
using System.Text;
using System.Collections.Generic;


using CommunityCare.Data.Services;

namespace CommunityCare.Test
{
    public class ServiceTests
    {
        private IUserService service;

        public ServiceTests()
        {
            service = new UserServiceDb();

            //empties data source before each test
            service.Initialise();
        }

        [Fact]
        public void EmptyDbShouldReturnNoUsers()
        {
            // act
            var users = service.GetUsers();

            // assert
            Assert.Empty(users);
        }
        
        [Fact]
        public void AddingUsersShouldWork()
        {
            // arrange
            //This creates and adds two users to the database
            service.AddUser("admin", "admin@mail.com", "admin", Role.admin );
            service.AddUser("institution", "institution@mail.com", "institution", Role.institution);

            // act
            // This calls the method that counts the total number of users created
            var users = service.GetUsers();

            // assert
            // This checks and asserts that the present user count is equal to 2
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public void UpdatingUserShouldWork()
        {
            // arrange
            var user = service.AddUser("admin", "admin@mail.com", "admin", Role.admin );
            
            // act
            user.Name = "administrator";
            user.Email = "admin@mail.com";            
            var updatedUser = service.UpdateUser(user);

            // assert
            Assert.Equal("administrator", user.Name);
            Assert.Equal("admin@mail.com", user.Email);
        }

        [Fact]
        public void LoginWithValidCredentialsShouldWork()
        {
            // arrange
            service.AddUser("admin", "admin@mail.com", "admin", Role.admin );
            
            // act            
            var user = service.Authenticate("admin@mail.com","admin");

            // assert
            Assert.NotNull(user);
           
        }

        [Fact]
        public void LoginWithInvalidCredentialsShouldNotWork()
        {
            // arrange
            service.AddUser("admin", "admin@mail.com", "admin", Role.admin );

            // act      
            var user = service.Authenticate("admin@mail.com","xxx");

            // assert
            Assert.Null(user);
           
        }

        [Fact]
        public void AddUser_With_Duplicate_Email_Should_Not_Work()
        {
            //act
            var u1 = service.AddUser("Paul", "paul@mail.com", "paul", Role.volunteer);
            //attempted duplicate user with same email
            var u2 = service.AddUser("John", "paul@mail.com", "John", Role.volunteer);

            //assert
            Assert.NotNull(u1); //User should have been successfully added
            Assert.Null(u2); //User should not have been added
        }

        [Fact]
        public void AddUser_Should_Assign_All_Properties_Correctly()
        {
            //act
            var u1 = service.AddUser("Paul", "paul@mail.com", "paul", Role.volunteer);

            //retrieve created user through user Id
            var u = service.GetUser(u1.Id);

            //Assert- that user is not null
            Assert.NotNull(u);

            //Check that properties are assigned correctly
            Assert.Equal(u.Id, u.Id);
            Assert.Equal("Paul", u.Name);
            Assert.Equal("paul@mail.com", u.Email);
            Assert.Equal(Role.volunteer, u.Role);

        }

        [Fact]
        public void User_Update_should_set_all_properties()
        {
            //add test user
            var add = service.AddUserV("Sarah", "Conor", 587654321, "Coleraine", "sarah@mail.com", "sarah", 1588529630, Role.volunteer, false);

            //act - create copy of user and update any user
            var u = new User
            {
                Id = add.Id,
                Name = "Jade",
                Surname = "Smith",
                PhoneNumber = 587654123,
                County = "Londonderry",
                Email = "jade@mail.com",
                Password = "jade",
                AccessNI = 1234558453,
                Role = Role.volunteer,
                Validator = true
            };
            // save updated user
            service.UpdateUser(u);

            //retrieve user from database
            var check = service.GetUser(add.Id);

            //assert
            Assert.NotNull(u);

            //Assert to check if properties were assigned correctly
            Assert.Equal(u.Name, check.Name);
            Assert.Equal(u.Surname, check.Surname);
            Assert.Equal(u.PhoneNumber, check.PhoneNumber);
            Assert.Equal(u.County, check.County);
            Assert.Equal(u.Email, check.Email);
            Assert.Equal(u.AccessNI, check.AccessNI);
            Assert.Equal(u.Role, check.Role);
            Assert.Equal(u.Validator, check.Validator);

        }

        [Fact]
        public void User_GetAllUsers_WhenNone_ShouldReturn0()
        {
            //act
            var users = service.GetAllUsers();
            var count = users.Count;

            //assert
            Assert.Equal(0, count);
        }


        [Fact]
        public void User_GetAllUsers_When2Exist_ShouldReturn2()
        {
            //arrange
            var u1 = service.AddUser("Paul", "paul@mail.com", "paul", Role.volunteer);
            var u2 = service.AddUserV("Sarah", "Conor", 587654321, "Coleraine", "sarah@mail.com", "sarah", 1588529630, Role.volunteer, false);

            //act
            var users = service.GetAllUsers();
            var count = users.Count;

            //assert
            Assert.Equal(2, count);
        }

        [Fact]
        public void Users_GetExistingUser_Shouldwork()
        {
            //act 
            var u1 = service.AddUser("Paul", "paul@mail.com", "paul", Role.volunteer);

            var newU = service.GetUser(u1.Id);

            //assert
            Assert.NotNull(newU);
            Assert.Equal(u1.Id, newU.Id);
        }

        [Fact]
        public void User_DeleteExistingUser_ShouldReturnTrue()
        {
            var u1 = service.AddUser("Paul", "paul@mail.com", "paul", Role.volunteer);
            var deleted = service.DeleteUser(u1.Id);

            //attempt to retrieve deleted student
            var u = service.GetUser(u1.Id);

            //assert
            Assert.True(deleted);
            Assert.Null(u);
        }

        [Fact]
        public void User_Delete_NonExistent_USer()
        {
            //act
            var deleted = service.DeleteUser(0);

            //
            Assert.False(deleted);
        }

        [Fact]
        public void ScheduleCreate_for_existing_user_should_work()
        {
            //arrange
            var u1 = service.AddUserV("Sarah", "Conor", 587654321, "Coleraine", "sarah@mail.com", "sarah", 1588529630, Role.volunteer, false);

            //act
            var s = service.CreateSchedule(u1.Id, "Test Note", Convert.ToDateTime("2020,12,25"), "Test Name");

            Assert.NotNull(s);
            Assert.Equal(u1.Id, s.UserId);
            Assert.True(s.IsActive);
        }

        [Fact]
        public void Schedule_GetSchedule_When_Exist_ShouldReturnScheduleAndUSer()
        {
            //arrange
            var u1 = service.AddUserV("Sarah", "Conor", 587654321, "Coleraine", "sarah@mail.com", "sarah", 1588529630, Role.volunteer, false);

            //act
            var s = service.CreateSchedule(u1.Id, "Test Note", Convert.ToDateTime("2020,12,25"), "Test Name");

            var schedule = service.GetSchedule(s.Id);

            //assert
            Assert.NotNull(schedule);
            Assert.NotNull(schedule.User);
            Assert.Equal(u1.Name, schedule.User.Name);   
            
        }

        [Fact]
        public void Schedule_GetOpenSchedule_WhenTwoAdded_ShouldReturnTwo()
        {
            //arrange
            var u1 = service.AddUserV("Sarah", "Conor", 587654321, "Coleraine", "sarah@mail.com", "sarah", 1588529630, Role.volunteer, false);
            var s1 = service.CreateSchedule(u1.Id, "Test Note", Convert.ToDateTime("2020,12,25"), "Test Name");
            var s2 = service.CreateSchedule(u1.Id, "Test Note 2", Convert.ToDateTime("2020,1,1"), "Test Resident 2");

            //act
            var active = service.GetOpenSchedules();

            //assert
            Assert.Equal(2, active.Count);

        }

        [Fact]
        public void Schedule_CloseSchedule_WhenOpen_ShouldReturnSchedule()
        {
            //arrange
            var u1 = service.AddUserV("Sarah", "Conor", 587654321, "Coleraine", "sarah@mail.com", "sarah", 1588529630, Role.volunteer, false);
            var s = service.CreateSchedule(u1.Id, "Test Note", Convert.ToDateTime("2020,12,25"), "Test Name");

            //act
            var c = service.CloseSchedule(s.Id);

            //assert
            Assert.NotNull(c);
            Assert.False(c.IsActive); //verify closed schedule returned
        }

        [Fact]
        public void Schedule_CloseSchedule_WhenAlreadyClose_ShouldReturnNull()
        {
            //arrange
            var u1 = service.AddUserV("Sarah", "Conor", 587654321, "Coleraine", "sarah@mail.com", "sarah", 1588529630, Role.volunteer, false); //close active schedule
            var s = service.CreateSchedule(u1.Id, "Test Note", Convert.ToDateTime("2020,12,25"), "Test Name"); //close non active schedule

            //act
            var c = service.CloseSchedule(s.Id);
            c = service.CloseSchedule(s.Id);

            //assert
            Assert.Null(c); //No schedule since it has previously been closed
            
        }

        [Fact]
        public void Schedule_DeleteSchedule_WhenExist_ShouldReturnTrue()
        {
           
            //arrange
            var u1 = service.AddUserV("Sarah", "Conor", 587654321, "Coleraine", "sarah@mail.com", "sarah", 1588529630, Role.volunteer, false);
            var s = service.CreateSchedule(u1.Id, "Test Note", Convert.ToDateTime("2020,12,25"), "Test Name");

            //act
            var delete = service.DeleteSchedule(s.Id); // Delete schedule

            //assert
            Assert.True(delete); //Schedule should be deleted
            
        }

        [Fact]
        public void Schedule_DeleteSchedule_WhenNonExistent_ShouldReturnFalse()
        {
            //act
            var delete = service.DeleteSchedule(2); //Delete non-existent schedule

            //assert
            Assert.False(delete); //Schedule should not be deleted
        }

    }

    
}
