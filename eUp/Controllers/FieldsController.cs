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
        //instance of a database
        private eUpDbContext context = new eUpDbContext();

        //default action, returns a list of fields
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

        //return a View to create a new form field along with a dropbox to select a field type
        // GET: /Fields/Create
        public ActionResult Create(int id)
        {
            ViewBag.UserTableId = id;
            Field f = new Field();
            f.UserTableId = id;
            //create a list of items to be displayed in a dropdown box when creating new field
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Text", Value = "STRING", Selected = true });
            items.Add(new SelectListItem { Text = "Numeric", Value = "INTEGER" });
            items.Add(new SelectListItem { Text = "True/False", Value = "BOOL" });
            //assign that list to a ViewBag so that it can be accessed from a View
            ViewBag.fieldTypes = items;
            //pass an empty form field to a View
            return View(f);
        } 

        //collects all values from a View and binds them to a Field model
        // POST: /Fields/Create
        [HttpPost]
        public ActionResult Create(Field field)
        {
            if (ModelState.IsValid)
            {
                //add a field to a database and save it
                context.Fields.Add(field);
                context.SaveChanges();
                //create another field
                return RedirectToAction("Create");  
            }
            return View(field);
        }
        
        //METHOD NOT USED
        //field can be adited using UserTablesController Edit method
        // GET: /Fields/Edit/5
        public ActionResult Edit(int id)
        {
            Field field = context.Fields.Single(x => x.FieldId == id);
            ViewBag.PossibleUserTables = context.UserTables;
            return View(field);
        }

        ////METHOD NOT USED
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

        ////METHOD NOT USED
        // GET: /Fields/Delete/5
        public ActionResult Delete(int id)
        {
            Field field = context.Fields.Single(x => x.FieldId == id);
            return View(field);
        }

        ////METHOD NOT USED
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