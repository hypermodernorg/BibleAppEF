﻿using System;
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
using System.Text.Json;
using MySqlX.XDevAPI.Relational;

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


        // Search
        public async Task<IActionResult> Search()
        {
            List<string> versionList = new List<string>();
            var availableVersions = await _context.Registers.ToListAsync();
            foreach (var versions in availableVersions)
            {
                if (versions.IsActive)
                {
                    versionList.Add(versions.Abbreviation);
                }

            }
            ViewData.Model = versionList;

            return View();
        }
        // End Search


        public string BuildQuery(string version, string book, string chapter, string verse, string wordstosearch)
        {
            string query = $"SELECT * FROM BIBLES WHERE Version = '{version}'";

            if (book != null)
            {
                query += $" and Book = '{book}'";

                if (chapter != null)
                {
                    query += $" and Chapter = '{chapter}'";
                    if (verse != null)
                    {
                        query += $" and Verse = '{verse}'";
                    }
                }
            }

            if (wordstosearch != null)
            {
                query += $" and BibleText LIKE '%{wordstosearch}%'";
            }

            return query;
        }

        // Process Search
        [HttpPost]
        public string GetBible(string version, string book, string chapter, string verse, string wordstosearch)
        {
            string queryString = BuildQuery(version, book, chapter, verse, wordstosearch);

            var bibleText = _context.Bibles
                .FromSqlRaw(queryString)
                .ToList();

            string allText = "";

            int chapterInt = 0;
            string bookCheck = "";

            foreach (var line in bibleText)
            {
                if (bookCheck != line.Book)
                {

                    allText += $"<div class='row'><div class='col text-center'><h5>{line.Book}</h5></div></div>";
                    chapterInt = 0;
                    bookCheck = line.Book;
                }

                if (chapterInt != line.Chapter)
                {
                    allText += $"<div class='row'><div class='col pt-4'><h5>Chapter {line.Chapter}</h5></div></div>";
                    chapterInt = line.Chapter;
                }

                allText += "<div class = 'row'><div class = 'col-1 no-gutters pr-0'><small>" + line.Verse + "</small></div><div class='col-11 pl-0'>" + line.BibleText + "</div></div>";
            }
            return allText;
        }
        // End Process Search


        [HttpPost]
        public string UpdateBooks(string version)
        {
            var bookList = _context.Books
                .FromSqlRaw($"SELECT Id, Book, Chapter, Verse, Version FROM bibles WHERE Version = '{version}' GROUP BY Book")
                .ToList();
            var bookWithoutCol = bookList.Select(x => new { x.Book }).ToList();

            var versionList = _context.Registers
                .FromSqlRaw($"SELECT * FROM registers WHERE Abbreviation = '{version}'")
                .ToList();
            var versionWithoutCol = versionList.Select(x => new { x.Name}).ToList();

            var objects = new { BookList = bookWithoutCol, Version = versionWithoutCol };
            //string result = new JavaScriptSerializer().Serialize(objects);

            return JsonSerializer.Serialize(objects);
        }

        [HttpPost]
        public string UpdateChapters(string version, string book)
        {
            var chapterList = _context.Books
                .FromSqlRaw($"SELECT Id, Book, Chapter, Verse, Version FROM bibles WHERE Version = '{version}' and Book = '{book}' GROUP BY Chapter")
                .ToList();

            var listWithoutCol = chapterList.Select(x => new { x.Chapter }).ToList();

            return JsonSerializer.Serialize(listWithoutCol);
        }

        [HttpPost]
        public string UpdateVerses(string version, string book, string chapter)
        {
            var verseList = _context.Books
                .FromSqlRaw($"SELECT Id, Book, Chapter, Verse, Version FROM bibles WHERE Version = '{version}' and Book = '{book}' and Chapter = '{chapter}' GROUP BY Verse")
                .ToList();

            var listWithoutCol = verseList.Select(x => new { x.Verse }).ToList();


            return JsonSerializer.Serialize(listWithoutCol);
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

            const string pattern = @"(\d{1,3}\w)\s(\d{1,3})\s(\d{1,3})\s*\d*\s*\s*([\ \S*]*)";
            //(\d{1,3}\w)\W(\d{1,3})\W(\d{1,3})\W{1,2}\d{1,7}\W{1}([\ \S*]*)";

            // Instantiate the regular expression object.
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            
            for (int i = 8; i<bibleText.Length; i++)
            {
                Match m = r.Match(bibleText[i]);

                if (m.Groups.Count >= 4)
                {
                    bible.Id = 0;
                    bible.Version = register.Abbreviation;
                    bible.BookChapterVerse = m.Groups[1].ToString() + "|" +m.Groups[2].ToString() + "|" + m.Groups[3].ToString();
                    bible.Book = m.Groups[1].ToString();
                    bible.Chapter = int.Parse(m.Groups[2].ToString());
                    bible.Verse = int.Parse(m.Groups[3].ToString());
                    bible.BibleText = m.Groups[4].ToString();
                    bibleContext.Add(bible);
                    await bibleContext.SaveChangesAsync();
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
            var abbreviation = register.Abbreviation;
            if (id != register.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(register);

                    //Bible bible = new Bible();

                    //var lines = _context.Bibles
                    //     .FromSqlRaw($"Select * from bibles where Version = '{abbreviation}'")
                    //     .ToList();


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
            var abbreviation = register.Abbreviation;
            _context.Registers.Remove(register); // registration to remove

            Bible bible = new Bible();

            var lines = _context.Bibles
                 .FromSqlRaw($"Select * from bibles where Version = '{abbreviation}'")
                 .ToList();
            _context.Bibles.RemoveRange(lines); // bible version to remove

            await _context.SaveChangesAsync(); // remove both the registration and the bible version
            return RedirectToAction(nameof(Index));
        }

        private bool RegisterExists(int id)
        {
            return _context.Registers.Any(e => e.Id == id);
        }
    }
}
