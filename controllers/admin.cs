using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Context;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class admin : Controller
    {
        public IActionResult IndexForAdmin()
        {
            DepartmentAndListOfFacultyAndListOfUniversity model = new DepartmentAndListOfFacultyAndListOfUniversity
            {
                Faculties = db.Faculty.ToList(),
                Universities = db.University.ToList()

            };
            return View(model);
        }
        private readonly DataContext db;
        public admin(DataContext context)
        {
            db = context;
        }
        public IActionResult LoginForm()
        {

            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult LoginForm(Admin log)
        {
            var Vaild = db.Admin.Where(admin => admin.AdminUserName == log.AdminUserName && admin.AdminPassword == log.AdminPassword).Count();
            if (Vaild > 0)
            {

                return RedirectToAction("IndexForAdmin");

            }

            return View();
        }
        public async Task<IActionResult> searchCourseForAdmin(string searchCourse)
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

        public async Task<IActionResult> searchDepartmentForAdmin(string searchDepartment)
        {
            ViewData["CurrentFilter"] = searchDepartment;
            var departmentsWithoutSearch = db.Department.ToList();
            var departmentsWithSearch = from department in db.Department select department;
            if (!String.IsNullOrEmpty(searchDepartment))
            {
                var resultFromSerach = departmentsWithSearch.Where(b => b.DepartmentName.Contains(searchDepartment) || b.UniversityName.Contains(searchDepartment)).ToList();

                return View(resultFromSerach);

            }
            else
            {
                return View(departmentsWithoutSearch);
            }
            return View();
        }
        public async Task<IActionResult> searchFacultyForAdmin(string searchFaculty)
        {
            ViewData["CurrentFilter"] = searchFaculty;
            var universities = db.University.ToList();
            var facultiesWithoutSearch = db.Faculty.ToList();
            var FacultiesWithSearch = from Faculty in db.Faculty select Faculty;
            if (!String.IsNullOrEmpty(searchFaculty))
            {
                var resultFromSerach = FacultiesWithSearch.Where(b => b.FacultyName.Contains(searchFaculty) || b.UniversityName.Contains(searchFaculty)).ToList();

                return View(resultFromSerach);

            }
            else
            {
                return View(facultiesWithoutSearch);
            }
            return View();
        }

        public async Task<IActionResult> SearchUniverstyForAdmin(string SearchUniversty)
        {
            ViewData["CurrentFilter"] = SearchUniversty;
            var UniverstiesWithoutSearch = db.University.ToList();
            var UniverstiesWithSearch = from University in db.University select University;
            if (!String.IsNullOrEmpty(SearchUniversty))
            {
                var resultFromSerach = UniverstiesWithSearch.Where(b => b.UniversityName.Contains(SearchUniversty)).ToList();

                return View(resultFromSerach);

            }
            else
            {
                return View(UniverstiesWithoutSearch);
            }
            return View();
        }
    }
}
