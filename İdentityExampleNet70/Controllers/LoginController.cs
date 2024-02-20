using İdentityExampleNet70.Models.DTOs;
using İdentityExampleNet70.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace İdentityExampleNet70.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public LoginController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        
        public IActionResult Lockout()
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
                var user = new AppUser { UserName = model.Email, Email = model.Email };
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


        public IActionResult AddRoleToUser()
        {
            return View();
        }






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


    }

}
