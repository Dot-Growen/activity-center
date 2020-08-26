using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc; // For Password Hashing
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // For Session
using belt_exam.Models;

namespace belt_exam.Controllers {
    public class HomeController : Controller {

        //*********** CONTEXT
        private MyContext _context;

        public HomeController (MyContext context) {
            _context = context;
        }

        //*********** GET Request
        public IActionResult Index () {
            return View ();
        }

        [HttpGet ("loginpage")]
        public ViewResult LoginPage () {
            return View ();
        }

        [HttpGet ("home")]
        public IActionResult Home () {
            if (HttpContext.Session.GetInt32 ("UserId") == null) {
                return RedirectToAction ("loginpage");
            } else {
                ViewBag.User = _context.Users.FirstOrDefault (l => l.UserId == HttpContext.Session.GetInt32 ("UserId"));
                List<Activity> allActivities = _context.Activities
                    .Include (a => a.Participants)
                    .ThenInclude (a => a.UsersActivity)
                    .OrderBy (a => a.Date)
                    .Where(a => a.Date > DateTime.Now)
                    .ToList ();
                Console.WriteLine ($"I AM logged in. My Id => {HttpContext.Session.GetInt32 ("UserId")}");
                return View (allActivities);
            }
        }

        [HttpGet ("new")]
        public IActionResult New () {
            ViewBag.User = _context.Users.SingleOrDefault (u => u.UserId == HttpContext.Session.GetInt32 ("UserId"));
            return View ();
        }

        [HttpGet ("activity/{Id}")]
        public IActionResult ActivityView (int Id) {
            ViewBag.ViewUser = _context.Users
                .Include (u => u.JoinedActivites)
                .ThenInclude (u => u.ActivityUsers)
                .SingleOrDefault (u => u.UserId == HttpContext.Session.GetInt32 ("UserId"));
            ViewBag.ViewAct = _context.Activities
                .Where (a => a.ActivityId == Id)
                .SingleOrDefault ();
            List<User> allUsers = _context.Users
                .Include (u => u.JoinedActivites)
                .ThenInclude (u => u.ActivityUsers)
                .ToList ();
            return View (allUsers);
        }

        [HttpGet ("logout")]
        public IActionResult Logout () {
            Console.WriteLine ($"I WAS login. My Id => {HttpContext.Session.GetInt32 ("UserId")}");
            HttpContext.Session.Clear ();
            Console.WriteLine ($"NOW IM out. Id => {HttpContext.Session.GetInt32 ("UserId")}");
            return View ("loginpage");
        }

        [HttpGet ("delete/{Id}")]
        public IActionResult Delete (int Id) {
            Activity getActivity = _context.Activities.SingleOrDefault (a => a.ActivityId == Id);
            _context.Activities.Remove (getActivity);
            _context.SaveChanges ();
            return RedirectToAction ("home");
        }

        [HttpGet ("join/{actId}/{userId}")]
        public IActionResult Join (int actId, int userId) {
            User ViewUser = _context.Users
                .Include (u => u.JoinedActivites)
                .ThenInclude (u => u.ActivityUsers)
                .SingleOrDefault (u => u.UserId == userId);
            Activity ViewAct = _context.Activities
                .Include (a => a.Participants)
                .ThenInclude (a => a.UsersActivity)
                .SingleOrDefault (u => u.ActivityId == actId);
            if (ViewUser.JoinedActivites.All (u => u.ActivityId != ViewAct.ActivityId)) {
                Association newAssociation = new Association ();
                newAssociation.ActivityId = actId;
                newAssociation.UserId = userId;
                _context.Associations.Add (newAssociation);
                _context.SaveChanges ();
                return RedirectToAction ("home");
            } else {
                return RedirectToAction ("home");
            }

        }

        [HttpGet ("leave/{actId}/{userId}")]
        public IActionResult leave (int actId, int userId) {
            Association leaving = _context.Associations.FirstOrDefault (a => a.ActivityId == actId && a.UserId == userId);
            _context.Remove (leaving);
            _context.SaveChanges ();
            return RedirectToAction ("home");
        }

        //*********** POST Request

        [HttpPost ("login")]
        public IActionResult Login (LoginUser log) {
            if (ModelState.IsValid) {
                User userInDb = _context.Users.FirstOrDefault (u => u.Email == log.LoginEmail);
                Console.WriteLine (userInDb);
                if (userInDb == null) {
                    ModelState.AddModelError ("LoginEmail", "Invalid Email/Password");
                    return View ("loginpage");
                } else {
                    var hasher = new PasswordHasher<LoginUser> ();
                    var result = hasher.VerifyHashedPassword (log, userInDb.Password, log.LoginPassword);
                    if (result == 0) {
                        ModelState.AddModelError ("LoginEmail", "Invalid Email/Password");
                        return View ("loginpage");
                    } else {
                        HttpContext.Session.SetInt32 ("UserId", userInDb.UserId);
                        return RedirectToAction ("home");
                    }
                }
            } else {
                Console.WriteLine (log.LoginEmail);
                return View ("loginpage");
            }
        }

        [HttpPost ("register")]
        public IActionResult Register (User user) {
            if (ModelState.IsValid) {
                if (_context.Users.Any (u => u.Email == user.Email)) {
                    ModelState.AddModelError ("Email", "Email already in use!");
                    return View ("Index");
                } else {
                    PasswordHasher<User> Hasher = new PasswordHasher<User> ();
                    user.Password = Hasher.HashPassword (user, user.Password);
                    _context.Users.Add (user);
                    _context.SaveChanges ();
                    HttpContext.Session.SetInt32 ("UserId", user.UserId);
                    Console.WriteLine ($"User id: {user.UserId}\nFirst Name: {user.FirstName}\nLastName: {user.LastName}\nEmail: {user.Email}\nSessionId: {HttpContext.Session.GetInt32("UserId")}");
                    return RedirectToAction ("home");
                }
            } else {
                return View ("Index");
            }
        }

        [HttpPost ("addactivity")]
        public IActionResult AddActivity (Activity newActivity) {

                User NewCreator = _context.Users
                .SingleOrDefault (u => u.UserId == HttpContext.Session.GetInt32 ("UserId"));
            if (ModelState.IsValid) {
                newActivity.Creator = NewCreator.FirstName;
                _context.Activities.Add (newActivity);
                _context.SaveChanges ();
                Console.WriteLine (newActivity.Title);
                return Redirect ($"activity/{newActivity.ActivityId}");
            } else {
                ModelState.AddModelError ("Date", "Sorry no time travel");
                return View ("New");
            }
        }

    }
}