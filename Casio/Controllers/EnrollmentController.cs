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
    public class EnrollmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Enrollment
        public ActionResult Index()
        {
            var enrollments = db.Enrollments.Include(e => e.Course).Include(e => e.Student);
            string userId = User.Identity.GetUserId();
            if (User.IsInRole("Teacher"))
            {
                enrollments = enrollments.Where(p => p.Course.TeacherId == userId);
            }
            if (User.IsInRole("Student"))
            {
                enrollments = enrollments.Where(p => p.StudentId == userId);
            }
            return View(enrollments.ToList());
        }

        // GET: Enrollment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // GET: Enrollment/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Teacher"))
            {
                string userId = User.Identity.GetUserId();
                ViewBag.CourseId = new SelectList(db.Courses.Where(p => p.TeacherId == userId), "Id", "FullName");
            } else
            {
                ViewBag.CourseId = new SelectList(db.Courses, "Id", "FullName");
            }
            if (User.IsInRole("Student"))
            {
                string userId = User.Identity.GetUserId();
                ViewBag.StudentId = new SelectList(db.Students.Where(p => p.Id == userId), "Id", "FullName");
            }
            else
            {
                ViewBag.StudentId = new SelectList(db.Students, "Id", "FullName");
            }
            return View();
        }

        // POST: Enrollment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StudentId,CourseId,Grade")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                enrollment.DateModified = DateTime.Now; //set date
                db.Enrollments.Add(enrollment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (User.IsInRole("Teacher"))
            {
                string userId = User.Identity.GetUserId();
                ViewBag.CourseId = new SelectList(db.Courses.Where(p => p.TeacherId == userId), "Id", "FullName");
            }
            else
            {
                ViewBag.CourseId = new SelectList(db.Courses, "Id", "FullName");
            }
            if (User.IsInRole("Student"))
            {
                string userId = User.Identity.GetUserId();
                ViewBag.StudentId = new SelectList(db.Students.Where(p => p.Id == userId), "Id", "FullName");
            }
            else
            {
                ViewBag.StudentId = new SelectList(db.Students, "Id", "FullName");
            }
            return View(enrollment);
        }

        // GET: Enrollment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("Teacher"))
            {
                string userId = User.Identity.GetUserId();
                ViewBag.CourseId = new SelectList(db.Courses.Where(p => p.TeacherId == userId), "Id", "FullName");
            }
            else
            {
                ViewBag.CourseId = new SelectList(db.Courses, "Id", "FullName");
            }
            if (User.IsInRole("Student"))
            {
                string userId = User.Identity.GetUserId();
                ViewBag.StudentId = new SelectList(db.Students.Where(p => p.Id == userId), "Id", "FullName");
            }
            else
            {
                ViewBag.StudentId = new SelectList(db.Students, "Id", "FullName");
            }
            return View(enrollment);
        }

        // POST: Enrollment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StudentId,CourseId,Grade")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                enrollment.DateModified = DateTime.Now; //change date
                db.Entry(enrollment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (User.IsInRole("Teacher"))
            {
                string userId = User.Identity.GetUserId();
                ViewBag.CourseId = new SelectList(db.Courses.Where(p => p.TeacherId == userId), "Id", "FullName");
            }
            else
            {
                ViewBag.CourseId = new SelectList(db.Courses, "Id", "FullName");
            }
            if (User.IsInRole("Student"))
            {
                string userId = User.Identity.GetUserId();
                ViewBag.StudentId = new SelectList(db.Students.Where(p => p.Id == userId), "Id", "FullName");
            }
            else
            {
                ViewBag.StudentId = new SelectList(db.Students, "Id", "FullName");
            }
            return View(enrollment);
        }

        // GET: Enrollment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // POST: Enrollment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Enrollment enrollment = db.Enrollments.Find(id);
            db.Enrollments.Remove(enrollment);
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
