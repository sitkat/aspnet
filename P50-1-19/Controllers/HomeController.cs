using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P50_1_19.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using P50_1_19.Entities;
using System.Dynamic;

namespace P50_1_19.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        private IWebHostEnvironment _app;
        public HomeController(ApplicationContext context , IWebHostEnvironment app)
        {
            db = context;
            _app = app;
        }
        public IActionResult addFile()
        {
            return View(db.FileModels.ToList());
        }
        [HttpPost]
        public async Task<IActionResult> addFile(IFormFile file)
        {
            if (file != null)
            {
                string path = "/Files/" + file.FileName;
                using(FileStream fileStream = new FileStream(_app.WebRootPath+path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                FileModel fileModel = new FileModel
                {
                    Name = file.FileName,
                    Path = path
                };
                db.FileModels.Add(fileModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("AddFile");
        }

        public async Task<IActionResult> Index(int? id, string login, int page = 1, SortState sortOrder = SortState.IdAsc)
        {
            IQueryable<User> users = db.Users;
            // Фильтрация или поиск
            if (id > 0 && id != null)
            {
                users = users.Where(p => p.Id == id);
            }
            if (!String.IsNullOrWhiteSpace(login))
            {
                users = users.Where(p => p.Login.Contains(login));
            }

            // Сортировка
            switch(sortOrder)
            {
                case SortState.IdAsc:
                    {
                        users = users.OrderBy(p => p.Id);
                        break;
                    }
                case SortState.IdDesc:
                    {
                        users = users.OrderByDescending(p => p.Id);
                        break;
                    }
                case SortState.LoginAsc:
                    {
                        users = users.OrderBy(p => p.Login);
                        break;
                    }
                case SortState.LoginDesc:
                    {
                        users = users.OrderByDescending(p => p.Login);
                        break;
                    }
            }
            // Пагинация
            int pageSize = 5;
            var count = await users.CountAsync();
            var item = await users.Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync();

            IndexViewModel indexViewModel = new IndexViewModel()
            {
                FilterViewModel = new FilterViewModel(id, login),
                SortViewModel = new SortViewModel(sortOrder),
                PageViewModel = new PageViewModel(count, page, pageSize),
                Users = item
            };
            return View(indexViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfinDelete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(predicate =>
                    predicate.Id == id);
                if (user != null)
                {
                    return View(user);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(predicate =>
                    predicate.Id == id);
                if (user != null)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(predicate =>
                    predicate.Id == id);
                if (user != null)
                {
                    return View(user);
                }
            }
            return NotFound();
        }

        public IActionResult ForUsers()
        {
            List<Post> posts = db.Posts.ToList();
            return View(posts);
        }

        public IActionResult PersonalPage()
        {
            dynamic mymodel = new ExpandoObject();

            var posts = db.Posts.ToList().FindAll(p => p.author == User.Identity.Name);

            mymodel.Posts = posts;
            User user = db.Users.ToList().Find(u => u.Login == User.Identity.Name);
            mymodel.User = user;
            return View(mymodel);
        }

        [HttpGet("/create")]
        public IActionResult CreatePost()
        {
            return View();
        }

        [HttpPost("/create")]
        public async Task<IActionResult> CreatePost(PostDTO newPost)
        {
            Post post = new Post();
            post.body = newPost.body;
            post.title = newPost.title;
            //User user = db.Users.FirstOrDefault(user => user.Login == User.Identity.Name);
            User user = await db.Users.FirstOrDefaultAsync(user => user.Login == User.Identity.Name);
            post.author = user.Login;


            db.Posts.Add(post);
            db.SaveChanges();

            return RedirectToAction("PersonalPage");
        }




        [HttpGet]
        [ActionName("DeletePost")]
        public async Task<IActionResult> ConfirmDeletePost(int? id)
        {
            if (id != null)
            {
                Post post = await db.Posts.FirstOrDefaultAsync(predicate =>
                    predicate.Id == id);
                if (post != null)
                {
                    return View(post);
                }
            }
            return NotFound();
            //return RedirectToAction("PersonalPage");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id != null)
            {
                //Post post = await db.Posts.FirstOrDefaultAsync(predicate =>
                //    predicate.Id == id);
                //if (post != null)
                //{
                //    db.Posts.Remove(post);
                //    await db.SaveChangesAsync();
                //    return RedirectToAction("PersonalPage");
                //}
                Post post = new Post { Id = id.Value };
                db.Entry(post).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("PersonalPage");
                //return View();
            }
            return NotFound();
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(predicate =>
                    predicate.Id == id);
                if (user != null)
                {
                    return View(user);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin, user")]
        public IActionResult R()
        {
            if (User.IsInRole("user"))
            {
                return View("ForUsers");
            }
            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            return Content($"ваша роль: {role}");
        }

        [Authorize(Roles = "admin")]
        public IActionResult A()
        {
            return Content("Вход только для администратора");
        }
    }
}
