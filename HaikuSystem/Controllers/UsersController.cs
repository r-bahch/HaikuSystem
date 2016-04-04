using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HaikuSystem.DAL;
using HaikuSystem.Models;
using System.Data.SqlClient;
using HaikuSystem.ViewModels;
using PagedList;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HaikuSystem.DTO;

namespace HaikuSystem.Controllers
{
    public class UsersController : Controller
    {
        private EFDbContext db = new EFDbContext();
        HttpClient client;
        //The URL of the WEB API Service
        string url = "http://haikuapi2.azurewebsites.net/api/users";


        //The HttpClient Class, this will be used for performing 
        //HTTP Operations, GET, POST, PUT, DELETE
        //Set the base address and the Header Formatter
        public UsersController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: Users
        public async Task<ActionResult> Index(string currentFilter, int? page, string sortOrder = "name")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.RatingSortParm = sortOrder == "rating" ? "rating_desc" : "rating";

            HttpResponseMessage responseMessage = await client.GetAsync(url + "?sortby=" + sortOrder);
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var users = JsonConvert.DeserializeObject<List<UserDTO>>(responseData);
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(users.ToPagedList(pageNumber, pageSize));
            }


            return View("Error");

        }

        // GET: Users/Details/Username
        public async Task<ActionResult> Details(string username)
        {
            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HttpResponseMessage responseMessage = await client.GetAsync(url + "/" + username);
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var user = JsonConvert.DeserializeObject<UserDTO>(responseData);
                return View(user);
            }


            return View("Error");
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Username,PublishCode")] UserCreateDTO user)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, user);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details", "Users", new { username = user.Username });
                }

                try
                {
                    //serialize error message and add errors to modelstate
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    var error = JsonConvert.DeserializeObject<HttpErrorJSON>(responseData);
                    foreach (var modelErrorArr in error.ModelState)
                    {
                        foreach (var modelError in modelErrorArr.Value)
                        {
                            //remove 
                            ModelState.AddModelError(modelErrorArr.Key.Substring(modelErrorArr.Key.IndexOf('.') + 1), modelError);
                        }
                    }
                    if (!ModelState.IsValid) return View(user);
                }
                catch (Exception)
                {
                    return View("Error");
                }
               
            }
            return View("Error");
        }

       
        // GET: Users/DeleteAll
        public ActionResult DeleteAll(string username)
        {
            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = db.Users.Where(u=>u.Username == username).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(new User { Username = user.Username });
        }

        // POST: Users/DeleteAll
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAllConfirmed(string username, User user)
        {
            User userByName = db.Users.Where(u => u.Username == username).FirstOrDefault();
            if (userByName.PublishCode == user.PublishCode)
            {
                db.Haikus.RemoveRange(db.Haikus.Where(c => c.UserID == userByName.ID));
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("PublishCode", "Wrong publish code!");
            return View(userByName);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
