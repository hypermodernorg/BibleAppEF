using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BibleAppEF.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<Guid>
    {
        //[NotMapped]
        //public List<string> VersionsList { get; set; }

        //public string VersionsString
        //{
        //    get => string.Join(",", VersionsString);
        //    set => VersionsList = value.Split(',').ToList();
        //}
    }
}
