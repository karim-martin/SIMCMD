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
    public class ManualFileUploadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManualFileUploadController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ManualFileUpload
        public async Task<IActionResult> Index()
        {
              return View(await _context.ManualFileUpload.ToListAsync());
        }

        // GET: ManualFileUpload/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ManualFileUpload == null)
            {
                return NotFound();
            }

            var manualFileUpload = await _context.ManualFileUpload
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manualFileUpload == null)
            {
                return NotFound();
            }

            return View(manualFileUpload);
        }

        // GET: ManualFileUpload/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ManualFileUpload/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FileName,Provider,IsFileUploaded,IsFileUnwraped,IsFileConverted,IsFileImported,FileLog")] ManualFileUpload manualFileUpload)
        {
            if (ModelState.IsValid)
            {
                _context.Add(manualFileUpload);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(manualFileUpload);
        }

        // GET: ManualFileUpload/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ManualFileUpload == null)
            {
                return NotFound();
            }

            var manualFileUpload = await _context.ManualFileUpload.FindAsync(id);
            if (manualFileUpload == null)
            {
                return NotFound();
            }
            return View(manualFileUpload);
        }

        // POST: ManualFileUpload/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FileName,Provider,IsFileUploaded,IsFileUnwraped,IsFileConverted,IsFileImported,FileLog")] ManualFileUpload manualFileUpload)
        {
            if (id != manualFileUpload.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manualFileUpload);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManualFileUploadExists(manualFileUpload.Id))
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
            return View(manualFileUpload);
        }

        // GET: ManualFileUpload/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ManualFileUpload == null)
            {
                return NotFound();
            }

            var manualFileUpload = await _context.ManualFileUpload
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manualFileUpload == null)
            {
                return NotFound();
            }

            return View(manualFileUpload);
        }

        // POST: ManualFileUpload/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ManualFileUpload == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ManualFileUpload'  is null.");
            }
            var manualFileUpload = await _context.ManualFileUpload.FindAsync(id);
            if (manualFileUpload != null)
            {
                _context.ManualFileUpload.Remove(manualFileUpload);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ManualFileUploadExists(int id)
        {
          return _context.ManualFileUpload.Any(e => e.Id == id);
        }
    }
}
