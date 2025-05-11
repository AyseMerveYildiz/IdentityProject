using IdentityProject.Entities;
using IdentityProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterController(UserManager<AppUser> userManager) //Dependency Injection
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            AppUser appUser = new AppUser()
            {
                Name = model.Name,
                UserName = model.Username,
                Surname = model.Surname,
                Email = model.Email,
            };
            var result = await _userManager.CreateAsync(appUser, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("UserLogin", "Login");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(model);
            }
        }
    }
}
