using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P50_1_19.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace P50_1_19.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext db;
        public AccountController(ApplicationContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(user => user.Email == model.Email);
                if (user == null)
                {
                    user = new User { 
                        Email = model.Email,
                        Login = model.Login,
                        Password = model.Password
                    };

                    Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                        user.Role = userRole;

                    db.Users.Add(user);


                    await db.SaveChangesAsync();
                    return RedirectToAction("PersonalPage", "Home");
                    //return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Пользователь с таким Email уже существует");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();
        
        [HttpGet]
        public IActionResult ForUsers() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(user => user.Email == model.Email && user.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user); // аутентификация
                    if (user.RoleId == 2)
                    {
                        return RedirectToAction("PersonalPage", "Home");
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Такого пользователя не существует");
            }
            return View(model);
        }

        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
