using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class Department : Controller
    {
        private readonly DataContext db;

        public Department(DataContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {

            return View(db.Department.ToList());
        }

        // GET: Departments/Create
        public IActionResult createDepartment()
        {
            ViewModels.DepartmentAndListOfFacultyAndListOfUniversity obj = new ViewModels.DepartmentAndListOfFacultyAndListOfUniversity
            {
                Faculties = db.Faculty.ToList(),
                Universities = db.University.ToList(),
            };
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> createDepartment(ViewModels.DepartmentAndListOfFacultyAndListOfUniversity dep)
        {
            if (!ModelState.IsValid)
            {
                dep.Universities = db.University.ToList();
                dep.Faculties = db.Faculty.ToList();
            }
            dep.Department.UniversityName = db.University.SingleOrDefault(x => x.UniversityId == dep.Department.UniversityId).UniversityName;
            dep.Department.FacultyName = db.Faculty.SingleOrDefault(x => x.FacultyId == dep.Department.FacultyId).FacultyName;
            db.Department.Add(dep.Department);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
          public async Task<IActionResult> editDepartment(int? id)
        {

            var dep = new ViewModels.DepartmentAndListOfFacultyAndListOfUniversity
            {
                Department = await db.Department.FindAsync(id),
                Universities = db.University.ToList(),
                Faculties = db.Faculty.ToList()
                
            };
            return View(dep);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editDepartment(int id, ViewModels.DepartmentAndListOfFacultyAndListOfUniversity deb)
        {
            if (!ModelState.IsValid)
            {
                deb.Universities = db.University.ToList();
                deb.Faculties = db.Faculty.ToList();
                return View(deb); 
            }
         
            var departmentFromDatabase = await db.Department.FindAsync(deb.Department.DepartmentId);
            var temp = await (db.University.FirstOrDefaultAsync(m => m.UniversityId == deb.Department.UniversityId));
            var tmpFaculty = await (db.Faculty.FirstOrDefaultAsync(m => m.FacultyId == deb.Department.FacultyId));

            departmentFromDatabase.UniversityName = temp.UniversityName;
            departmentFromDatabase.FacultyName = tmpFaculty.FacultyName;
            departmentFromDatabase.UniversityId = deb.Department.UniversityId;
            departmentFromDatabase.FacultyId = deb.Department.FacultyId;
            departmentFromDatabase.DepartmentName = deb.Department.DepartmentName;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DepartmentDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await db.Department
                .FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }
        public async Task<IActionResult> deleteDepartmnet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ActionDelete = db.Department.Find(id);
            db.Department.Remove(ActionDelete);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> searchDepartment(string searchDepartment)
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


    }
}
