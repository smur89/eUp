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
using System.Data.SqlClient;
using System.Configuration;
 
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
            //return View();
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
            ServerConnection conn = new ServerConnection(@".\SQLEXPRESS");
            Server myServer = new Server(conn);
            Microsoft.SqlServer.Management.Smo.Database myDatabase = myServer.Databases["UserTablesDb"];
            try
            {
                //myServer.ConnectionContext.Connect();


                 /*if (myServer.Databases["UserTablesDb"] != null)
                 {
                   myServer.Databases["UserTablesDb"].Drop();
                 }
                 //myServer.ConnectionContext.Connect();
                
                 myDatabase.Create();*/
                try
                {

                    Table uTable = new Table(myDatabase, "UserTable_" + id + "_" + tableId); //MUST BE DYNAMIC+UNIQUE
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
                    System.Diagnostics.Debug.WriteLine("UserTable_" + id + "_" + tableId + " created");
                    myServer.ConnectionContext.Disconnect();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to create UserTable_" + id + "_" + tableId);
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
            ServerConnection conn = new ServerConnection(@".\SQLEXPRESS");
            Server myServer = new Server(conn);
            Microsoft.SqlServer.Management.Smo.Database myDatabase = myServer.Databases["UserTablesDb"];  

           // var searchName = ("User Table_" + id + "_" + tableId);

            //Get required table
          /*  Table uTable = new Table();
            foreach (Table t in myDatabase.Tables)
            {
                if(t.Name == searchName)
                {
                    uTable = t;
                }
            }
            */
            //Find table in DB with this name
            Table uTable = myDatabase.Tables["UserTable_" + id + "_" + tableId];
            var colNames = new Collection<dynamic>();
            // Add column names from table to List
            if (uTable != null)
            {
                foreach (Column col in uTable.Columns)
                {
                    colNames.Add(col.Name.ToString());
                }
                colNames.Remove("UserTableId");
            }
            ViewBag.Columns = colNames;
            ViewBag.TableName = ("UserTable_" + id + "_" + tableId);
            return View();
            //return View(context.UserTables.Include(table => table.Fields).Single(table => table.UserTableId == id));
        }

        //
        // POST: /Tables/FillTable

        [HttpPost]
        public ActionResult FillTable(FormCollection form)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=.\sqlexpress;Initial Catalog=UserTablesDb;Integrated Security=True";
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd;
            string tValues = "";
                for (var i = 1; i < form.Count-1; i++)
                {
                   // System.Diagnostics.Debug.Write(form.Get(i));
                    tValues += "'" + form.Get(i)+"', ";
                }
                tValues += "'"+form.Get(form.Count-1)+"'";
                System.Diagnostics.Debug.Write(tValues);
                //Submit values to SQL table

                string tName = form.Get("TableName");
                cmd = new SqlCommand("INSERT INTO " + tName + " VALUES ("+ tValues +")", con);
                cmd.ExecuteNonQuery();

           return RedirectToAction("ListTable");
        }

        public ViewResult TableData(int id, int tableId)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=.\sqlexpress;Initial Catalog=UserTablesDb;Integrated Security=True";
            con.Open();
            ServerConnection conn = new ServerConnection(@".\SQLEXPRESS");
            Server myServer = new Server(conn);
            Microsoft.SqlServer.Management.Smo.Database myDatabase = myServer.Databases["UserTablesDb"];
            ICollection<dynamic> d = new Collection<dynamic>();
            string tName = "UserTable_" + id + "_" + tableId;
            DataTable dt = new DataTable();
            try
            {
                /*foreach (Table table in myDatabase.Tables)
                {
                    System.Diagnostics.Debug.WriteLine(" " + table.Name);
                    foreach (Column col in table.Columns)
                    {
                        System.Diagnostics.Debug.WriteLine("  " + col.Name + " " + col.DataType.Name);
                        d.Add(col.Name);
                    }
                }*/
                SqlCommand c = new SqlCommand("select * from " + tName, con);
                SqlDataAdapter adapter = new SqlDataAdapter(c);
                adapter.SelectCommand = c;
                adapter.Fill(dt);
                foreach (var i in dt.AsEnumerable())
                {
                    var items = i.ItemArray;
                    foreach (var v in items)
                    {
                        System.Diagnostics.Debug.Write(v + " ");
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.InnerException.Message);
                System.Diagnostics.Debug.WriteLine(e.InnerException.InnerException.Message);
                //return View();
            }
       
            return View(dt);
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
                return RedirectToAction("Create", "Fields", new { id = tableId });
            }
            ViewBag.PossibleUsers = context.Users;
            return View(table);
        }
        
        //
        // GET: /Tables/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View(context.UserTables.Include(table => table.Fields).Single(table => table.UserTableId == id));
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
            DropUserTable(table.UserId, table.UserTableId);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public void DropUserTable(int id, int tableId)
        {
            ServerConnection conn = new ServerConnection(@".\SQLEXPRESS");
            Server myServer = new Server(conn);
            Microsoft.SqlServer.Management.Smo.Database myDatabase = myServer.Databases["UserTablesDb"];
            try
            {
                Table t = myDatabase.Tables["UserTable_" + id + "_" + tableId];
                if (t != null)
                {
                    t.Drop();
                    System.Diagnostics.Debug.WriteLine("UserTable_" + id + "_" + tableId + " dropped");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to drop UserTable_" + id + "_" + tableId);
                System.Diagnostics.Debug.WriteLine(e.Message);
                myServer.ConnectionContext.Disconnect();
            }
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