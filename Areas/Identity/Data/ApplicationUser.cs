using Microsoft.AspNetCore.Identity;
using System;

namespace BibleAppEF.Areas.Identity.Data
{
    // Add user version preferences
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string VersionsString { get; set; }
    }
}
