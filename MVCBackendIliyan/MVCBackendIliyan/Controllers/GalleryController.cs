using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCBackendIliyan.Context;
using MVCBackendIliyan.Models;
using PagedList;

namespace MVCBackendIliyan.Controllers
{
    public class GalleryController : Controller
    {
        private MVCBackendDb db = new MVCBackendDb();
        private const int PageSize = 12;

        // GET: Gallery
        public ActionResult Index(string message, string sortBy, int? page)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }
            var galleries = db.Galleries.OrderByDescending(x => x.Time).Include(g => g.User).AsQueryable();

            if (Request.IsAuthenticated)
            {
                ViewBag.UserId = db.Users.Where(x => x.Email.Contains(User.Identity.Name)).FirstOrDefault().Id;
            }

            ViewBag.AddedPhoto = String.IsNullOrEmpty(sortBy) ? "AddedPhoto" : "";


            switch (sortBy)
            {
                case "AddedPhoto":
                    galleries = galleries.OrderBy(b => b.Time);
                    break;
                default:
                    galleries = galleries.OrderByDescending(b => b.Time);
                    break;
            }


            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            return View(galleries.ToPagedList(pageIndex, PageSize));
        }
        [HttpGet, ActionName("PartialIndex")]
        public ActionResult _PartialIndex(string message, string searchBy, string sortBy, int? page)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }
            if (Request.IsAuthenticated)
            {
                ViewBag.UserId = db.Users.Where(x => x.Email.Contains(User.Identity.Name)).FirstOrDefault().Id;
            }

            var galleries = db.Galleries.OrderByDescending(x => x.Time).Include(g => g.User).AsQueryable();

            galleries = galleries.Where(x => searchBy == null || searchBy == "" || x.Title.TrimStart().ToUpper().StartsWith(searchBy.TrimStart().ToUpper()) || x.User.Email.TrimStart().ToUpper().StartsWith(searchBy.TrimStart().ToUpper()));
            ViewBag.AddedPhoto = String.IsNullOrEmpty(sortBy) ? "AddedPhoto" : "";

            switch (sortBy)
            {
                case "AddedPhoto":
                    galleries = galleries.OrderBy(b => b.Time);
                    break;
                default:
                    galleries = galleries.OrderByDescending(b => b.Time);
                    break;
            }

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            return PartialView("_PartialIndex", galleries.ToPagedList(pageIndex, PageSize));
        }

        public JsonResult GetGalleryStuff(string term)
        {
            var searchFirstNameAll = db.Users.Where(x => x.Email.TrimStart().ToUpper().StartsWith(term.TrimStart().ToUpper()))
           .Select(s => s.Email).Distinct().ToList();
            var searchTitle = db.Galleries.Where(x => x.Title.TrimStart().ToUpper().StartsWith(term.TrimStart().ToUpper()))
           .Select(s => s.Title).Distinct().ToList();
            searchFirstNameAll = searchFirstNameAll.Concat(searchTitle).ToList();
            return Json(searchFirstNameAll, JsonRequestBehavior.AllowGet);
        }


        // GET: Gallery
        public ActionResult Download(string message, string sortBy, int? page)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }
            var galleries = db.Galleries.OrderByDescending(x => x.Time).Include(g => g.User).AsQueryable();

            if (Request.IsAuthenticated)
            {
                ViewBag.UserId = db.Users.Where(x => x.Email.Contains(User.Identity.Name)).FirstOrDefault().Id;
            }

            ViewBag.AddedPhoto = String.IsNullOrEmpty(sortBy) ? "AddedPhoto" : "";


            switch (sortBy)
            {
                case "AddedPhoto":
                    galleries = galleries.OrderBy(b => b.Time);
                    break;
                default:
                    galleries = galleries.OrderByDescending(b => b.Time);
                    break;
            }


            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            return View(galleries.ToPagedList(pageIndex, PageSize));
        }
        [HttpGet, ActionName("PartialDownload")]
        public ActionResult _PartialDownload(string message, string searchBy, string sortBy, int? page)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }
            if (Request.IsAuthenticated)
            {
                ViewBag.UserId = db.Users.Where(x => x.Email.Contains(User.Identity.Name)).FirstOrDefault().Id;
            }

            var galleries = db.Galleries.OrderByDescending(x => x.Time).Include(g => g.User).AsQueryable();

            galleries = galleries.Where(x => searchBy == null || searchBy == "" || x.Title.TrimStart().ToUpper().StartsWith(searchBy.TrimStart().ToUpper()) || x.User.Email.TrimStart().ToUpper().StartsWith(searchBy.TrimStart().ToUpper()));
            ViewBag.AddedPhoto = String.IsNullOrEmpty(sortBy) ? "AddedPhoto" : "";

            switch (sortBy)
            {
                case "AddedPhoto":
                    galleries = galleries.OrderBy(b => b.Time);
                    break;
                default:
                    galleries = galleries.OrderByDescending(b => b.Time);
                    break;
            }

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            return PartialView("_PartialDownload", galleries.ToPagedList(pageIndex, PageSize));
        }

        // GET: Gallery/Create
        public ActionResult Create()
        {
            return View();
        }
        [ActionName("PartialCreate")]
        public ActionResult _PartialCreate()
        {
            return PartialView("_PartialCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Gallery image, HttpPostedFileBase file)
        {
            if (file == null)
            {
                TempData["image"] = "Enter a photo!";
                return View("Create");
            }
            int userId = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault().Id;
            string filename = null;
            if (ModelState.IsValid)
            {
                string time = DateTime.Now.Year.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Minute.ToString()
                 + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
                filename = time + "_" + Path.GetFileName(file.FileName);
                file.SaveAs(Server.MapPath("~/Images/" + filename));
                image.UserId = userId;
                image.Time = DateTime.Now;
                image.ImageUrl = filename;
                db.Galleries.Add(image);
                db.SaveChanges();
                return RedirectToAction("Index", new { message = "Added Photo." });
            }
            TempData["image"] = "Enter a photo!";
            return View();
        }

        // POST: Gallery/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Gallery gallery = db.Galleries.Find(id);
            var fullPath = Server.MapPath("~/Images/" + gallery.ImageUrl);
            if (System.IO.File.Exists(fullPath))
            {

                System.IO.File.Delete(fullPath);
            }
            db.Galleries.Remove(gallery);
            db.SaveChanges();
            return RedirectToAction("PartialIndex", new { message = "Removed Photo." });
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
