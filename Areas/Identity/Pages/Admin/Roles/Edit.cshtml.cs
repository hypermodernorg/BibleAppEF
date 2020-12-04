using BibleAppEF.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace BibleAppEF.Areas.Identity.Pages.Admin.Roles
{
    public class RoleClaim
    {
        public string claim { get; set; }
        public bool Selected { get; set; }
    }
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public ApplicationRole GetRole;
        public List<Claim> Claims { get; set; }
        public List<string> AllClaims { get; set; }

        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public List<RoleClaim> RoleClaims { get; set; }

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
            List<RoleClaim> roleClaims = new List<RoleClaim>();
            var RoleToUpdate = TempData["UpdateRole"].ToString();
            GetRole = await _roleManager.FindByIdAsync(RoleToUpdate);
            //await _roleManager.AddClaimAsync(GetRole, new Claim("CanViewRoles", "CanViewRoles"));
            Claims = await _roleManager.GetClaimsAsync(GetRole) as List<Claim>;
            ClaimTypes();
            foreach (var claim in AllClaims)
            {
                if (Claims.Exists(x => x.Type == claim))
                {
                    RoleClaim newClaim = new RoleClaim
                    {
                        claim = claim,
                        Selected = true
                    };
                    roleClaims.Add(newClaim);
                }
                else
                {
                    RoleClaim newClaim = new RoleClaim
                    {
                        claim = claim,
                        Selected = false
                    };
                    roleClaims.Add(newClaim);
                }
            }
            RoleClaims = roleClaims;

            ClaimTypes();
            return Page();
        }

        public async void UpdateRolePermissions(ApplicationRole role)
        {
            Claims = await _roleManager.GetClaimsAsync(role) as List<Claim>;
            

            foreach (var roleClaim in RoleClaims)
            {
                var testselect = roleClaim.Selected;
                var testclami = roleClaim.claim;
                //if (roleClaim.Selected && !Claims.Exists(x => x.Type == roleClaim.claim))
                if (roleClaim.Selected)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(roleClaim.claim, roleClaim.claim));
                }
                //else if (!roleClaim.Selected && Claims.Exists(x => x.Type == roleClaim.claim))
                //{
                //    await _roleManager.RemoveClaimAsync(role, Claims.Find(x=> x.Type.Contains(roleClaim.claim)));
                //}
                //await _roleManager.AddClaimAsync(role, new Claim("CanViewRoles", "CanViewRoles"));
            }
        }

        public async Task<IActionResult> OnPostUpdate(string roleId, string roleName)
        {
            GetRole = await _roleManager.FindByIdAsync(roleId);
            GetRole.Name = roleName;
            
            Claims = await _roleManager.GetClaimsAsync(GetRole) as List<Claim>;


            foreach (var roleClaim in RoleClaims)
            {
                var testselect = roleClaim.Selected;
                var testclami = roleClaim.claim;
                if (roleClaim.Selected && !Claims.Exists(x => x.Type == roleClaim.claim))
                //if (roleClaim.Selected)
                {
                    await _roleManager.AddClaimAsync(GetRole, new Claim(roleClaim.claim, roleClaim.claim));
                }
                else if (!roleClaim.Selected && Claims.Exists(x => x.Type == roleClaim.claim))
                {
                    await _roleManager.RemoveClaimAsync(GetRole, Claims.Find(x => x.Type.Contains(roleClaim.claim)));
                }
                //await _roleManager.AddClaimAsync(role, new Claim("CanViewRoles", "CanViewRoles"));
            }
            await _roleManager.UpdateAsync(GetRole);
            return RedirectToPage("Index");
        }
    }


}
