using BibleAppEF.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BibleAppEF.Areas.Identity.Pages.Admin.Roles
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class IndexModel : PageModel
    {
        private readonly UserManager<BibleAppEFUser> _userManager;
        private readonly SignInManager<BibleAppEFUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public IndexModel(UserManager<BibleAppEFUser> userManager, SignInManager<BibleAppEFUser> signInManager, RoleManager<IdentityRole> roleManager)
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
                Roles.Add(role.Id, role.Name);
            }

            AllRoles = Roles;
            return Roles;
        }

        public async Task<IActionResult> OnPostAdd(string NewRole)
        {
            IdentityRole role = new IdentityRole
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
            IdentityRole theRole = await _roleManager.FindByIdAsync(updateRole);
            await _roleManager.DeleteAsync(theRole);
            AllRoles = AllTheRoles();

            return RedirectToPage();
        }

        [TempData]
        public string UpdateRole { get; set; }

        public IActionResult OnPostEdit(string updateRole)
        {
            UpdateRole = updateRole;
            return RedirectToPage($"Edit", "Get");
        }
    }
}
