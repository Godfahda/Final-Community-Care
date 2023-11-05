
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using CommunityCare.Core.Models;
using CommunityCare.Core.Services;
using CommunityCare.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using CommunityCare.Core.Security;
using CommunityCare.Data.Services;

/**
 *  User Management Controller providing registration and login functionality
 */
namespace CommunityCare.Web.Controllers
{
    public class UserController : BaseController
    {

        private IUserService svc;

        public UserController()
        {        
            
            svc = new UserServiceDb();
        }

        //GET /user
        public IActionResult Index()
        {
            var users = svc.GetUsers();

            return View(users);
        }

        //GET/ users/details/id
        public IActionResult Details(int id)
        {
            //Retrieve user using ID from the service
            var u = svc.GetUser(id);

            if (u == null)
            {   
                //Display warning alert and redirect to index
                return NotFound();
            }

            //pass user as parameter to the view
            return View(u);
        }

        //GET:/user/create
        public IActionResult Create()
        {
            //display blank form to create a user
            return View();
        }

        //POST/ user/createI (For institutions)
        [HttpPost]
        public IActionResult CreateI(User u)
        {
            //POST action to add user
            if (ModelState.IsValid)
            {
                //pass data to service to store
                svc.AddUserI(u.Name, u.Address, u.PhoneNumber, u.County, u.Email, u.Password,u.CompanyNumber, u.Role, u.Validator);

                return RedirectToAction(nameof(Details), new { Id = u.Id });
            }
            return View(u);
        }

        //POST/user/CreateV (For volunteer)
        [HttpPost]
        public IActionResult CreateV(User u)
        {
            //POST action to add user
            if (ModelState.IsValid)
            {
                //pass data to service to store
                svc.AddUserV(u.Name, u.Surname, u.PhoneNumber, u.County, u.Email, u.Password, u.AccessNI, u.Role, u.Validator);
                
                RedirectToAction(nameof(Details), new { Id = u.Id });
            }
            return View(u);
        }

        //GET/ user/edit/id
        public  IActionResult Edit(int id)
        {
            //load the user using the service
            var u = svc.GetUser(id);

            //Check for user existence
            if (u == null)
            {
                //Alerts and redirect to Index
                Alert($"User does not exist", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            return View(u);
        }

        [HttpPost]
        public IActionResult Edit (int id,[Bind("Id,Name,Surname,PhoneNumber,Address,County,Email,AccessNI,CompanyNumber,Role")] User u)
        {
            if (ModelState.IsValid)
            {
                //pass data to service to update
                svc.UpdateUser(u);

                 Alert("User updated successfully", AlertType.info);

                //Displays alert and redirect to details
                return RedirectToAction(nameof(Details), new { Id = u.Id });
            }

            //redisplay the form for editing as validation errors
            return View(u);
        }

        //GET/user/delete/id
        public IActionResult Delete (int id)
        {
            //load the user using the service
            var u = svc.GetUser(id);
            //Check if user exist
            if (u == null)
            {
                return NotFound();
            }

            //pass user to view for deletion confirmation
            return View(u);
        }

        //POST/user/delete/id
        [HttpPost]
        public IActionResult DeleteConfirm (int id)
        {
            svc.DeleteUser(id);
            Alert("User deleted successfully", AlertType.info);

            return RedirectToAction(nameof(Index));
        }

        // ================User Schedule Management ======================
        //GET/user/createschedule/id
        public IActionResult ScheduleCreate(int id)
        {
            var u = svc.GetUser(id);

            if (u == null)
            {
                Alert($"User does not exist", AlertType.warning);
                return NotFound();
            }

            if (u.Role == Role.volunteer)
            {
                Alert($"Only institutions can create Schedules", AlertType.warning);
                return RedirectToAction(nameof(Details));
            }

            var schedule = new Schedule { UserId = id };
            //Returns blank form
            return View( schedule);
        }

        //POST/user/create
        [HttpPost]
        public IActionResult ScheduleCreate (Schedule s)
        {
            if (ModelState.IsValid)
            {
                var schedule = svc.CreateSchedule(s.UserId, s.ScheduleNote,s.ScheduleTime,s.ResidentName);

                //Displays success alert
                return RedirectToAction(nameof (Details), new {Id = schedule.UserId});

            }
            //display form for editing
            return View(s);
        }

        //GET/user/scheduledelete/id
        public IActionResult ScheduleDelete(int id)
        {
            //load the schedule from service
            var schedule = svc.GetSchedule(id);
            //check schedule to ensure not null
            if (schedule == null)
            {
                Alert($"Schedule not found", AlertType.warning);
                return NotFound();
            }
            return View(schedule);
        }

        [HttpPost]
        public IActionResult ScheduleDeleteConfirm (int id, int userId)
        {
            //delete user via service
            Alert($"Schedule deleted sucessfully", AlertType.success);

            return RedirectToAction(nameof(Details), new {Id = id, userId});
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] UserLoginViewModel m)
        {
            var user = svc.Authenticate(m.Email, m.Password);
            // check if login was unsuccessful and add validation errors
            if (user == null)
            {
                ModelState.AddModelError("Email", "Invalid Login Credentials");
                ModelState.AddModelError("Password", "Invalid Login Credentials");
                return View(m);
            }

            // Login Successful, so sign user in using cookie authentication
           await SignInCookie(user);

            Alert("Successfully Logged in", AlertType.info);

            return Redirect("/");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register([Bind("Name,Surname, Address, PhoneNumber,County,Email,Password,PasswordConfirm,CompanyNumber,AccessNI,Role")] UserRegisterViewModel m)       
        {
            if (!ModelState.IsValid)
            {
                return View(m);
            }
            // add user via service
            var user = svc.AddUser(m.Name, m.Surname, m.Address, m.PhoneNumber, m.County, m.Email,m.Password,m.CompanyNumber, m.AccessNI, m.Role);
            // check if error adding user and display warning
            if (user == null) {
                Alert("There was a problem Registering. Please try again", AlertType.warning);
                return View(m);
            }

            Alert("Successfully Registered. Now login", AlertType.info);

            return RedirectToAction(nameof(Login));
        }

        [Authorize(Roles = "admin")]
        public IActionResult Validate(int id)
        {

            var u = svc.GetUser(id);
            
            if (u == null)
            {
                Alert($"User {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(u);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult ValidateConfirm(int id)
        {
            var u = svc.GetUser(id);

            u.Validator = true;
           
            svc.UpdateUser(u);

            Alert("User validated for successfully", AlertType.info);

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult UpdateProfile()
        {
           // use BaseClass helper method to retrieve Id of signed in user 
            var user = svc.GetUser(User.GetSignedInUserId());
            var userViewModel = new UserProfileViewModel { 
                Id = user.Id, 
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                County = user.County,
                Email = user.Email,
                AccessNI = user.AccessNI,
                CompanyNumber = user.CompanyNumber
            };
            return View(userViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile([Bind("Id,Name,Surname,PhoneNumber,Address,County,Email,AccessNI,CompanyNumber")] UserProfileViewModel m)       
        {
            var user = svc.GetUser(m.Id);
            // check if form is invalid and redisplay
            if (!ModelState.IsValid || user == null)
            {
                return View(m);
            } 

            // update user details and call service
            user.Name = m.Name;
            user.Surname = m.Surname;
            user.PhoneNumber = m.PhoneNumber;
            user.Address = m.Address;
            user.County = m.County;
            user.Email = m.Email;
            user.AccessNI = m.AccessNI;
            user.CompanyNumber = m.CompanyNumber;        
            var updated = svc.UpdateUser(user);

            // check if error updating service
            if (updated == null) {
                Alert("There was a problem Updating. Please try again", AlertType.warning);
                return View(m);
            }

            Alert("Successfully Updated Account Details", AlertType.info);
            
            // sign the user in with updated details)
            await SignInCookie(user);

            return RedirectToAction("Index","Home");
        }

        // Change Password
        [Authorize]
        public IActionResult UpdatePassword()
        {
            // use BaseClass helper method to retrieve Id of signed in user 
            var user = svc.GetUser(User.GetSignedInUserId());
            var passwordViewModel = new UserPasswordViewModel { 
                Id = user.Id, 
                Password = Hasher.CalculateHash(user.Password), 
                PasswordConfirm =Hasher.CalculateHash(user.Password), 
            };
            return View(passwordViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword([Bind("Id,OldPassword,Password,PasswordConfirm")] UserPasswordViewModel m)       
        {
            var user = svc.GetUser(m.Id);
            if (!ModelState.IsValid || user == null)
            {
                return View(m);
            }  
            // update the password
            user.Password =Hasher.CalculateHash(m.Password); 
            // save changes      
            var updated = svc.UpdateUser(user);
            if (updated == null) {
                Alert("There was a problem Updating the password. Please try again", AlertType.warning);
                return View(m);
            }

            Alert("Successfully Updated Password", AlertType.info);
            // sign the user in with updated details
            await SignInCookie(user);

            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // Return not authorised and not authenticated views
        public IActionResult ErrorNotAuthorised() => View();
        public IActionResult ErrorNotAuthenticated() => View();

        // -------------------------- Helper Methods ------------------------------

        // Called by Remote Validation attribute on RegisterViewModel to verify email address is available
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEmailAvailable(string email, int id)
        {
            // check if email is available, or owned by user with id 
            if (!svc.IsEmailAvailable(email,id))
            {
                return Json($"A user with this email address {email} already exists.");
            }
            return Json(true);                  
        }

        // Called by Remote Validation attribute on ChangePassword to verify old password
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyPassword(string oldPassword)
        {
            // use BaseClass helper method to retrieve Id of signed in user 
            var id = User.GetSignedInUserId();            
            // check if email is available, unless already owned by user with id
            var user = svc.GetUser(id);
            if (user == null || !Hasher.ValidateHash(user.Password, oldPassword))
            {
                return Json($"Please enter current password.");
            }
            return Json(true);                  
        }

        // Sign user in using Cookie authentication scheme
        private async Task SignInCookie(User user)
        {
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                AuthBuilder.BuildClaimsPrincipal(user)
            );
        }
    }
}