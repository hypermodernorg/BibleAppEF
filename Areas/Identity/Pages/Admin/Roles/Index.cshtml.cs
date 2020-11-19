using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BibleAppEF.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BibleAppEF.Areas.Identity.Pages.Admin.Roles
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<BibleAppEFUser> _userManager;
        private readonly SignInManager<BibleAppEFUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public IndexModel(UserManager<BibleAppEFUser> userManager, SignInManager<BibleAppEFUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public Dictionary<string, string> AllRoles { get; set; }

        public IActionResult OnGet() {

            Dictionary<string, string> Roles = new Dictionary<string, string>();
            foreach (var role in _roleManager.Roles)
            {
                Roles.Add(role.Id, role.Name);
            }

            AllRoles = Roles;
            return Page();
        }
    }
}
