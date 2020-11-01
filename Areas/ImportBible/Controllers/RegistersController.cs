using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibleAppEF.Areas.ImportBible.Data;
using BibleAppEF.Areas.ImportBible.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace BibleAppEF.Areas.ImportBible.Controllers
{
    [Area("ImportBible")]
    public class RegistersController : Controller
    {
        private readonly BibleContext _context;
        private readonly IWebHostEnvironment _env;

        public RegistersController(BibleContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: ImportBible/Registers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Registers.ToListAsync());
        }

        // GET: ImportBible/Registers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var register = await _context.Registers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (register == null)
            {
                return NotFound();
            }

            return View(register);
        }

        // GET: ImportBible/Registers/Create
        public IActionResult Create()
        {
            return View();
        }

        public async void InsertBibleText(string[] bibleText, Register register)
        {
            Bible bible = new Bible();

            BibleContext bibleContext = new BibleContext();


            //string pattern = @"(\w*)\W(\w)\W(\w)\W\W\w*\W([\s\S]*)";
         
            string pattern = @"(\d{1,3}\w)\W(\d{1,3})\W(\d{1,3})\W{1,2}\d{1,7}\W{1}([\ \S*]*)";
            // Instantiate the regular expression object.
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            
            for (int i = 8; i<bibleText.Length; i++)
            {
                Match m = r.Match(bibleText[i]);

                if (m.Groups.Count >= 4)
                {
                    bible.Id = 0;
                    bible.Version = register.Abbreviation;
                    bible.BookChapterVerse = m.Groups[1].ToString() + m.Groups[2].ToString() + m.Groups[3].ToString();
                    bible.Book = m.Groups[1].ToString();
                    bible.Chapter = int.Parse(m.Groups[2].ToString()); 
                    bible.Verse = int.Parse(m.Groups[3].ToString());
                    bible.BibleText = m.Groups[4].ToString();
                    bibleContext.Add(bible);
                    await bibleContext.SaveChangesAsync();
                  
                   // await _context.Database.ExecuteSqlRawAsync($"INSERT INTO table_name (Version, BookChapterVerse, Book, Chapter, Verse, BibleText) VALUES({bible.Version}, {bible.BookChapterVerse}, {bible.Book}, {bible.Chapter}, {bible.Verse}, {bible.BibleText}); ");
                }

            }

        }


        // POST: ImportBible/Registers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Source,Name,FileType,Copyright,Abbreviation,Language,Note,IsActive")] Register register, IFormFile file)
        {
            // Begin get and save file.
            string contentRootPath = _env.ContentRootPath;
            string webroot = _env.WebRootPath;
            contentRootPath = webroot;
            string uploads = Path.Combine(contentRootPath, "uploads");
            string filePath = Path.Combine(uploads, file.FileName);
            using (Stream stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            // End get and save file.

            string[] bibleText = System.IO.File.ReadAllLines(filePath);

            int lineCount = bibleText.Length;
            

            if (ModelState.IsValid)
            {
                _context.Add(register);
                await _context.SaveChangesAsync();
                InsertBibleText(bibleText, register);
                return RedirectToAction(nameof(Index));
            }
            
            return View(register);
        }

        // GET: ImportBible/Registers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var register = await _context.Registers.FindAsync(id);
            if (register == null)
            {
                return NotFound();
            }
            return View(register);
        }

        // POST: ImportBible/Registers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Source,Name,FileType,Copyright,Abbreviation,Language,Note,IsActive")] Register register)
        {
            if (id != register.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(register);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegisterExists(register.Id))
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
            return View(register);
        }

        // GET: ImportBible/Registers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var register = await _context.Registers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (register == null)
            {
                return NotFound();
            }

            return View(register);
        }

        // POST: ImportBible/Registers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var register = await _context.Registers.FindAsync(id);
            _context.Registers.Remove(register);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegisterExists(int id)
        {
            return _context.Registers.Any(e => e.Id == id);
        }
    }
}
