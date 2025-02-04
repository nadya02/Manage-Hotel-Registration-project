﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }   
            [Required]
            [Display(Name = "Middle Name")]
            public string MiddleName { get; set; } 
            [Required]
            [Display(Name = "Surame")]
            public string SurName { get; set; }   
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }  
            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; }
            [Required]
            [StringLength(10, MinimumLength = 10)]
            [Display(Name = "EGN")]
            public string EGN { get; set; }
            [Required]
            [Phone]
            [StringLength(10, MinimumLength = 10)]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; } 

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            return Task.CompletedTask;
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            
            if (ModelState.IsValid)
            {
                var user = new User {Id = Guid.NewGuid().ToString(), FirstName = Input.FirstName, MiddleName = Input.MiddleName
                , Surname = Input.SurName, Email = Input.Email, UserName = Input.Email, EGN = Input.EGN
                , PhoneNumber = Input.PhoneNumber, IsActive = true
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {     
                   /* if(this._userManager.Users.Count() == 1)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Employee");
                    }*/
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
