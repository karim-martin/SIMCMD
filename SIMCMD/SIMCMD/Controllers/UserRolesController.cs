using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIMCMD.Models;
using static Google.Cloud.Speech.V1.LanguageCodes;

namespace SIMCMD.Controllers
{
    [Authorize(Roles = "Root, Admin")]
    public class UserRolesController : Controller
    {
        private readonly UserManager<IdentityExtendUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesController(UserManager<IdentityExtendUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public IActionResult Create()
        {
            var roles = _roleManager.Roles.Select(r => new RoleViewModel { Id = r.Id, Name = r.Name }).ToList();

            var model = new UserViewModel
            {
                Roles = roles,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityExtendUser
                {
                    FullName = model.FullName,
                    UserName = model.UserName,
                    Email = model.Email,
                    AccountStatus = model.AccountStatus,
                    EmailConfirmed = model.EmailConfirmed
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var role = await _roleManager.FindByNameAsync(model.Role);

                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            var roles = _roleManager.Roles.Select(r => new RoleViewModel { Id = r.Id, Name = r.Name }).ToList();
            model.Roles = roles;

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = _roleManager.Roles.Select(r => new RoleViewModel { Id = r.Id, Name = r.Name }).ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                AccountStatus = user.AccountStatus,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles,
                Role = userRoles.FirstOrDefault()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user == null)
                {
                    return NotFound();
                }

                user.FullName = model.FullName;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.AccountStatus = model.AccountStatus;
                user.EmailConfirmed = model.EmailConfirmed;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var role = await _roleManager.FindByNameAsync(model.Role);

                    if (role != null && !userRoles.Contains(role.Name))
                    {
                        // Remove user from all roles
                        var allRoles = _roleManager.Roles.ToList();
                        foreach (var r in allRoles)
                        {
                            await _userManager.RemoveFromRoleAsync(user, r.Name);
                        }

                        // Add user to selected role
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            var roles = _roleManager.Roles.Select(r => new RoleViewModel { Id = r.Id, Name = r.Name }).ToList();
            model.Roles = roles;

            return View(model);
        }  
    }
}