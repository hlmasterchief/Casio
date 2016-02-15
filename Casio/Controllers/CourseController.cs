using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Casio.Models;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;

namespace Casio.Controllers
{
    public class CourseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Course
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var courses = db.Courses.Include(c => c.Teacher);
                return View(courses.ToList());
            }

            if (User.IsInRole("Teacher"))
            {
                string userId = User.Identity.GetUserId();
                var courses = db.Courses.Include(c => c.Teacher)
                    .Where(p => p.TeacherId == userId);
                return View(courses.ToList());
            }

            if (User.IsInRole("Student"))
            {
                string userId = User.Identity.GetUserId();
                var enrollments = db.Enrollments.Where(p => p.StudentId == userId && p.Grade > 0).ToList();

                List<Course> courses = new List<Course>();
                foreach (Enrollment enrollment in enrollments)
                {
                    Course course = db.Courses.FirstOrDefault(c => c.Id == enrollment.CourseId);
                    if (course != null)
                    {
                        courses.Add(course);
                    }
                }

                return View(courses);
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Course/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Teacher"))
            {
                course.Enrollments = db.Enrollments.Where(p => p.CourseId == id).ToList();
            }

            if (User.IsInRole("Student"))
            {
                string userId = User.Identity.GetUserId();
                course.Enrollments = db.Enrollments.Where(p => p.CourseId == id && p.StudentId == userId).ToList();
            }
            return View(course);
        }

        // GET: Course/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Teacher"))
            {
                string userId = User.Identity.GetUserId();
                ViewBag.TeacherId = new SelectList(db.Teachers.Where(p => p.Id == userId), "Id", "Name");
            } else
            {
                ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "Name");
            }
            return View();
        }

        // POST: Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,Credit,Description,TeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "Name", course.TeacherId);
            return View(course);
        }

        // GET: Course/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "Name", course.TeacherId);
            return View(course);
        }

        // POST: Course/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Credit,Description,TeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "Name", course.TeacherId);
            return View(course);
        }

        // GET: Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
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
