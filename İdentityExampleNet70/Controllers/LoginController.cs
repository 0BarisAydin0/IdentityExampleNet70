using İdentityExampleNet70.Models.DTOs;
using İdentityExampleNet70.Models.Entity;
using İdentityExampleNet70.Models.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using IEmailSender = İdentityExampleNet70.Models.Services.IEmailSender;

namespace İdentityExampleNet70.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public LoginController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
          
        }



        public IActionResult Lockout()
        {
            return View();
        }


        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        //{
        //    returnUrl ??= Url.Content("~/");

        //    if (ModelState.IsValid)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("LoginCheck", "Home");
        //        }
        //        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //        return View(model);
        //    }

        //    return View(model);
        //}


        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("LoginCheck", "Home");
                    }
                    else if (result.IsLockedOut)
                    {
                        var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                        var remainingTime = lockoutEnd - DateTime.Now;
                        ViewBag.RemainingTime = remainingTime;
                        return View("Lockout");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Random random = new Random();
                int code;
                code = random.Next(100000, 1000000);
                var user = new AppUser { UserName = model.Email, Email = model.Email, ConfirmCode = code };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false); // isPersistent: false -> tarayıcı kapandığında kullanıcının oturumu                                                                  sonlanır
                    return RedirectToAction("LoginCheck", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }




        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            // Diğer kimlik doğrulama şemalarından (external logins, cookie vb.) da çıkış yapabilirsiniz
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return RedirectToAction("Login", "Login");
        }






        public IActionResult AddRoleToUser()
        {
            return View();
        }




        /*

        [HttpPost]
        public async Task<IActionResult> AddRoleToUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Kullanıcı bulunamadı, uygun bir hata mesajı gösterin
                return NotFound();
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                // Rol atama başarılı, gerekirse bir geri dönüş mesajı gösterin
                return Ok("Role added successfully");
            }
            else
            {
                // Rol atama başarısız, hata mesajlarını işleyin ve uygun bir hata mesajı gösterin
                return BadRequest(result.Errors);
            }
        }

        */



        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Kullanıcı yok veya e-posta onaylanmamışsa hata mesajı göster
                    return View("ForgotPasswordConfirmation");
                }

                // Şifre sıfırlama bağlantısı oluştur ve kullanıcıya e-posta ile gönder
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Şifre Sıfırlama", $"Lütfen şifrenizi sıfırlamak için <a href='{callbackUrl}'>buraya tıklayınız</a>.");
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }


    }

}
