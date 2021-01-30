using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.Data;
using Website.Models;

namespace Website.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AdminController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind("Email,Password,RememberMe")] InputLoginModel input, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(input.Email, input.Password, input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult UserList() => View(_userManager.Users.ToList());

        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> ChangePasswordAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            InputChangePasswordModel model = new InputChangePasswordModel
            {
                Email = user.Email
            };

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> ChangePassword(InputChangePasswordModel input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user == null)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }
            var result = await _userManager.ChangePasswordAsync(user, input.Password, input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction("Index", "Home");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);



                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(nameof(Login));
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()=> View();

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ContactMeResponses()
        {
            var contactModels = await _context.ContactModel.ToListAsync();
            if (contactModels == null)
            {
                return NotFound();
            }

            return View(contactModels);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(InputChangePasswordModel input)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = input.Email, Email = input.Email };
                var result = await _userManager.CreateAsync(user, input.Password);
                if (result.Succeeded)
                {
                    return Redirect(nameof(UserList));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.DeleteAsync(user);
            var userIdGot = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userIdGot}'.");
            }
            return Redirect(nameof(UserList));
        }
    }
}
