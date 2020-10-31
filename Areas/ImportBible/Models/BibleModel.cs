﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BibleAppEF.Areas.ImportBible.Models
{
    public class Register
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string Source { get; set; }
        [Required][Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string FileType { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string Copyright { get; set; }
        [Required][Column(TypeName = "varchar(40)")]
        public string Abbreviation { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string Language { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
    }
    public class Bible
    {
        public int Id { get; set; }
        [Required][Column(TypeName = "varchar(40)")]
        public string Version { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string BookChapterVerse { get; set; }
        [Column(TypeName = "varchar(40)")]
        public string Book { get; set; }
        [Column(TypeName = "varchar(40)")]
        public int Chapter { get; set; }
        [Column(TypeName = "varchar(40)")]
        public int Verse { get; set; }
        public string BibleText { get; set; }
    }
}
