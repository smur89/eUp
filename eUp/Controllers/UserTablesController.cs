using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eUp.Models;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Collections.ObjectModel;
using System.Collections;
 
namespace eUp.Controllers
{   
    [Authorize]
    public class UserTablesController : Controller
    {

        private eUpDbContext context = new eUpDbContext();

        //
        // GET: /Tables/

        public ActionResult Index()
        {
            return RedirectToAction("ListTable", "UserTables");
            //return View(context.UserTables.Include(table => table.User).Include(table => table.Fields).ToList());
        }

        //
        // POST: /Tables/
        public ViewResult ListTable()
        {
            System.Web.Security.MembershipUser user = System.Web.Security.Membership.GetUser();
            int id = (int)user.ProviderUserKey;
            return View(context.UserTables.Where(x => x.UserId == id));
        }

        public ViewResult SaveTable(int id, int tableId)
        {
            ICollection<Field> tableFields = context.Fields.Where(x => x.UserTableId == tableId).ToList();
            var conn = new ServerConnection(@".\SQLEXPRESS");
            var myServer = new Server(conn);
            var myDatabase = myServer.Databases["UserTablesDb"];
            //var myDatabase =  new Microsoft.SqlServer.Management.Smo.Database(myServer, "UserTablesDb");

            try
            {
                /*
                myServer.ConnectionContext.Connect();

                 if (myServer.Databases["UserTablesDb"] != null)
                 {
                   myServer.Databases["UserTablesDb"].Drop();
                 }
                 myServer.ConnectionContext.Connect();
                
                 myDatabase.Create();*/

                try
                {
                    Table uTable = new Table(myDatabase, "User Table_" + id + "_" + tableId); //MUST BE DYNAMIC+UNIQUE
                    Column col = new Column(uTable, "UserTableId", DataType.Int);
                    col.Identity = true;
                    uTable.Columns.Add(col);

                    foreach (var field in tableFields) // names in each table must be unique
                    {
                        string name = field.FieldName;
                        col = new Column(uTable, name, DataType.Text);
                        uTable.Columns.Add(col);
                    }
                    uTable.Create();
                    myServer.ConnectionContext.Disconnect();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.InnerException.Message);
                    System.Diagnostics.Debug.WriteLine(e.InnerException.InnerException.Message);
                    //return View();
                }

            }
            catch (Exception e)
            {
                myServer.ConnectionContext.Disconnect();
                System.Diagnostics.Debug.WriteLine(e.Message);
                //return View();
            }
            return View(tableFields);
        }

        //
        // GET: /Tables/FillTable
        public ActionResult FillTable(int id, int tableId)
        {
            var conn = new ServerConnection(@".\SQLEXPRESS");
            var myServer = new Server(conn);
            var myDatabase = myServer.Databases["UserTablesDb"];            

            //Find table in DB with this name
            var searchName = ("User Table_" + id + "_" + tableId);

            //Get required table
            Table uTable = new Table();
            foreach (Table t in myDatabase.Tables)
            {
                    if(t.Name == searchName)
                    {
                        uTable = t;
                    }
            }

            // Add column names from table to List
            IList colNames = new List<string>();
            foreach (Column col in uTable.Columns)
            {
                colNames.Add(col.Name.ToString());
            }
            
            ViewBag.Columns = colNames;

            

            return View();
            //return View(context.UserTables.Include(table => table.Fields).Single(table => table.UserTableId == id));
        }

        //
        // POST: /Tables/FillTable

        [HttpPost]
        public ActionResult FillTable(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IList listOfValues = new List<string>();
                for (var i = 0; i < form.Count; i++)
                {
                    listOfValues.Add(form[i]);
                }
                System.Diagnostics.Debug.Write(listOfValues);
                //Submit values to SQL table
            }

            return View();
        }

        public ViewResult TableData(int id)
        {
            //get data from table
            //Collection<UserTable> ut = new Collection<UserTable>();
            //ut.Add(context.UserTables.Include(table => table.Fields).Single(table => table.UserTableId == id));
            UserTable ut = context.UserTables.Include(table => table.Fields).Single(table => table.UserTableId == id);
            return View(ut.Fields);
        }

        //
        // GET: /Tables/Details/5
        public ViewResult Details(int id)
        {
            UserTable table = context.UserTables.Single(x => x.UserTableId == id);
            return View(table);
        }

        //
        // GET: /Tables/Create
        public ActionResult Create()
        {
            ViewBag.PossibleUsers = context.Users;
            return View();
        } 

        //
        // POST: /Tables/Create

        [HttpPost]
        public ActionResult Create(UserTable table)
        {
            if (ModelState.IsValid)
            {
                System.Web.Security.MembershipUser user = System.Web.Security.Membership.GetUser();
                int id = (int)user.ProviderUserKey;
                table.UserId = id;
                context.UserTables.Add(table);
                context.SaveChanges();
                int tableId = table.UserTableId;
                //return RedirectToAction("Index");  
               // return RedirectToAction("ListTable", "UserTables", new { id = id });
                return RedirectToAction("Create", "Fields", new { id = tableId });
            }
            ViewBag.PossibleUsers = context.Users;
            return View(table);
        }
        
        //
        // GET: /Tables/Edit/5
 
        public ActionResult Edit(int id)
        {
            // table = context.UserTables.Single(x => x.UserTableId == id);
            return View(context.UserTables.Include(table => table.Fields).Single(table => table.UserTableId == id));
           /* UserTable t = context.UserTables.Single(x => x.UserTableId == id);
            IList<Field> f = context.Fields.Where(y => y.UserTableId == id).ToList();
            CompModel c = new CompModel();
            c.Table = t;
            c.Fields = f;
            return View(c);*/
        }

        //
        // POST: /Tables/Edit/5

        [HttpPost]
        public ActionResult Edit(FormCollection table)
        {
            if (ModelState.IsValid)
            {
                int id = int.Parse(table.Get("UserTableId"));
                UserTable t = context.UserTables.Include(f => f.Fields).Single(f => f.UserTableId == id);
                ///check if fields null[]
                t.TableName = table.Get("TableName");
                string s = table.Get("field.FieldName");
                string[] fNames = s.Split(',');
                int index = 0;
                foreach (var n in t.Fields)
                {
                    n.FieldName = fNames[index++];
                }
                context.Entry(t).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("ListTable");
            }
            ViewBag.PossibleUsers = context.Users;
           // return View(table);
            return RedirectToAction("ListTable");
        }

        //
        // GET: /Tables/Delete/5
 
        public ActionResult Delete(int id)
        {
            UserTable table = context.UserTables.Single(x => x.UserTableId == id);
            return View(table);
        }

        //
        // POST: /Tables/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            UserTable table = context.UserTables.Single(x => x.UserTableId == id);
            context.UserTables.Remove(table);
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