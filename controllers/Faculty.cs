using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class Faculty : Controller
    {
        private readonly DataContext db;

        public Faculty(DataContext context)
        {
            db = context;
        }

        // GET: Faculty
        public async Task<IActionResult> Index()
        {
            var x = db.Faculty.ToList();
            return View(x);
        }

        
        public IActionResult createFaculty()
        {
            ViewModels.FacultyAndListOfUniversity faculty = new ViewModels.FacultyAndListOfUniversity {
                Universities = db.University.ToList(),
            };
            
            return View(faculty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> createFaculty( ViewModels.FacultyAndListOfUniversity facult)
        {
            if (!ModelState.IsValid)
            {
                facult.Universities = db.University.ToList();
            }
            facult.Faculty.UniversityName = db.University.SingleOrDefault(x=>x.UniversityId == facult.Faculty.UniverstyId).UniversityName;
            db.Faculty.Add(facult.Faculty);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> editFaculty(int? id)
        {

            var fac = new FacultyAndListOfUniversity
            {
                Faculty = await db.Faculty.FindAsync(id),
                Universities = db.University.ToList()
            };
            return View(fac);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editFaculty(int id, FacultyAndListOfUniversity fac)
        {
            if (!ModelState.IsValid)
            {
                fac.Universities = db.University.ToList();
                return View(fac); 
            }
         
            var FacultyFromdata = await db.Faculty.FindAsync(fac.Faculty.FacultyId);
            var temp = await (db.University.FirstOrDefaultAsync(m => m.UniversityId == fac.Faculty.UniverstyId));
            FacultyFromdata.UniversityName = temp.UniversityName;
            FacultyFromdata.UniverstyId = fac.Faculty.UniverstyId;
            FacultyFromdata.AreaOfExpertise = fac.Faculty.AreaOfExpertise;
            FacultyFromdata.FacultyName = fac.Faculty.FacultyName;
            FacultyFromdata.Description = fac.Faculty.Description;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> deleteFaculty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ActionDelete = db.Faculty.Find(id);
            db.Faculty.Remove(ActionDelete);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> facultyDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Faculty = await db.Faculty
                .FirstOrDefaultAsync(m => m.FacultyId == id);
            if (Faculty == null)
            {
                return NotFound();
            }

            return View(Faculty);
        }

        public async Task<IActionResult> searchFaculty(string searchFaculty)
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


    }
}
