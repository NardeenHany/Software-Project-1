using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Context;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly DataContext db;
        public HomeController(DataContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {

            DepartmentAndListOfFacultyAndListOfUniversity model = new DepartmentAndListOfFacultyAndListOfUniversity
            {
                Faculties = db.Faculty.ToList(),
                Universities=db.University.ToList()

            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        
    }
}
