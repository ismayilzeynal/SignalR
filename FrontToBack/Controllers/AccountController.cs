using FrontToBack.Helpers;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FrontToBack.Services.Email;

namespace FrontToBack.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICreateEmailFile _createEmailFile;
        private readonly IEmailSend _emailSend;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, ICreateEmailFile createEmailFile, IEmailSend emailSend)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _createEmailFile = createEmailFile;
            _emailSend = emailSend;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost][AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            AppUser user = new AppUser
            {
                Fullname = registerVM.Fullname,
                UserName = registerVM.Username,
                Email = registerVM.Email,
                IsActive = true
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(registerVM);
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string link = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token },
                Request.Scheme, Request.Host.ToString());

            string body = string.Empty;
            string FilePath = "wwwroot/Template/Verify.html";

            body = _createEmailFile.CreateFile(FilePath, body);
            body = body.Replace("{{link}}", link);
            body = body.Replace("{{Fullname}}", user.Fullname);

            _emailSend.Send(user.Email, "Verify Registration", body); // to user.Email
            return RedirectToAction(nameof(VerifyEmail));
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            await _userManager.ConfirmEmailAsync(user, token);
            await _userManager.AddToRoleAsync(user, RoleEnums.Member.ToString());   // add role
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction(nameof(Login));
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginVM login, string? ReturnUrl)
        {
            if (!ModelState.IsValid) return View(login);
            AppUser user = await _userManager.FindByEmailAsync(login.UsernameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(login.UsernameOrEmail);
                if (user == null || !user.EmailConfirmed)       // elave  email confirmed serti verdim, yoxsa confirmed olmadan da daxil olurdu. Confirm your mail de ayrica modelstate yazmaq olardi eslinde
                {
                    ModelState.AddModelError("", "Username or Password invalid");
                    return View(login);
                }
            }
            if (!user.IsActive)
            {
                ModelState.AddModelError("", "Your account locked");
                return View(login);
            }
            var result = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account locked");
                return View(login);
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password invalid");
                return View(login);
            }
            // sign in
            if (await _userManager.IsInRoleAsync(user, RoleEnums.Admin.ToString()))
            {
                return RedirectToAction("index", "dashboard", new { Area = "AdminArea" });
            }
            if (ReturnUrl != null)
            {
                return Redirect(ReturnUrl);
            }

            await _signInManager.SignInAsync(user, isPersistent: true);
            return RedirectToAction("index", "home");
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login");
        }

        public async Task<IActionResult> CreateRole()
        {
            foreach (var item in Enum.GetValues(typeof(RoleEnums)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = item.ToString() });
                }
            }
            return Content("role added");
        }


        public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPassword)
        {
            if (!ModelState.IsValid) return NotFound();
            AppUser exsistUser = await _userManager.FindByEmailAsync(forgetPassword.Email);
            if (exsistUser == null)
            {
                ModelState.AddModelError("Email", "Email isn't found");
                return View();
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(exsistUser);
            string link = Url.Action(nameof(ResetPassword), "Account", new { userId = exsistUser.Id, token },
                Request.Scheme, Request.Host.ToString());

            string body = string.Empty;
            string FilePath = "wwwroot/Template/Verify.html";
            body = _createEmailFile.CreateFile(FilePath, body);
            body = body.Replace("{{link}}", link);
            body = body.Replace("{{Fullname}}", exsistUser.Fullname);

            _emailSend.Send(exsistUser.Email, "Reset Password", body);
            return RedirectToAction(nameof(VerifyEmail));
        }

        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            ResetPasswordVM resetPassword = new ResetPasswordVM()
            {
                UserId = userId,
                Token = token
            };
            return View(resetPassword);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPassword)
        {
            if (!ModelState.IsValid) return View();
            AppUser exsistUser = await _userManager.FindByIdAsync(resetPassword.UserId);
            bool chekPassword = await _userManager.VerifyUserTokenAsync(exsistUser,
                _userManager.Options.Tokens.PasswordResetTokenProvider,
                "ResetPassword", resetPassword.Token);

            if (!chekPassword) return Content("Error");
            if (exsistUser == null) return NotFound();
            if (await _userManager.CheckPasswordAsync(exsistUser, resetPassword.Password))
            {
                ModelState.AddModelError("", "This password is your last password");
                return View(resetPassword);
            }
            await _userManager.ResetPasswordAsync(exsistUser, resetPassword.Token, resetPassword.Password);
            await _userManager.UpdateSecurityStampAsync(exsistUser);
            return RedirectToAction(nameof(Login));
        }


    }
}
