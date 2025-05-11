using IdentityProject.Context;
using IdentityProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.ViewComponents
{
    public class _MessageDetailComponent:ViewComponent
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public _MessageDetailComponent(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name); // Tüm mesaj detayları gelir
            string email = values.Email;

            ViewBag.inboxMessageCount = _context.Messages.Where(x => x.ReceiverEmail == email).Where(x => x.IsRead == false).Count();
            ViewBag.sendboxCount = _context.Messages.Where(x => x.SenderEmail == email).Count();
            ViewBag.trashboxCount = _context.Messages.Where(x => x.ReceiverEmail == email || x.SenderEmail == email).Where(z => z.IsDeleted == true).Count();


            return View();
        }
    }
}
