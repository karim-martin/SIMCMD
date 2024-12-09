using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMCMD.Data;
using SIMCMD.Models;

namespace SIMCMD.Controllers
{
    [Authorize(Roles = "Root, Admin")]
    public class MyBackgroundJobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyBackgroundJobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MyBackgroundJobs
        public async Task<IActionResult> Index()
        {
            ViewBag.Message = "It's always best to set backbround jobs to run after hours.";

            return View(await _context.MyBackgroundJob.ToListAsync());
        }
    }
}
