using BibleAppEF.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace BibleAppEF.Areas.Identity.Pages.Admin.Roles
{
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public ApplicationRole GetRole;

        //public IList<Claim> Claims { get; set; }

        public List<string> AllClaims { get; set; }

        public void ClaimTypes()
        {
            List<string> listclaims = new List<string>
            {
                "CanViewRoles",
                "CanAddRoles",
                "CanEditRoles",
                "CanDeleteRoles"
            };
            AllClaims = listclaims;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var RoleToUpdate = TempData["UpdateRole"].ToString();
            GetRole = await _roleManager.FindByIdAsync(RoleToUpdate);
            //await _roleManager.AddClaimAsync(GetRole, new Claim("CanViewRoles", "CanViewRoles"));
            //Claims = await _roleManager.GetClaimsAsync(GetRole);
            //ClaimTypes();
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
