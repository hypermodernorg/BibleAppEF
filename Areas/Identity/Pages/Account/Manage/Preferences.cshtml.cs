using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BibleAppEF.Areas.ImportBible.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BibleAppEF.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;


namespace BibleAppEF.Areas.Identity.Pages.Account.Manage
{
    public class PreferencesModel : PageModel
    {

        public class VersionsSelected
        {
            public string Version { get; set; }
            public bool Selected { get; set; }
        }
        // Get the context to get a list of the available bible versions
        private readonly BibleContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public List<VersionsSelected> VersionList { get; set; }

        public PreferencesModel(BibleContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            List<VersionsSelected> versionList = new List<VersionsSelected>();
            var availableVersions = await _context.Registers.ToListAsync();
            foreach (var versions in availableVersions)
            {
                if (versions.IsActive)
                {
                    var versionlist = new VersionsSelected();
                    versionlist.Version = versions.Abbreviation;
    
                    versionlist.Selected = false;

                    if (user.VersionsString != null) {
                        foreach (var userVersion in user.VersionsString.Split().ToList())
                        {
                            if (userVersion == versions.Abbreviation)
                            {
                                versionlist.Selected = true;
                            }
                        }
                        versionList.Add(versionlist);
                    }
                    else
                    {
                        versionList.Add(versionlist);
                    }
                }
            }
            VersionList = versionList;
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            string versionString = "";
            foreach(var version in VersionList)
            {
                if (version.Selected)
                {
                    versionString += version.Version + " ";
                }
                
            }
            user.VersionsString = versionString;
            await _userManager.UpdateAsync(user);
            return RedirectToPage();
        }
    }
}
