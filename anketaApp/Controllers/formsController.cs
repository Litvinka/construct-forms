using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using anketaApp.Models;

namespace anketaApp.Controllers
{
    public class formsController : Controller
    {
        private anketaEntities db = new anketaEntities();

        // GET: forms
        public ActionResult Index()
        {
            var form = db.form.Include(f => f.role);
            return View(form.ToList());
        }

        // GET: forms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            form form = db.form.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }

        // GET: forms/Create
        public ActionResult Create()
        {
            ViewBag.id_role = new SelectList(db.role, "id", "name");
            return View();
        }

        // POST: forms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,id_role")] form form)
        {
            if (ModelState.IsValid)
            {
                db.form.Add(form);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_role = new SelectList(db.role, "id", "name", form.id_role);
            return View(form);
        }

        // GET: forms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            form form = db.form.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_role = new SelectList(db.role, "id", "name", form.id_role);
            return View(form);
        }

        // POST: forms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,id_role")] form form)
        {
            if (ModelState.IsValid)
            {
                db.Entry(form).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_role = new SelectList(db.role, "id", "name", form.id_role);
            return View(form);
        }

        // GET: forms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            form form = db.form.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }

        // POST: forms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            form form = db.form.Find(id);
            db.form.Remove(form);
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
