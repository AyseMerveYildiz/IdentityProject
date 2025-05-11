using IdentityProject.Context;
using IdentityProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.Controllers
{
    public class ProfileController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> ProfileDetail()
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            string email = values.Email;

            ViewBag.profilePicture = values.ProfileImageUrl;
            ViewBag.name = values.Name;
            ViewBag.surname = values.Surname;
            ViewBag.email = values.Email;
            ViewBag.phone = values.PhoneNumber;
            ViewBag.username = values.UserName;

            ViewBag.inboxMessageCount = _context.Messages
                                                 .Where(x => x.ReceiverEmail == email && !x.IsRead)
                                                 .Count();

            ViewBag.sendboxCount = _context.Messages
                                           .Where(x => x.SenderEmail == email)
                                           .Count();

            ViewBag.trashboxCount = _context.Messages
                                            .Where(x => (x.ReceiverEmail == email || x.SenderEmail == email) && x.IsDeleted)
                                            .Count();

            return View();
        }

    }

}
