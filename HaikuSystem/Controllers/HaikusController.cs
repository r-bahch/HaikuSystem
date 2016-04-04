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
using HaikuSystem.ViewModels;
using System.Data.Entity.Validation;
using System.Diagnostics;
using PagedList;

namespace HaikuSystem.Controllers
{
    public class HaikusController : Controller
    {
        private EFDbContext db = new EFDbContext();

        // GET: Haikus
        public ActionResult Index(string sortOrder, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.RatingSortParm = string.IsNullOrEmpty(sortOrder) ? "rating_asc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_asc" : "Date";

            var haikus = from s in db.Haikus
                         select s;
            switch (sortOrder)
            {
                case "rating_asc":
                    haikus = haikus.OrderBy(s => s.Ratings.Average(r => r.Value));
                    break;
                case "Date":
                    haikus = haikus.OrderByDescending(s => s.SubmissionDate);
                    break;
                case "date_asc":
                    haikus = haikus.OrderBy(s => s.SubmissionDate);
                    break;
                default:
                    haikus = haikus.OrderByDescending(s => s.Ratings.Average(r => r.Value));
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(haikus.ToPagedList(pageNumber, pageSize));
        }

        // GET: Haikus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Haiku haiku = db.Haikus.Find(id);
            if (haiku == null)
            {
                return HttpNotFound();
            }
            return View(haiku);
        }

        // GET: Haikus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Haikus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TheHaiku,AuthorPublishCode")] HaikuEditVM viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    viewModel.TheHaiku.UserID = db.Users.Where(u => u.PublishCode == viewModel.AuthorPublishCode).First().ID;
                }
                catch (InvalidOperationException)
                {
                    //ModelState.Clear();
                    ModelState.AddModelError("AuthorPublishCode", "Publish code not registered!");
                    return View(viewModel);
                }

                viewModel.TheHaiku.SubmissionDate = DateTime.Now;
                db.Haikus.Add(viewModel.TheHaiku);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        // GET: Haikus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Haiku haiku = db.Haikus.Find(id);
            if (haiku == null)
            {
                return HttpNotFound();
            }
            HaikuEditVM viewModel = new HaikuEditVM { TheHaiku = haiku };
            return View(viewModel);
        }

        // POST: Haikus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(HaikuEditVM viewModel, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var haikuToUpdate = db.Haikus.Find(id);

            if (haikuToUpdate.User.PublishCode == viewModel.AuthorPublishCode)
            {
                haikuToUpdate.Text = viewModel.TheHaiku.Text;
                haikuToUpdate.SubmissionDate = DateTime.Now;
                db.Ratings.RemoveRange(db.Ratings.Where(c => c.HaikuID == haikuToUpdate.ID));
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
                return View(haikuToUpdate);
            }
            ModelState.AddModelError("AuthorPublishCode", "Wrong publish code!");
            return View(viewModel);
        }

        // GET: Haikus/Rate/5
        public ActionResult Rate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Haiku haiku = db.Haikus.Find(id);
            if (haiku == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rate([Bind(Include = "ID,Value")] Rating rating, int? id)
        {
            if (ModelState.IsValid)
            {
                rating.HaikuID = (int)id;
                db.Ratings.Add(rating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.HaikuID = new SelectList(db.Haikus, "ID", "Text", rating.HaikuID);
            return View(rating);
        }

        // GET: Haikus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Haiku haiku = db.Haikus.Find(id);
            if (haiku == null)
            {
                return HttpNotFound();
            }
            HaikuEditVM viewModel = new HaikuEditVM { TheHaiku = haiku };
            return View(viewModel);
        }

        // POST: Haikus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HaikuEditVM viewModel, int id)
        {

            Haiku haiku = db.Haikus.Find(id);
            if (haiku.User.PublishCode == viewModel.AuthorPublishCode)
            {
                db.Haikus.Remove(haiku);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("AuthorPublishCode", "Wrong publish code!");
            return View(viewModel);
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
