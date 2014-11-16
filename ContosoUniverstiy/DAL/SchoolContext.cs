using ContosoUniverstiy.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ContosoUniverstiy.DAL
{
    public class SchoolContext : DbContext
    {
        /*
         * This code creates a DbSet property for each entity set. 
         * In Entity Framework terminology, an entity set typically corresponds to a database table, 
         * and an entity corresponds to a row in the table.
         */
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }

        /*
         * The modelBuilder.Conventions.Remove statement in the OnModelCreating method prevents table names from being pluralized. 
         * If you didn't do this, the generated tables would be named Students, Courses, and Enrollments. 
         * Instead, the table names will be Student, Course, and Enrollment. 
         * Developers disagree about whether table names should be pluralized or not. 
         * This tutorial uses the singular form, but the important point is that you can select whichever form you prefer by including or omitting this line of code.
         */
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}

/*
 * The main class that coordinates Entity Framework functionality for a given data model is the database context class. 
 * You create this class by deriving from the System.Data.Entity.DbContext class. In your code you specify which entities are included in the data model. 
 * You can also customize certain Entity Framework behavior. In this project, the class is named SchoolContext.
 * This is a folder named DAL (for Data Access Layer).
 */