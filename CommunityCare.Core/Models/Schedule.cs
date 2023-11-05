using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityCare.Core.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public DateTime ScheduleTime { get; set; }
        public string HomeLocation { get; set; }
        public string HomeName { get; set; }
        public string ScheduleNote { get; set; }
        public string ResidentName { get; set; }
        public string VolunteerName { get; set; }
        public string VolunteerSurname { get; set; }
        public long VolunteerPhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;

        //Schedule attributed to specific user
        public int UserId { get; set; } //foreign key
        public User User { get; set; } //navigation property

        public Schedule()
        {

        }
    }
}
