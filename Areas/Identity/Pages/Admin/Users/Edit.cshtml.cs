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
