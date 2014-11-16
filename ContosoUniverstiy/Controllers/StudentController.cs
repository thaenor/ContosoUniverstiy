using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContosoUniverstiy.Models;
using ContosoUniverstiy.DAL;

namespace ContosoUniverstiy.Controllers
{
    public class StudentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        //
        // GET: /Student/
        /*
         * This code receives a sortOrder parameter from the query string in the URL. 
         * The query string value is provided by ASP.NET MVC as a parameter to the action method. 
         * The parameter will be a string that's either "Name" or "Date", optionally followed by an underscore and the string "desc" to specify descending order. 
         * The default sort order is ascending.
         * The first time the Index page is requested, there's no query string. 
         * The students are displayed in ascending order by LastName, which is the default as established by the fall-through case in the switch statement. 
         * When the user clicks a column heading hyperlink, the appropriate sortOrder value is provided in the query string.
         * ###
         * For my next trick, I'll add a search function, adding the searchString param as the if statement to locate student
         */
        public ActionResult Index(string sortOrder, string searchString)
        {
            /*
             * The two ViewBag variables bellow are used so that the view can configure the column heading hyperlinks with the appropriate query string values
             * These are ternary statements. 
             * The first one specifies that if the sortOrder parameter is null or empty, ViewBag.NameSortParm should be set to "name_desc"; 
             * otherwise, it should be set to an empty string. 
             */
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date_desc" : "Date";
            var students = from s in db.Students
                           select s;
            
            //searching student... where did he go?
            if (!String.IsNullOrEmpty(searchString)) 
            {
                students = students.Where(s => s.LastName.ToUpper().Contains(searchString.ToUpper())
                                        || s.FirstMidName.ToUpper().Contains(searchString.ToUpper())); 
            }
            /*
             * Nice! You've just added a searchString parameter to the Index method.
             * You've also added  to the LINQ statement a where clause that selects only students whose first name or last name contains the search string. 
             * The search string value is received from a text box that you'll add to the Index view. 
             * The statement that adds the where clause is executed only if there's a value to search for
             * 
             * Note: 
             * In many cases you can call the same method either on an Entity Framework entity set or as an extension method on an in-memory collection. 
             * The results are normally the same but in some cases may be different. 
             * For example, the .NET Framework implementation of the Contains method returns all rows when you pass an empty string to it, but the Entity Framework provider for SQL Server Compact 4.0 returns zero rows for empty strings.
             * Therefore the code in the example (putting the Where statement inside an if statement) makes sure that you get the same results for all versions of SQL Server. 
             * Also, the .NET Framework implementation of the Contains method performs a case-sensitive comparison by default, but Entity Framework SQL Server providers perform case-insensitive comparisons by default. 
             * Therefore, calling the ToUpper method to make the test explicitly case-insensitive ensures that results do not change when you change the code later to use a repository, which will return an IEnumerable collection instead of an IQueryable object. 
             * (When you call the Contains method on an IEnumerable collection, you get the .NET Framework implementation; 
             * when you call it on an IQueryable object, you get the database provider implementation.)
             */


            switch (sortOrder) 
            { 
                case "Name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "Date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }
            return View(students.ToList());
            //Old return statement was returning the db students unsorted list
            //return View(db.Students.ToList());
        }

        //
        // GET: /Student/Details/5
        //The key value is passed to the method as the id parameter and comes from route data in the Details hyperlink on the Index page. 
        public ActionResult Details(int id = 0)
        {
            Student student = db.Students.Find(id); //retrieves single student
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // GET: /Student/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Student/Create
        //changes will add a try-catch block and the Bind attribute to the scaffolded method.
        /*
         * This code adds the Student entity created by the ASP.NET MVC model binder to the Students entity set and then saves the changes to the database. 
         * (Model binder refers to the ASP.NET MVC functionality that makes it easier for you to work with data submitted by a form;
         * a model binder converts posted form values to CLR types and passes them to the action method in parameters. 
         * In this case, the model binder instantiates a Student entity for you using property values from the Form collection.)
         * 
         * The ValidateAntiForgeryToken attribute helps prevent cross-site request forgery attacks.
         */
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName, FirstMidName, EnrollmentDate")] 
                                    Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */) 
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.)
                ModelState.AddModelError("", "unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            /*
             * If an exception that derives from DataException is caught while the changes are being saved, a generic error message is displayed. 
             * DataException exceptions are sometimes caused by something external to the application rather than a programming error, 
             * so the user is advised to try again. Although not implemented in this sample, a production quality application would log the exception 
             * (and non-null inner exceptions ) with a logging mechanism such as ELMAH.
             */
            return View(student);
        }

        //
        // GET: /Student/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // POST: /Student/Edit/5
        /*
         * This code is similar to what you saw in the HttpPost Create method. 
         * However, instead of adding the entity created by the model binder to the entity set, this code sets a flag on the entity indicating it has been changed. 
         * When the SaveChanges method is called, the Modified flag causes the Entity Framework to create SQL statements to update the database row. 
         * All columns of the database row will be updated, including those that the user didn't change, and concurrency conflicts are ignored. 
         * (You'll learn how to handle concurrency in a later tutorial in this series.)
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentID, LastName, FirstMidName, EnrollmentDate")]
                                    Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(student).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */) 
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes, Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }

        //
        // GET: /Student/Delete/5

        /*
         * Goal: implement custom error message when the call to SaveChanges fails.
         * 
         * As you saw for update and create operations, delete operations require two action methods. 
         * The method that is called in response to a GET request displays a view that gives the user a chance to approve or cancel the delete operation. 
         * If the user approves it, a POST request is created. 
         * When that happens, the HttpPost Delete method is called and then that method actually performs the delete operation.
         * 
         * You'll add a try-catch block to the HttpPost Delete method to handle any errors that might occur when the database is updated. 
         * If an error occurs, the HttpPost Delete method calls the HttpGet Delete method, passing it a parameter that indicates that an error has occurred.
         * The HttpGet Delete method then redisplays the confirmation page along with the error message, giving the user an opportunity to cancel or try again.
         */

        public ActionResult Delete(bool? saveChangesError=false, int id = 0)
        {
            /*
             * This code accepts an optional Boolean parameter that indicates whether it was called after a failure to save changes. 
             * This parameter is false when the HttpGet Delete method is called without a previous failure. 
             * When it is called by the HttpPost Delete method in response to a database update error, the parameter is true 
             * and an error message is passed to the view.
             */

            if (saveChangesError.GetValueOrDefault()) 
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persist see your system administrator.";
            }

            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // POST: /Student/Delete/5

        /***CHANGES will perform the actual delete operation and catch any database update errors***/
        /*
         * This code retrieves the selected entity, then calls the Remove method to set the entity's status to Deleted. 
         * When SaveChanges is called, a SQL DELETE command is generated. 
         * You have also changed the action method name from DeleteConfirmed to Delete. 
         * The scaffolded code named the HttpPost Delete method DeleteConfirmed to give the HttpPost  method a unique signature. 
         * ( The CLR requires overloaded methods to have different method parameters.) 
         * Now that the signatures are unique, you can stick with the MVC convention and use the same name for the HttpPost and HttpGet delete methods.
         */

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();
            }
            catch (DataException /* dex */) 
            { 
                //uncomment dex and log error
                return RedirectToAction("Delete", new { id = id, savechangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}