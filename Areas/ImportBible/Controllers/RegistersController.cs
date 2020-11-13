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
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        public async Task<IActionResult> Search(string? id)
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


        public string BuildQuery(string version, string book, string chapter, string verse, string wordstosearch, string searchmode, string nottosearch, string notmode)
        {
            string query = $"SELECT * FROM BIBLES WHERE (Version = '{version}')";

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
                bool bor = false;
                bool band = false;
                if (searchmode == "phrase" || wordstosearch.Split().ToArray().Length < 2)
                {
                    query += $" AND BibleText LIKE '%{wordstosearch}%'";
                }
                else if (searchmode == "and" && wordstosearch.Split().ToArray().Length > 1)
                {
                    var wtsArray = wordstosearch.Split().ToArray();
                    foreach (string word in wtsArray)
                    {

                        if (band == false)
                        {
                            query += $" AND (BibleText LIKE '%{word}%'";
                            band = true;
                        }
                        else
                        {
                            query += $" AND BibleText LIKE '%{word}%'";
                        }

                   
                    }
                    query += ")";

                }

                // SELECT * FROM BIBLES WHERE (Version = 'asv') and (BibleText LIKE '%David%' or BibleText LIKE '%Jesus%');
                else if (searchmode == "or" && wordstosearch.Split().ToArray().Length > 1)
                {
                    var wtsArray = wordstosearch.Split().ToArray();
                    foreach (string word in wtsArray)
                    {
                        if (bor == false)
                        {
                            query += $" AND (BibleText LIKE '%{word}%'";
                        }
                        else
                        {
                            query += $" OR BibleText LIKE '%{word}%'";
                        }
                        bor = true;
                    }
                    query += ")";
                }
            }

            // Begin not to search
            if (nottosearch != null)
            {
                bool bor = false;
                bool band = false;
                if (notmode == "phrase" || nottosearch.Split().ToArray().Length < 2)
                {
                    query += $" AND (BibleText NOT LIKE '%{nottosearch}%')";
                }
                else if (notmode == "and" && nottosearch.Split().ToArray().Length > 1)
                {
                    var wtsArray = nottosearch.Split().ToArray();
                    foreach (string word in wtsArray)
                    {
                        if (band == false)
                        {
                            query += $" AND (BibleText NOT LIKE '%{word}%'";
                            band = true;
                        }
                        else
                        {
                            query += $" AND BibleText NOT LIKE '%{word}%'";
                        }
                        
                    }
                    query += ")";

                }

                // SELECT * FROM BIBLES WHERE (Version = 'asv') and (BibleText LIKE '%David%' or BibleText LIKE '%Jesus%');
                else if (notmode == "or" && nottosearch.Split().ToArray().Length > 1)
                {
                    var wtsArray = nottosearch.Split().ToArray();
                    foreach (string word in wtsArray)
                    {
                        if (bor == false)
                        {
                            query += $" AND (BibleText NOT LIKE '%{word}%'";
                        }
                        else
                        {
                            query += $" OR BibleText NOT LIKE '%{word}%'";
                        }
                        bor = true;
                    }
                    query += ")";
                }
            } // End not to search

            return query;
        }

        // Process Search
        [HttpPost]
        public string GetBible(string version, string book, string chapter, string verse, string wordstosearch, string searchmode, string nottosearch, string notmode)
        {
            string queryString = BuildQuery(version, book, chapter, verse, wordstosearch, searchmode, nottosearch, notmode);

            var bibleText = _context.Bibles
                .FromSqlRaw(queryString)
                .ToList();

            string allText = "";

            string chapterInt = "";
            string bookCheck = "";

            foreach (var line in bibleText)
            {
                if (bookCheck != line.Book)
                {

                    allText += $"<div class='row'><div class='col pt-4 text-center'><h5>{BookDictionary(line.Book)}</h5></div></div>";
                    chapterInt = "";
                    bookCheck = line.Book;
                }

                if (chapterInt != line.Chapter)
                {
                    allText += $"<div class='row'><div class='col pt-4'><h5>Chapter {line.Chapter}</h5></div></div>";
                    chapterInt = line.Chapter;
                }



                allText += "<div class = 'row'><div class = 'col-1 no-gutters pr-0'><small>" + line.Verse + "</small></div><div class='col-11 pl-0'>" + ParseText(line.BibleText, wordstosearch, searchmode) + "</div></div>";
            }
            return allText;
        }
        // End Process Search

        public string ParseText(string text, string search, string searchmode)
        {
            string [] textArray = text.Split();
            string newText = "";

            if (search != null)
            {
                var searchArray = search.Split().ToArray();
                const string pattern = @"\w[\w\d\W]*\w";
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                //if (searchmode == "and" || searchmode == "or" || search.Split().ToArray().Length == 1) {
                    foreach (var word in textArray)
                    {
                        Match m = r.Match(word);

                        if (m.Length > 3)
                        {
                            int i = 0;
                            foreach (var searchWord in searchArray)
                            {
                                if (searchWord == m.ToString())
                                {
                                    newText += $"<a class=\"searchword\"onclick=\"LinkSearch('{m}');\">{word}</a> ";
                                    i = 1;
                                }
                            }
                            if (i == 0)
                            {
                                newText += $"<a onclick=\"LinkSearch('{m}');\">{word}</a> ";
                            }
                        }
                        else
                        {
                            newText += $"{word} ";
                        }
                    }
                //}
            }
            else // If no search words
            {
                const string pattern = @"\w[\w\d\W]*\w";
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                //if (m.Groups.Count >= 4)

                foreach (var word in textArray)
                {
                    Match m = r.Match(word);

                    if (m.Length > 3)
                    {
                        newText += $"<a onclick=\"LinkSearch('{m}');\">{word}</a> ";
                    }
                    else
                    {
                        newText += $"{word} ";
                    }
                }
            }
            return newText;
        }


        public string BookDictionary(string bookKey)
        {
            Dictionary<string, string> bookDictionary = new Dictionary<string, string>()
            {
                { "01O", "Genesis" },
                { "02O", "Exodus" },
                { "03O", "Leviticus" },
                { "04O", "Numbers" },
                { "05O", "Deuteronomy" },
                { "06O", "Joshua" },
                { "07O", "Judges" },
                { "08O", "Ruth" },
                { "09O", "1 Samuel" },
                { "10O", "2 Samuel" },
                { "11O", "1 Kings" },
                { "12O", "2 Kings" },
                { "13O", "1 Chronicles" },
                { "14O", "2 Chronicles" },
                { "15O", "Ezra" },
                { "16O", "Nehemiah" },
                { "17O", "Esther" },
                { "18O", "Job" },
                { "19O", "Psalms" },
                { "20O", "Proverbs" },
                { "21O", "Ecclesiastes" },
                { "22O", "Song of Solomon" },
                { "23O", "Isaiah" },
                { "24O", "Jeremiah" },
                { "25O", "Lamentations" },
                { "26O", "Ezekiel" },
                { "27O", "Daniel" },
                { "28O", "Hosea" },
                { "29O", "Joel" },
                { "30O", "Amos" },
                { "31O", "Obadiah" },
                { "32O", "Jonah" },
                { "33O", "Micah" },
                { "34O", "Nahum" },
                { "35O", "Habakkuk" },
                { "36O", "Zephaniah" },
                { "37O", "Haggai" },
                { "38O", "Zechariah" },
                { "39O", "Malachi" },
                { "40N", "Matthew" },
                { "41N", "Mark" },
                { "42N", "Luke" },
                { "43N", "John" },
                { "44N", "Acts of the Apostles" },
                { "45N", "Romans" },
                { "46N", "1 Corinthians" },
                { "47N", "2 Corinthians" },
                { "48N", "Galatians" },
                { "49N", "Ephesians" },
                { "50N", "Philippians" },
                { "51N", "Colossians" },
                { "52N", "1 Thessalonians" },
                { "53N", "2 Thessalonians" },
                { "54N", "1 Timothy" },
                { "55N", "2 Timothy" },
                { "56N", "Titus" },
                { "57N", "Philemon" },
                { "58N", "Hebrews" },
                { "59N", "James" },
                { "60N", "1 Peter" },
                { "61N", "2 Peter" },
                { "62N", "1 John" },
                { "63N", "2 John" },
                { "64N", "3 John" },
                { "65N", "Jude" },
                { "66N", "Revelation" },
                { "67A", "Tobit" },
                { "68A", "Judith" },
                { "69A", "Esther (Greek)" },
                { "70A", "Wisdom of Solomon" },
                { "71A", "Ecclesiasticus (Sira)" },
                { "72A", "Baruch" },
                { "73A", "Epistle of Jeremiah" },
                { "74A", "Prayer of Azariah" },
                { "75A", "Susanna" },
                { "76A", "Bel and the Dragon" },
                { "77A", "1 Maccabees" },
                { "78A", "2 Maccabees" },
                { "79A", "3 Maccabees" },
                { "80A", "4 Maccabees" },
                { "81A", "1 Esdras" },
                { "82A", "2 Esdras" },
                { "83A", "Prayer of Manasseh" },
                { "84A", "Psalm 151" },
                { "85A", "Psalm of Solomon" },
                { "86A", "Odes" }
            };

            return bookDictionary[bookKey];
        }



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
                    bible.Chapter = m.Groups[2].ToString();
                    bible.Verse = m.Groups[3].ToString();
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
