using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMCMD.Data;
using SIMCMD.Models;

namespace SIMCMD.Controllers
{
    public class FileConversionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FileConversionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FileConversion
        public async Task<IActionResult> Index()
        {
              return View(await _context.FileConversion.ToListAsync());
        }

        // GET: FileConversion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FileConversion == null)
            {
                return NotFound();
            }

            var fileConversion = await _context.FileConversion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileConversion == null)
            {
                return NotFound();
            }

            return View(fileConversion);
        }

        // GET: FileConversion/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FileConversion fileConversion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fileConversion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fileConversion);
        }

        // GET: FileConversion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FileConversion == null)
            {
                return NotFound();
            }

            var fileConversion = await _context.FileConversion.FindAsync(id);
            if (fileConversion == null)
            {
                return NotFound();
            }
            return View(fileConversion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FileConversion fileConversion)
        {
            if (id != fileConversion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fileConversion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileConversionExists(fileConversion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fileConversion);
        }

        // GET: FileConversion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FileConversion == null)
            {
                return NotFound();
            }

            var fileConversion = await _context.FileConversion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileConversion == null)
            {
                return NotFound();
            }

            return View(fileConversion);
        }

        // POST: FileConversion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FileConversion == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FileConversion'  is null.");
            }
            var fileConversion = await _context.FileConversion.FindAsync(id);
            if (fileConversion != null)
            {
                _context.FileConversion.Remove(fileConversion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FileConversionExists(int id)
        {
          return _context.FileConversion.Any(e => e.Id == id);
        }
    }
}
