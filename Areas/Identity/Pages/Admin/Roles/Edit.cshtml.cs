using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BibleAppEF.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BibleAppEF.Areas.Identity.Pages.Admin.Roles.Edit
{
    public class EditModel : PageModel
    {
        private readonly UserManager<BibleAppEFUser> _userManager;
        private readonly SignInManager<BibleAppEFUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public EditModel(UserManager<BibleAppEFUser> userManager, SignInManager<BibleAppEFUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        public string theKey { get; set; }
        public string theValue { get; set; }
        public string theValueNorm { get; set; }
        public async Task<IActionResult> RoleToDelete(string RoleId)
        {
            IdentityRole theRole = await _roleManager.FindByIdAsync(RoleId).ConfigureAwait(false);

            theKey = theRole.Id;
            theValue = theRole.Name;
            theValueNorm = theRole.NormalizedName;
            return RedirectToPage("Edit");
        }


        // Because the below will not update the page, item is removed through 
        // site.js javascript function DeleteRole(). May come back to this later
        // for a better solution
        public async Task<IActionResult> OnPostUpdate(string RoleId)
        {
            IdentityRole theRole = await _roleManager.FindByIdAsync(RoleId).ConfigureAwait(false);
            await _roleManager.UpdateAsync(theRole);

            return Page();
        }
    }
}
