using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Context;

namespace WebApplication1.Controllers
{
    public class Course : Controller
    {
        private readonly DataContext db;
        public Course(DataContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View(db.Course.ToList());
        }
        // GET: Departments/Create
        public IActionResult CreateCourse()
        {
            ViewModels.CourseAndListOfUFD obj = new ViewModels.CourseAndListOfUFD
            {
                Universities = db.University.ToList(),
                Faculties = db.Faculty.ToList(),
                Departments = db.Department.ToList()
            };
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(ViewModels.CourseAndListOfUFD obj)
        {
            if (!ModelState.IsValid)
            {
                obj.Universities = db.University.ToList();
                obj.Faculties = db.Faculty.ToList();
                obj.Departments = db.Department.ToList();
            }
            obj.Course.UniversityName = db.University.SingleOrDefault(x => x.UniversityId == obj.Course.UniversityId).UniversityName;
             
            obj.Course.FacultyName = db.Faculty.SingleOrDefault(x => x.FacultyId == obj.Course.FacultyId).FacultyName;
            obj.Course.DepartmentName = db.Department.SingleOrDefault(x => x.DepartmentId == obj.Course.DepartmentID).DepartmentName;
            db.Course.Add(obj.Course);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> editCourse(int? id)
        {

            var course = new ViewModels.CourseAndListOfUFD
            {
                Course = await db.Course.FindAsync(id),
            };
            return View(course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editCourse(int id, ViewModels.CourseAndListOfUFD courseModel)
        {
            if (!ModelState.IsValid)
            {
                return View(courseModel);
            }

            var CourseFromDatabase = await db.Course.FindAsync(id);       
            CourseFromDatabase.CourseName = courseModel.Course.CourseName;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> courseDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await db.Course
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        public async Task<IActionResult> deleteCourse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ActionDelete = db.Course.Find(id);
            db.Course.Remove(ActionDelete);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> searchCourse(string searchCourse)
        {
            ViewData["CurrentFilter"] = searchCourse;
            var coursesWithoutSearch = db.Course.ToList();
            var coursesWithSearch = from course in db.Course select course;
            if (!String.IsNullOrEmpty(searchCourse))
            {
                var resultFromSerach = coursesWithSearch.Where(b => b.CourseName.Contains(searchCourse) || b.UniversityName.Contains(searchCourse)).ToList();

                return View(resultFromSerach);

            }
            else
            {
                return View(coursesWithoutSearch);
            }
            return View();
        }


    }
}
