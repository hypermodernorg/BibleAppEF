using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BibleAppEF.Areas.Identity.Data;
using BibleAppEF.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BibleAppEF.Areas.Identity.Pages.Admin.Roles
{
    public class EditModel : PageModel
    {
        private readonly UserManager<BibleAppEFUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        

        public EditModel(UserManager<BibleAppEFUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IdentityRole GetRole;
        public async Task<IActionResult> OnGetAsync()
        {
            var RoleToUpdate = TempData["UpdateRole"].ToString();
            GetRole = await _roleManager.FindByIdAsync(RoleToUpdate);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdate(string roleId, string roleName)
        {
            GetRole = await _roleManager.FindByIdAsync(roleId);
            GetRole.Name = roleName;
            await _roleManager.UpdateAsync(GetRole);
            return RedirectToPage("Index");
        }
    }
}
