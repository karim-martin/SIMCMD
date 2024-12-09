using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SIMCMD.Data;
using SIMCMD.Models;

namespace SIMCMD.Controllers
{
    public class ProviderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ProviderController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Provider
        public async Task<IActionResult> Index()
        {
              return View(await _context.Provider.ToListAsync());
        }

        // GET: Provider/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Provider == null)
            {
                return NotFound();
            }

            var provider = await _context.Provider
                .FirstOrDefaultAsync(m => m.Id == id);
            if (provider == null)
            {
                return NotFound();
            }

            return View(provider);
        }

        // GET: Provider/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Provider provider)
        {
            if (ModelState.IsValid)
            {
                var sftpFolderPath = Path.Combine(_configuration.GetValue<string>("FolderConfig:DownloadFiles:SFTPFolder"), provider.FolderName);
                var sftpCopyFolderPath = Path.Combine(_configuration.GetValue<string>("FolderConfig:DownloadFiles:SFTPCopyFolder"), provider.FolderName);

                // Check and create SFTP folder
                if (!Directory.Exists(sftpFolderPath))
                {
                    Directory.CreateDirectory(sftpFolderPath);
                }

                // Check and create SFTP Copy folder
                if (!Directory.Exists(sftpCopyFolderPath))
                {
                    Directory.CreateDirectory(sftpCopyFolderPath);
                }

                _context.Add(provider);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(provider);
        }

        // GET: Provider/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Provider == null)
            {
                return NotFound();
            }

            var provider = await _context.Provider.FindAsync(id);
            if (provider == null)
            {
                return NotFound();
            }
            return View(provider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Provider provider)
        {
            if (id != provider.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(provider);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProviderExists(provider.Id))
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
            return View(provider);
        }

        // GET: Provider/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Provider == null)
            {
                return NotFound();
            }

            var provider = await _context.Provider
                .FirstOrDefaultAsync(m => m.Id == id);
            if (provider == null)
            {
                return NotFound();
            }

            return View(provider);
        }

        // POST: Provider/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Provider == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Provider'  is null.");
            }
            var provider = await _context.Provider.FindAsync(id);
            if (provider != null)
            {
                _context.Provider.Remove(provider);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProviderExists(int id)
        {
          return _context.Provider.Any(e => e.Id == id);
        }
    }
}
