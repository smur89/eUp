namespace eUp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebMatrix.WebData;
    using System.Web.Security;
    using System.Collections.Generic;
    using eUp.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<eUp.Models.eUpDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(eUp.Models.eUpDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
           /* base.Seed(context);

            WebSecurity.InitializeDatabaseConnection(
            "eUpDbContext",
            "User",
            "UserId",
            "UserName",
            "Email", 
            autoCreateTables: true);


            if (!Roles.Provider.RoleExists("Administrator"))
                Roles.Provider.CreateRole("Administrator");

            if (!WebSecurity.UserExists("admin"))
                WebSecurity.CreateUserAndAccount("admin", "admin");

            if (!((IList<string>)Roles.Provider.GetRolesForUser("admin")).Contains("Administrator"))
                Roles.Provider.AddUsersToRoles(new[] { "admin" }, new[] { "Administrator" });

            context.Database.Initialize(true);
            context.Set<User>().Add(new User{ UserId = 1, UserName = "admin" , Email="admin@admin.com"});
     */
        }
    }
}
