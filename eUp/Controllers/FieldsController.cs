using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eUp.Models;

namespace eUp.Controllers
{   
    public class FieldsController : Controller
    {
        private eUpDbContext context = new eUpDbContext();

        //
        // GET: /Fields/

        public ViewResult Index()
        {
            return View(context.Fields.ToList());
        }

        //
        // GET: /Fields/Details/5

        public ViewResult Details(int id)
        {
            Field field = context.Fields.Single(x => x.FieldId == id);
            return View(field);
        }

        //
        // GET: /Fields/Create

        public ActionResult Create(int id)
        {
           // ViewBag.PossibleUserTables = context.UserTables;
            ViewBag.TableId = id;
            Field f = new Field();
            f.UserTableId = id;
            return View(f);
        } 

        //
        // POST: /Fields/Create

        [HttpPost]
        public ActionResult Create(Field field)
        {
            if (ModelState.IsValid)
            {

               // field.UserTableId = 16;
                context.Fields.Add(field);
                context.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.PossibleUserTables = context.UserTables;
            return View(field);
        }
        
        //
        // GET: /Fields/Edit/5
 
        public ActionResult Edit(int id)
        {
            Field field = context.Fields.Single(x => x.FieldId == id);
            ViewBag.PossibleUserTables = context.UserTables;
            return View(field);
        }

        //
        // POST: /Fields/Edit/5

        [HttpPost]
        public ActionResult Edit(Field field)
        {
            if (ModelState.IsValid)
            {
                context.Entry(field).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PossibleUserTables = context.UserTables;
            return View(field);
        }

        //
        // GET: /Fields/Delete/5
 
        public ActionResult Delete(int id)
        {
            Field field = context.Fields.Single(x => x.FieldId == id);
            return View(field);
        }

        //
        // POST: /Fields/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Field field = context.Fields.Single(x => x.FieldId == id);
            context.Fields.Remove(field);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}