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

namespace BibleAppEF.Areas.Identity.Pages.Admin.Users
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
        public BibleAppEFUser GetUser;
        public string Role;
        public SelectList Options { get; set; }
        public string selectedRole { get; set; }

        public async Task<IActionResult> OnGetAsync() //not gonna delete here, change var name later.
        {
            var NameToDelete = TempData["DeleteUser"].ToString();
            var user = await _userManager.FindByNameAsync(NameToDelete);
            var roles = await _userManager.GetRolesAsync(user);
            GetUser = user;

            var Roles = _roleManager.Roles.ToList();
            

            if (roles.Count == 0)
            {
                Options = new SelectList(Roles, nameof(Roles), nameof(Roles));
            }
            else
            {
                Options = new SelectList(Roles, nameof(Roles), nameof(Roles));
                selectedRole = roles[0];

            }

            return Page();
        }

        public async Task<IActionResult> OnPostDelete(string name, string email, string password, string role)
        {
            var Roles = _roleManager.Roles.ToList();
            Options = new SelectList(Roles);

            return RedirectToPage("Index");
        }
        
 
            

    }
}
