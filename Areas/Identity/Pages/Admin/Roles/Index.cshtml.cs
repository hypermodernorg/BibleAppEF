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

        public async void Add(string NewRole)
        {
            IdentityRole role = new IdentityRole();
            role.Name = NewRole;
            await _roleManager.CreateAsync(role);
        }

        public IActionResult OnPost(string NewRole)
        {
            Add(NewRole);
            return RedirectToPage();
        }

        public async void Deleted(string RoleId)
        {
            IdentityRole theRole = await _roleManager.FindByIdAsync(RoleId);
            await _roleManager.DeleteAsync(theRole);
        }


        public IActionResult OnPostDelete(string RoleId)
        {
            Deleted(RoleId);
            return RedirectToPage();
        }
    }
}
