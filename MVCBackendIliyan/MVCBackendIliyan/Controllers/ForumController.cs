using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCBackendIliyan.Context;
using MVCBackendIliyan.Models;
using PagedList;

namespace MVCBackendIliyan.Controllers
{
    public class ForumController : Controller
    {
        private MVCBackendDb db = new MVCBackendDb();
        private const int PageSize = 7;

        // GET: Forum
        public ActionResult Index(string sortBy, int? page)
        {
            if(Request.IsAuthenticated)
            {
                ViewBag.UserId = db.Users.Where(x => x.Email.Contains(User.Identity.Name)).FirstOrDefault().Id;
            }
            var forums = db.Forums.OrderByDescending(x => x.Time).Include(f => f.User).AsQueryable();

            ViewBag.AddedComment = String.IsNullOrEmpty(sortBy) ? "AddedComment" : "";


            switch (sortBy)
            {
                case "AddedComment":
                    forums = forums.OrderBy(b => b.Time);
                    break;
                default:
                    forums = forums.OrderByDescending(b => b.Time);
                    break;
            }

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            return View(forums.ToPagedList(pageIndex, PageSize));
        }
        [HttpGet, ActionName("PartialIndex")]
        public ActionResult _PartialIndex(string searchBy, string sortBy, int? page)
        {
            if (Request.IsAuthenticated)
            {
                ViewBag.UserId = db.Users.Where(x => x.Email.Contains(User.Identity.Name)).FirstOrDefault().Id;
            }
            var forums = db.Forums.OrderByDescending(x => x.Time).Include(f => f.User).AsQueryable();

            forums = forums.Where(x => searchBy == null || searchBy == "" || x.Comment.TrimStart().ToUpper().StartsWith(searchBy.TrimStart().ToUpper()) ||
          x.User.FirstName.TrimStart().ToUpper().StartsWith(searchBy.TrimStart().ToUpper()) ||
          x.User.LastName.TrimStart().ToUpper().StartsWith(searchBy.TrimStart().ToUpper()) ||
          x.User.Email.TrimStart().ToUpper().StartsWith(searchBy.TrimStart().ToUpper()) ||
          x.User.Genter.TrimStart().ToUpper().StartsWith(searchBy.TrimStart().ToUpper()));

            ViewBag.AddedComment = String.IsNullOrEmpty(sortBy) ? "AddedComment" : "";

            switch (sortBy)
            {
                case "AddedComment":
                    forums = forums.OrderBy(b => b.Time);
                    break;
                default:
                    forums = forums.OrderByDescending(b => b.Time);
                    break;
            }

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            return PartialView("_PartialIndex", forums.ToPagedList(pageIndex, PageSize));
        }

        public JsonResult GetForumStuff(string term)
        {
            var searchFirstNameAll = db.Users.Where(x => x.FirstName.TrimStart().ToUpper().StartsWith(term.TrimStart().ToUpper()))
           .Select(s => s.FirstName).Distinct().ToList();
            var searchLastName = db.Users.Where(x => x.LastName.TrimStart().ToUpper().StartsWith(term.TrimStart().ToUpper()))
            .Select(s => s.LastName).Distinct().ToList();
            var searchEmail = db.Users.Where(x => x.Email.TrimStart().ToUpper().StartsWith(term.TrimStart().ToUpper()))
            .Select(s => s.Email).Distinct().ToList();
            var searchComment = db.Forums.Where(x => x.Comment.TrimStart().ToUpper().StartsWith(term.TrimStart().ToUpper()))
           .Select(s => s.Comment).Distinct().ToList();
            searchEmail = searchEmail.Concat(searchComment).ToList();
            searchLastName = searchLastName.Concat(searchEmail).ToList();
            searchFirstNameAll = searchFirstNameAll.Concat(searchLastName).ToList();
            return Json(searchFirstNameAll, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddComment(string text)
        {
            var user = db.Users.Where(x => x.Email.Contains(User.Identity.Name)).FirstOrDefault();
            var forum = new Forum();
            forum.UserId = user.Id;
            forum.Comment = text;
            forum.Time = DateTime.Now;
            db.Forums.Add(forum);
            db.SaveChanges();
            TempData["message"] = "Added Comment.";
            return RedirectToAction("PartialIndex");
        }

        // GET: Forum/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forum forum = db.Forums.Find(id);
            if (forum == null)
            {
                return HttpNotFound();
            }
            return View(forum);
        }

        [HttpGet, ActionName("PartialEdit")]
        public ActionResult _PartialEdit(int id)
        {
            Forum forum = db.Forums.Find(id);

            return PartialView("_PartialEdit", forum);
        }

        // POST: Forum/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(int id, string text)
        {
            Forum forum = db.Forums.Find(id);
            forum.Comment = text;
            forum.Time = DateTime.Now;
            db.SaveChanges();
            TempData["message"] = "Comment Changed.";
            return RedirectToAction("PartialIndex");
        }

        // POST: Forum/Delete/5
        [HttpPost]
        public ActionResult DeleteComment(int id)
        {
            Forum forum = db.Forums.Find(id);
            db.Forums.Remove(forum);
            db.SaveChanges();
            TempData["message"] = "Removed Comment.";
            return RedirectToAction("PartialIndex");
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
