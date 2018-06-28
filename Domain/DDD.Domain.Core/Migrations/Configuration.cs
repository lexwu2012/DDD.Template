using DDD.Domain.Core.Migrations.SeedData;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;


namespace DDD.Domain.Core.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DDDDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            ContextKey = "DDDTest";

        }

        protected override void Seed(DDDDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            new DefaultCreator(context).Create();
        }
    }
}
