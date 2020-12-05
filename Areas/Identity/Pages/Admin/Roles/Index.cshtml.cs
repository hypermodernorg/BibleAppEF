using BibleAppEF.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace BibleAppEF.Areas.Identity.Pages.Admin.Roles
{
    [Authorize("ViewRoles")]
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;


        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public Dictionary<string, string> AllRoles { get; set; }

        
        public IActionResult OnGet()
        {
            AllRoles = AllTheRoles();
            return Page();
        }

        public Dictionary<string, string> AllTheRoles()
        {
            Dictionary<string, string> Roles = new Dictionary<string, string>();
            foreach (var role in _roleManager.Roles)
            {
                Roles.Add(role.Id.ToString(), role.Name);
            }

            AllRoles = Roles;
            return Roles;
        }

        public async Task<IActionResult> OnPostAdd(string NewRole)
        {
            ApplicationRole role = new ApplicationRole
            {
                Name = NewRole,
                NormalizedName = NewRole.ToUpper()
            };
            await _roleManager.CreateAsync(role);
            AllRoles = AllTheRoles();
            return Page();
        }

        public async Task<IActionResult> OnPostDelete(string updateRole)
        {
            ApplicationRole theRole = await _roleManager.FindByIdAsync(updateRole);
            await _roleManager.DeleteAsync(theRole);
            AllRoles = AllTheRoles();

            return RedirectToPage();
        }

        [TempData]
        public Guid UpdateRole { get; set; }

        public IActionResult OnPostEdit(Guid updateRole)
        {
            UpdateRole = updateRole;
            return RedirectToPage($"Edit", "Get");
        }
    }
}
