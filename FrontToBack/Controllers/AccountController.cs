using FrontToBack.Helpers;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace FrontToBack.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }


        // confirmation olmadan da login alinir. cixis yolu kimi isActive istifade etmek olar en son variantda
        [HttpPost]
        [AutoValidateAntiforgeryToken]
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


            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("ismayilbz@code.edu.az"));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Verify Email";

            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/Template/Verify.html"))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{{link}}", link);
            body = body.Replace("{{Fullname}}", user.Fullname);

            email.Body = new TextPart(TextFormat.Html) { Text = body };

            // send email
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);  //465
            smtp.Authenticate("ismayilbz@code.edu.az", "apaswqtxbhcriskl");
            smtp.Send(email);
            smtp.Disconnect(true);


            return RedirectToAction(nameof(VerifyEmail));

            // add role
            // await _userManager.AddToRoleAsync(user, RoleEnums.Admin.ToString());
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

            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("ismayilbz@code.edu.az"));
            email.To.Add(MailboxAddress.Parse(exsistUser.Email));
            email.Subject = "Verify reset password Email";
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/Template/Verify.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{{link}}", link);
            body = body.Replace("{{Fullname}}", exsistUser.Fullname);
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            // send email
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);  //465
            smtp.Authenticate("ismayilbz@code.edu.az", "apaswqtxbhcriskl");
            smtp.Send(email);
            smtp.Disconnect(true);

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
