using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BibleAppEF.Areas.Identity.Data
{
    // Add user version preferences
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string VersionsString { get; set; }
    }
}
