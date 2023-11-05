using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CommunityCare.Data.Services;
using CommunityCare.Core.Models;
using CommunityCare.Core.Services;
using CommunityCare.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CommunityCare.Web.Controllers
{
    public class ScheduleController : BaseController
    {
        private readonly IUserService svc;

        public ScheduleController()
        {
            svc = new UserServiceDb();
        }

        //GET/scheduke/index
        public IActionResult Index()
        {
            var schedules = svc.GetAllSchedules();
            return View(schedules);
        }
        
        //GET/Schedule/ActiveSchedules
        public IActionResult IndexForActiveSchedules()
        {
            var schedules = svc.GetOpenSchedules();
            return View(schedules);
        }

    
        // GET: Schedules/Create
        [Authorize(Roles="admin,institution")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles="admin,institution")]
        public IActionResult Create([Bind("UserId","ScheduleNote","ScheduleTime","ResidentName")] ScheduleViewModel s)
        {
            var userId = User.GetSignedInUserId();    


            if (!ModelState.IsValid)
            {
               return View(s);
            }

            var schedule = svc.CreateSchedule(userId, s.ScheduleNote, s.ScheduleTime, s.ResidentName);
            
            if (schedule == null)
            {
                Alert("There was a problem creating Schedule. Please try again", AlertType.warning);
                return View(s); 
            }
            
            Alert($"Schedule created successfully", AlertType.success);
            return RedirectToAction(nameof(Index));


            
        }

         // GET /Schedule/edit/{id}
        [Authorize(Roles="admin,institution")]
        public IActionResult Edit(int id)
        {        
            // load the student using the service
            var s = svc.GetSchedule(id);

            var userId = User.GetSignedInUserId();
            var u = svc.GetUser(userId);

            // check if s is null and if so alert
            if (s == null)
            {
                Alert($"Schedule {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            if (u.Id != s.UserId)
            {
                Alert($"You can only edit schedules created by your Instituiton", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            if (u.Validator == false)
            {
                Alert($"You are not yet verified to carry out this action", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            // pass schedule to view for editing
            return View(s);
        }

        // POST /Schedule/edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="admin,institution")]
        public IActionResult Edit(int id, [Bind("Id,ScheduleTime,HomeLocation,HomeName,ScheduleNote,ResidentName")] Schedule s )
        {
        
            if (ModelState.IsValid)
            {
                // pass data to service to update
                svc.UpdateSchedule(s);
                Alert("Schedule updated successfully", AlertType.info);

                return RedirectToAction(nameof(Details), new { Id = s.Id });
            }

            return View(s);
        }

        //GET/schedule/apply/id
        [Authorize(Roles="admin,volunteer")]
        public IActionResult Apply(int id)
        {

            var s = svc.GetSchedule(id);
            var userId = User.GetSignedInUserId();
            var u = svc.GetUser(userId);

            if (s == null)
            {
                Alert($"Schedule {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }   

            if (u.Validator == false)
            {
                Alert($"You are not yet verified to carry out this action", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            if (s.IsActive == false)
            {
                Alert($"Sorry this schedule has already been assigned to a user", AlertType.info);
                return RedirectToAction(nameof(Index));
            }

            // pass schedule to view to apply
            return View(s);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="admin,volunteer")]
        public IActionResult ApplyConfirm(int id)
        {       
            var s = svc.GetSchedule(id);

            var userId = User.GetSignedInUserId(); 
            var u = svc.GetUser(userId);

            
            s.VolunteerName = u.Name;
            s.VolunteerSurname = u.Surname;
            s.VolunteerPhoneNumber = u.PhoneNumber;
            s.IsActive = false;

            svc.UpdateSchedule(s);

            Alert("Schedule applied for successfully", AlertType.info);
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details (int id)
        {
            var s = svc.GetSchedule(id);

            if (s == null)
            {
                Alert($"Schedule (id) not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View (s);
        }
          // GET / Schedule/delete/{id}
        [Authorize(Roles="admin,institution")]      
        public IActionResult Delete(int id)
        {       
            // load the schedule using the service
            var s = svc.GetSchedule(id);
            var userId = User.GetSignedInUserId();
            var u = svc.GetUser(userId);

            if (u.Validator == false)
            {
                Alert($"You are not yet verified to carry out this action", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            if (u.Id != s.UserId)
            {
                Alert("$You can only delete schedules created by your Institution", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            // check the returned defibrillator is not null and if so return NotFound()
            if (s == null)
            {
                // TBC - Display suitable warning alert and redirect
                Alert($"Schedule {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }     
            
            // pass schedule to view for deletion confirmation
            return View(s);
        }

        // POST /Schedule/delete/{id}
        [HttpPost]
       [Authorize(Roles="admin,institution")]
        [ValidateAntiForgeryToken]              
        public IActionResult DeleteConfirm(int id)
        {
            // delete schedule via service
            svc.DeleteSchedule(id);

            Alert("Schedule deleted successfully", AlertType.info);
            // redirect to the index view
            return RedirectToAction(nameof(Index));
        }


        // GET: Schedules
        public async Task<IActionResult> ShowSearchForm()
        {
            return svc.GetSchedule != null ?
                        View() :
                        Problem("Entity set 'ApplicationDbContext.Schedule'  is null.");
        }

         //POST: Scedules/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
        {
            var schedule = svc.GetAllSchedules().Where(x => x.HomeName.Contains(SearchPhrase, StringComparison.OrdinalIgnoreCase) || 
                                                            x.ScheduleNote.Contains(SearchPhrase, StringComparison.OrdinalIgnoreCase));
                        return View ("Index",schedule); 
                
        }

    }
}
