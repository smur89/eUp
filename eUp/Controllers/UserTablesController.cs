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
    [Authorize] // prevent unauthorized users from accessing application
    public class UserTablesController : Controller
    {
        //create an instance of a database
        private eUpDbContext context = new eUpDbContext();
        
        //default action
        // GET: /Tables/
        public ActionResult Index()
        {
            //display a list of user forms
            return RedirectToAction("ListTable", "UserTables");
        }

        // Retrieves Id of logged in user and return all tables that belong to that user
        // POST: /Tables/
        public ViewResult ListTable()
        {
            System.Web.Security.MembershipUser user = System.Web.Security.Membership.GetUser();
            int id = (int)user.ProviderUserKey;
            return View(context.UserTables.Where(x => x.UserId == id));
        }

        //takes 2 parameters, user id and table id and creates a user form with unique name and user defined fields
        //
        public ViewResult SaveTable(int id, int tableId)
        {
            //collection of user defined fields
            ICollection<Field> tableFields = context.Fields.Where(x => x.UserTableId == tableId).ToList();
            //Connection to a server
            ServerConnection conn = new ServerConnection(@".\SQLEXPRESS");
            Server myServer = new Server(conn);
            Microsoft.SqlServer.Management.Smo.Database myDatabase = myServer.Databases["UserTablesDb"];
            try
            {
                //Commented out section contains a code necessary to drop and recreate a table containig users' forms
                //WARNING - ALL DATA WILL BE LOST

                /*myServer.ConnectionContext.Connect();
                if(myServer.Databases["UserTablesDb"] != null)
                {
                    myServer.Databases["UserTablesDb"].Drop();
                }
                myDatabase.Create();*/

                try
                {
                    //creates a form with unique name based on user id and table id
                    Table uTable = new Table(myDatabase, "UserTable_" + id + "_" + tableId);
                    //first column of a form is always UserTableId
                    Column col = new Column(uTable, "UserTableId", DataType.Int);
                    col.Identity = true;
                    //add column to a table
                    uTable.Columns.Add(col);
                    //loops through user defined fields and creates columns from them
                    foreach (var field in tableFields) // TODO: names in each table must be unique
                    {
                        string name = field.FieldName;
                        col = new Column(uTable, name, DataType.Text);//TODO: get dataType from dropdown box
                        uTable.Columns.Add(col);
                    }
                    //saves table to a databes
                    uTable.Create();
                    //TODO: Log file
                    System.Diagnostics.Debug.WriteLine("UserTable_" + id + "_" + tableId + " created");
                    //disconnect from a server
                    myServer.ConnectionContext.Disconnect();
                }
                catch (Exception e)
                {
                    //error messages print off to a console
                    System.Diagnostics.Debug.WriteLine("Failed to create UserTable_" + id + "_" + tableId);
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.InnerException.Message);
                    System.Diagnostics.Debug.WriteLine(e.InnerException.InnerException.Message);
                    //optional: may return a page with error description
                    //return View();
                }

            }
            catch (Exception e)
            {
                // disconnects from a server and print off error message  to a console
                myServer.ConnectionContext.Disconnect();
                System.Diagnostics.Debug.WriteLine(e.Message);
                //optional: may return a page with error description
                //return View();
            }
            //pass collection of user defined fields to a View
            return View(tableFields);
        }

        //
        // GET: /Tables/FillTable
        public ActionResult FillTable(int id, int tableId)
        {
            //server connection objects
            ServerConnection conn = new ServerConnection(@".\SQLEXPRESS");
            Server myServer = new Server(conn);
            Microsoft.SqlServer.Management.Smo.Database myDatabase = myServer.Databases["UserTablesDb"];  
            //Find table in DB with this name
            Table uTable = myDatabase.Tables["UserTable_" + id + "_" + tableId];
            var colNames = new Collection<dynamic>();
            // Add column names from table to collection
            if (uTable != null)
            {
                foreach (Column col in uTable.Columns)
                {
                    colNames.Add(col.Name.ToString());
                }
                //remove first column - UserTableId - from collection - exclude it from insert statement
                colNames.Remove("UserTableId");
            }
            //use ViewBag to pass column names and table name to a View
            ViewBag.Columns = colNames;
            ViewBag.TableName = ("UserTable_" + id + "_" + tableId);
            return View();
        }

        //retrieves form values entered by user and save them in a user form
        // POST: /Tables/FillTable
        [HttpPost]
        public ActionResult FillTable(FormCollection form)
        {
            //SQL connection object
            SqlConnection con = new SqlConnection();
            //SQL connection string
            con.ConnectionString = @"Data Source=.\sqlexpress;Initial Catalog=UserTablesDb;Integrated Security=True";
            con.Open();
            //SqlDataAdapter is a set of commands to manipulate data and database
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd;
            //empty string from which a proper string of values to be inserted will be formed
            string tValues = "";
                //loops through FormCollection to get values to be inserted
                //index starts with 1 - value at index 0 is Table Name
                //goes up to number of values in collection -1 // last value is added outside a loop to avoid adding a coma after last value
                for (var i = 1; i < form.Count-1; i++)
                {
                    //construct a string of values by enclosing them inside ' ' and separating by comas
                    tValues += "'" + form.Get(i)+"', ";
                }
                //last value added without a coma at the end // otherwise - error
                tValues += "'"+form.Get(form.Count-1)+"'";
                //retreives name of a table
                string tName = form.Get("TableName");
                //Submit values to SQL table using table name and constructed string of values
                cmd = new SqlCommand("INSERT INTO " + tName + " VALUES ("+ tValues +")", con);
                cmd.ExecuteNonQuery();
           return RedirectToAction("ListTable");
        }

        //returns all user form data
        public ViewResult TableData(int id, int tableId)
        {
            //Sql connection established
            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=.\sqlexpress;Initial Catalog=UserTablesDb;Integrated Security=True";
            con.Open();
            //name of the user form that contain data
            string tName = "UserTable_" + id + "_" + tableId;
            //DataTable represent a table. Can be filled with table data
            DataTable dt = new DataTable();
            try
            {
                //return all values from user form
                SqlCommand c = new SqlCommand("select * from " + tName, con);
                SqlDataAdapter adapter = new SqlDataAdapter(c);
                //execute select command
                adapter.SelectCommand = c;
                //fill DataTable object with form data
                adapter.Fill(dt);
                /*Debugging code
                foreach (var i in dt.AsEnumerable())
                {
                    var items = i.ItemArray;
                    foreach (var v in items)
                    {
                        System.Diagnostics.Debug.Write(v + " ");
                    }
                }*/
            }
            catch (Exception e)
            {
                //print off errors to a console
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.InnerException.Message);
                System.Diagnostics.Debug.WriteLine(e.InnerException.InnerException.Message);
                //optional: display a page with error description
                //return View();
            }
            //pass DataTable object to a View to display data
            return View(dt);
        }

        //
        // GET: /Tables/Details/5
        public ViewResult Details(int id)
        {
            UserTable table = context.UserTables.Single(x => x.UserTableId == id);
            return View(table);
        }

        //returns a View to create user form
        // GET: /Tables/Create
        public ActionResult Create()
        {
            return View();
        } 

        //save user form and returns a View to create form fields
        // POST: /Tables/Create
        [HttpPost]
        public ActionResult Create(UserTable table)
        {
            //validate state of model-binding
            if (ModelState.IsValid)
            {
                //currently logged in user
                System.Web.Security.MembershipUser user = System.Web.Security.Membership.GetUser();
                //currently logged in user's id
                int id = (int)user.ProviderUserKey;
                //assign user id to a form user id
                table.UserId = id;
                context.UserTables.Add(table);
                context.SaveChanges();
                //assign form id to a variable
                int tableId = table.UserTableId;
                //call a methof to create form fields and pass in a form id
                return RedirectToAction("Create", "Fields", new { id = tableId });
            }
            //if state not valid create again
            return View(table);
        }
        
        //return a View to edit user form
        // GET: /Tables/Edit/5
        public ActionResult Edit(int id)
        {
            return View(context.UserTables.Include(table => table.Fields).Single(table => table.UserTableId == id));
        }

        //retrieves all values from Edit View and saves them
        // POST: /Tables/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection table)
        {
            if (ModelState.IsValid)
            {
                //all values return as string
                //IDs need to be parsed to INT
                int id = int.Parse(table.Get("UserTableId"));
                //fetch a user form to be changed from DB
                UserTable t = context.UserTables.Include(f => f.Fields).Single(f => f.UserTableId == id);
                //change form name
                t.TableName = table.Get("TableName");
                //construct one, long string of fields' names separated by commas
                string s = table.Get("field.FieldName");
                //all values need to be separated and put into an array so that they can be accessed separately
                string[] fNames = s.Split(',');
                //array index
                int index = 0;
                //loops through fields in a form
                foreach (var n in t.Fields)
                {
                    //assing a new field name and increment array index by 1
                    n.FieldName = fNames[index++];
                }
                //indicate to Entity Framework that a table has been changed and save it
                context.Entry(t).State = EntityState.Modified;
                context.SaveChanges();
                //return to form list
                return RedirectToAction("ListTable");
            }
            return RedirectToAction("ListTable");
        }

        //return a View to confirm form deletion
        // GET: /Tables/Delete/5
        public ActionResult Delete(int id)
        {
            UserTable table = context.UserTables.Single(x => x.UserTableId == id);
            return View(table);
        }

        //form deletion confirmed
        // POST: /Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        { 
            //fetch a table from database and remove it
            UserTable table = context.UserTables.Single(x => x.UserTableId == id);
            context.UserTables.Remove(table);
            //call a method to remove associated user form
            DropUserTable(table.UserId, table.UserTableId);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        
        //delete user form associated with user table
        public void DropUserTable(int id, int tableId)
        {
            //SMO connection objects
            ServerConnection conn = new ServerConnection(@".\SQLEXPRESS");
            Server myServer = new Server(conn);
            Microsoft.SqlServer.Management.Smo.Database myDatabase = myServer.Databases["UserTablesDb"];
            try
            {
                //fetch user form
                Table t = myDatabase.Tables["UserTable_" + id + "_" + tableId];
                //check if it is not null
                if (t != null)
                {
                    //if exits delete it
                    t.Drop();
                    System.Diagnostics.Debug.WriteLine("UserTable_" + id + "_" + tableId + " dropped");
                }
            }
            catch (Exception e)
            {
                //print off error messages and close server connection
                System.Diagnostics.Debug.WriteLine("Failed to drop UserTable_" + id + "_" + tableId);
                System.Diagnostics.Debug.WriteLine(e.Message);
                myServer.ConnectionContext.Disconnect();
            }
        }

        //release unmanaged resources
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}