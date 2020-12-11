using BibleAppEF.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace BibleAppEF.Areas.Identity.Pages.Admin.Roles
{
    public class RoleClaim
    {
        public string claim { get; set; }
        public bool Selected { get; set; }
    }
    
    [Authorize("EditRoles")]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ApplicationRole GetRole;
        public List<Claim> Claims { get; set; }
        public List<string> AllRoleClaims { get; set; }
        public List<string> AllUserClaims { get; set; }
        public List<string> AllVersionClaims { get; set; }

        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

        }

        [BindProperty]
        public List<RoleClaim> RoleClaims { get; set; }
        [BindProperty]
        public List<RoleClaim> UserClaims { get; set; }
        [BindProperty]
        public List<RoleClaim> VersionClaims { get; set; }

        public void RoleClaimTypes()
        {
            AllRoleClaims = new List<string>
            {
                "CanViewRoles",
                "CanAddRoles",
                "CanEditRoles",
                "CanDeleteRoles"
            };
        }
        public void UserClaimTypes()
        {
            AllUserClaims = new List<string>
            {
                "CanViewUsers",
                "CanAddUsers",
                "CanEditUsers",
                "CanDeleteUsers"
            };
        }

        public void VersionClaimTypes()
        {
            AllVersionClaims = new List<string>
            {
                "CanViewVersions",
                "CanAddVersions",
                "CanEditVersions",
                "CanDeleteVersions"
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            List<RoleClaim> roleClaims = new List<RoleClaim>();
            List<RoleClaim> userClaims = new List<RoleClaim>();
            List<RoleClaim> versionClaims = new List<RoleClaim>();
            var RoleToUpdate = TempData["UpdateRole"].ToString();
            GetRole = await _roleManager.FindByIdAsync(RoleToUpdate);
            Claims = await _roleManager.GetClaimsAsync(GetRole) as List<Claim>;
            RoleClaimTypes();
            UserClaimTypes();
            VersionClaimTypes();

            foreach (var claim in AllRoleClaims)
            {
                if (claim.Contains("Roles"))
                {
                    /////
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
                    ////
                }
            }
            foreach (var claim in AllUserClaims)
            {
                if (claim.Contains("Users"))
                {
                    /////
                    if (Claims.Exists(x => x.Type == claim))
                    {
                        RoleClaim newClaim = new RoleClaim
                        {
                            claim = claim,
                            Selected = true
                        };
                        userClaims.Add(newClaim);
                    }
                    else
                    {
                        RoleClaim newClaim = new RoleClaim
                        {
                            claim = claim,
                            Selected = false
                        };
                        userClaims.Add(newClaim);
                    }
                    ////
                }
            }

            foreach (var claim in AllVersionClaims)
            {
                if (claim.Contains("Version"))
                {
                    /////
                    if (Claims.Exists(x => x.Type == claim))
                    {
                        RoleClaim newClaim = new RoleClaim
                        {
                            claim = claim,
                            Selected = true
                        };
                        versionClaims.Add(newClaim);
                    }
                    else
                    {
                        RoleClaim newClaim = new RoleClaim
                        {
                            claim = claim,
                            Selected = false
                        };
                        versionClaims.Add(newClaim);
                    }
                    ////
                }
            }

            RoleClaims = roleClaims;
            UserClaims = userClaims;
            VersionClaims = versionClaims;

            return Page();
        }

        public async void UpdateRolePermissions(ApplicationRole role)
        {
            Claims = await _roleManager.GetClaimsAsync(role) as List<Claim>;

            foreach (var roleClaim in RoleClaims)
            {
                if (roleClaim.Selected)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(roleClaim.claim, roleClaim.claim));
                }
            }
            foreach (var userClaim in UserClaims)
            {
                if (userClaim.Selected)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(userClaim.claim, userClaim.claim));
                }
            }
            foreach (var versionClaim in VersionClaims)
            {
                if (versionClaim.Selected)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(versionClaim.claim, versionClaim.claim));
                }
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
                {
                    await _roleManager.AddClaimAsync(GetRole, new Claim(roleClaim.claim, roleClaim.claim));
                }
                else if (!roleClaim.Selected && Claims.Exists(x => x.Type == roleClaim.claim))
                {
                    await _roleManager.RemoveClaimAsync(GetRole, Claims.Find(x => x.Type.Contains(roleClaim.claim)));
                }
            }
            foreach (var userClaim in UserClaims)
            {
                var testselect = userClaim.Selected;
                var testclami = userClaim.claim;
                if (userClaim.Selected && !Claims.Exists(x => x.Type == userClaim.claim))
                {
                    await _roleManager.AddClaimAsync(GetRole, new Claim(userClaim.claim, userClaim.claim));
                }
                else if (!userClaim.Selected && Claims.Exists(x => x.Type == userClaim.claim))
                {
                    await _roleManager.RemoveClaimAsync(GetRole, Claims.Find(x => x.Type.Contains(userClaim.claim)));
                }
            }
            foreach (var versionClaim in VersionClaims)
            {
                var testselect = versionClaim.Selected;
                var testclami = versionClaim.claim;
                if (versionClaim.Selected && !Claims.Exists(x => x.Type == versionClaim.claim))
                {
                    await _roleManager.AddClaimAsync(GetRole, new Claim(versionClaim.claim, versionClaim.claim));
                }
                else if (!versionClaim.Selected && Claims.Exists(x => x.Type == versionClaim.claim))
                {
                    await _roleManager.RemoveClaimAsync(GetRole, Claims.Find(x => x.Type.Contains(versionClaim.claim)));
                }
            }

            await _roleManager.UpdateAsync(GetRole);
            return RedirectToPage("Index");
        }
    }
}
