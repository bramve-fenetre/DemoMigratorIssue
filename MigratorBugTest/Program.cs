using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MigratorBugTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using var dbContext = new TestContext();

            var migrator = dbContext.Database.GetService<IMigrator>();
            var pendingMigrations = dbContext.Database.GetPendingMigrations();
            foreach(var pendingMigration in pendingMigrations)
            {
                try
                {
                    migrator.Migrate(pendingMigration);
                }
                catch (Exception e)
                {
                    var someException = e;
                }
            }
        }
    }

    public class TestContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MigrationTest");
    }

    [Table("Car")]
    public class Car
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

	[DbContext(typeof(TestContext))]
    [Migration("InitialMigration")]
    public class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Car",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });
        }
    }


}
