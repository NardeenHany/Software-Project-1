using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Context;

namespace WebApplication1.Controllers
{
    public class University : Controller
    {
        private readonly DataContext db;
        
        public University(Context.DataContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View(db.University.ToList());
        }

        
        public IActionResult createUniversity()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> createUniversity(Models.University model)
        {

            var files = Request.Form.Files;
            if (!files.Any())
            {
                ModelState.AddModelError("UniversetyPhotoLink", "please enter Fuculty Photo");
                return View( model);
            }
            var UniversetyPhotoLink = files.FirstOrDefault();
            using var dataStream = new MemoryStream();
            await UniversetyPhotoLink.CopyToAsync(dataStream);
            var university = new Models.University
            {
                UniversityName = model.UniversityName,
                Description=model.Description,
                UniversetyPhotoLink = dataStream.ToArray()

            };
            db.University.Add(university);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> editUniversity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var university = await db.University.FindAsync(id);
            if (university == null)
            {
                return NotFound();
            }
            return View(university);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editUniversity(int id, Models.University university)
        {
            if (id != university.UniversityId)
            {
                return NotFound();
            }
            var universityFrom = await db.University.FindAsync(university.UniversityId);
            var files = Request.Form.Files;
            using var dataStream = new MemoryStream();
            if (files.Any())
            {
                var UniversityPhoto = files.FirstOrDefault();
                await UniversityPhoto.CopyToAsync(dataStream);
                university.UniversetyPhotoLink = dataStream.ToArray();
                universityFrom.UniversetyPhotoLink = dataStream.ToArray();

            }
            universityFrom.UniversityName = university.UniversityName;
            universityFrom.Description = university.Description;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> universityDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var university = await db.University
                .FirstOrDefaultAsync(m => m.UniversityId == id);
            if (university == null)
            {
                return NotFound();
            }

            return View(university);
        }

        public async Task<IActionResult> deleteUniversity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ActionDelete = db.University.Find(id);
            db.University.Remove(ActionDelete);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> SearchUniversty(string SearchUniversty)
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
