﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SIMCMD.Models;

namespace SIMCMD.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityExtendUser> _userManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityExtendUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;    

        public LoginModel(IConfiguration config, RoleManager<IdentityRole> roleManager, SignInManager<IdentityExtendUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<IdentityExtendUser> userManager)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        //public LoginModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //Create the "Admin" role if it doesn't already exist
            if (!_roleManager.RoleExistsAsync("Root").Result)
            {
                string[] array = {  "Root", "Admin", "Claims"};
                foreach (var item in array)
                {
                    var role = new IdentityRole(item);
                    await _roleManager.CreateAsync(role);
                }               
            }

            //Create the default admin user if it doesn't already exist
            var adminCredentialsUser = _config["ServiceSetting:AdminCredentials:Username"];
            var adminCredentialsPass = _config["ServiceSetting:AdminCredentials:Password"];
            var adminCredentialsEmail = _config["ServiceSetting:AdminCredentials:Email"];
            var adminUser = await _userManager.FindByNameAsync(adminCredentialsUser);
            if (adminUser == null)
            {
                adminUser = new IdentityExtendUser {EmailConfirmed = true, FullName = "Administrator", AccountStatus = "Enable", UserName = adminCredentialsUser, Email = adminCredentialsEmail};
                var result = await _userManager.CreateAsync(adminUser, adminCredentialsPass);
                if (result.Succeeded)
                {
                    //Assign the "Admin" role to the admin user
                    await _userManager.AddToRoleAsync(adminUser, "Root");
                }
            }

            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (_userManager.FindByEmailAsync(Input.Email).Result.AccountStatus == "Enable")
                    {
                        _logger.LogInformation($"{User.Identity.Name} logged in.");
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check with you adminstrator.");
                        return Page();
                    }                   
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning($"{User.Identity.Name} account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check with you adminstrator.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
