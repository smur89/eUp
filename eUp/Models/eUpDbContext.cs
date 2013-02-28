using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace eUp.Models
{
    public class eUpDbContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<MvcApplication12.Models.MvcApplication12Context>());

        static eUpDbContext()
        {
            Database.SetInitializer(
                new DropCreateDatabaseAlways<eUp.Models.eUpDbContext>());
              // new DropCreateDatabaseIfModelChanges<eUp.Models.eUpDbContext>());
        }

        public DbSet<eUp.Models.User> Users { get; set; }

        public DbSet<eUp.Models.UserTable> UserTables { get; set; }

        public DbSet<eUp.Models.Field> Fields { get; set; }
    }
}