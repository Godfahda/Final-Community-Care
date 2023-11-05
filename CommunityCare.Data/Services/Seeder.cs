using System;
using System.Text;
using System.Collections.Generic;

using CommunityCare.Core.Models;
using CommunityCare.Core.Services;

namespace CommunityCare.Data.Services
{
    public static class Seeder
    {
        // use this class to seed the database with dummy 
        // test data using an IUserService 
         public static void Seed(IUserService svc)
        {
            svc.Initialise();

            // adding users
            var u1 = svc.AddUser("Administrator", "admin@mail.com", "admin", Role.admin);
            var u2 = svc.AddUser("Volunteer", "volunteer@mail.com", "volunteer", Role.volunteer);
            var u3 = svc.AddUser("Institution", "institution@mail.com", "institution", Role.institution);
            var u4 = svc.AddUserI("Large Manor", "2 Heavens Street", 123456789, "Antrim", "large@mail.com","large",1234567890,Role.institution,true );
            var u5 = svc.AddUserV("John", "Doe",987654321,"Londonderry","john@mail.com","john",1408529630,Role.volunteer,true);
            var u6 = svc.AddUserI("Small Manor", "2 Devils Street", 666999666, "Strabane", "small@mail.com", "small", 1996669990, Role.institution, true);
            var u7 = svc.AddUserV("Sarah", "Conor", 587654321, "Coleraine", "sarah@mail.com", "sarah", 1588529630, Role.volunteer, false);


            //adding schedules
            var s1 = svc.CreateSchedule(u6.Id,"We need a baker to take classes on baking",Convert.ToDateTime("2022,09,30") ,"Whole Unit");
            var s2 = svc.CreateSchedule(u4.Id,"Samantha has requested an Afrobeats playlist. Can anyone help?",Convert.ToDateTime("2022,09,25"),"Samantha G.");
            var s3 = svc.CreateSchedule(u6.Id, " We are going to be airing the Manchester Derby this Saturday. Calling on all fans to come watch together",Convert.ToDateTime("2022,10,15"),"Whole Unit");
            var s4 = svc.CreateSchedule(u4.Id, "We would be having a christmas carol choir. Can any musician come arond to coordinate our christmas choir?", Convert.ToDateTime("2022,12,05"), "Whole unit");
            var s5 = svc.CreateSchedule(u4.Id, "Does anyone know how to play a violin? We would be glad if anyone can come play for a residents birthday next Tuesday", Convert.ToDateTime("2022.09.27"), "Ethel B.");
           
        }

    }
}
