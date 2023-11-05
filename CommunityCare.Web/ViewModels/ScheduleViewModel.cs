using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CommunityCare.Web.ViewModels
{
    public class ScheduleViewModel
    {
        public SelectList Users { set; get; }

        [Required]
        [Display(Name = "Select User")]

        public int UserId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string ScheduleNote { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string ResidentName { get; set; }

        [Required]
        public DateTime ScheduleTime { get; set; }
    }
}
