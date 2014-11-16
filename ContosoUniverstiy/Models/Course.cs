using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ContosoUniverstiy.Models
{
    public class Course
    {
        /*Basically, this attribute lets you enter the primary key for the course rather than having the database generate it.*/
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        /*The Enrollments property is a navigation property. A Course entity can be related to any number of Enrollment entities*/
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}