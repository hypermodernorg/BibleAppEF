using BibleAppEF.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace BibleAppEF.Areas.Identity.Pages.Admin.Users
{
    [Authorize("EditUsers")]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;


        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public ApplicationUser GetUser;
        public string Role;
        public List<SelectListItem> Options { get; set; }
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
                Options = new List<SelectListItem>();
                foreach (var role in Roles)
                {

                    Options.Add(new SelectListItem { Value = role.Name, Text = role.Name });

                }
            }
            else
            {
                Options = new List<SelectListItem>();

                foreach (var role in Roles)
                {
                    if (role.Name == roles[0])
                    {
                        Options.Add(new SelectListItem { Value = role.Name, Text = role.Name, Selected = true });
                    }
                    else
                    {
                        Options.Add(new SelectListItem { Value = role.Name, Text = role.Name });

                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdate(string name, string email, string password, string role, string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count > 0)
            {
                await _userManager.RemoveFromRoleAsync(user, roles[0]);
            }


            if (role != "" && role != null)
            {
                await _userManager.AddToRoleAsync(user, role);
            }



            if (name != "" || name != null)
            {
                await _userManager.SetUserNameAsync(user, name);
            }

            if (email != "" || email != null)
            {
                await _userManager.SetEmailAsync(user, email);
            }

            //if (password != "" || password != null)
            //{
            //    await _userManager.RemovePasswordAsync(user);
            //    await _userManager.AddPasswordAsync(user, password);
            //}

            return RedirectToPage("Index");
        }




    }
}
