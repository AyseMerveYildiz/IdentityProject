using IdentityProject.Context;
using IdentityProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IdentityProject.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;
        public MessageController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> Inbox(string search)
        {
            // Kullanıcı bilgilerini al
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.email = values.Email;
            ViewBag.v1 = values.Name + " " + values.Surname;

            var messages = _context.Messages.Where(x => x.ReceiverEmail == values.Email);

     
            if (!string.IsNullOrEmpty(search))
            {
                messages = messages.Where(m => m.Subject.Contains(search) || m.SenderEmail.Contains(search));
            }

        
            var messageList = await messages.OrderByDescending(x => x.SendDate).ToListAsync();

           
            return View(messageList);
        }
        public async Task<IActionResult> SendBox(string search)
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            string emailValue = values.Email;

      
            var sendMessageList = _context.Messages
                                          .Where(x => x.SenderEmail == emailValue && !x.IsDeleted);

           
            if (!string.IsNullOrEmpty(search))
            {
                sendMessageList = sendMessageList.Where(m => m.Subject.Contains(search) || m.ReceiverEmail.Contains(search));
                ViewBag.v1 = $"Arama Sonuclari: '{search}'";
            }
            else
            {
                ViewBag.v1 = "Tüm Giden Mesajlar";
            }

            // Mesajları tarih sırasına göre getir
            var model = await sendMessageList.OrderByDescending(x => x.SendDate).ToListAsync();
            return View(model);
        }



        [HttpGet]
        public IActionResult CreateMessage()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage(Message message)
        {

            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            string senderEmail = values.Email;


            message.SenderEmail = senderEmail;
            message.IsRead = false;
            message.SendDate = DateTime.Now;


            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Mesajın başarıyla gönderildiğini belirtmek için TempData kullan
            TempData["MessageSent"] = "true";

            // SendBox sayfasına yönlendir
            return RedirectToAction("SendBox");
        }


        public async Task<IActionResult> MessageDetail(int id)
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.v1 = values.Name + " " + values.Surname;
            var message = _context.Messages.Find(id);
            if (message != null)
            {
                message.IsRead = true;
                _context.SaveChanges();
            }
            return View(message);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeMessageStatus(List<int> messageId)
        {
            if (messageId != null && messageId.Any())
            {
                // İlgili mesajları veritabanından çek
                var messages = await _context.Messages
                    .Where(m => messageId.Contains(m.MessageId))
                    .ToListAsync();

             
                foreach (var msg in messages)
                {
                    msg.IsRead = true;
                }

               
                await _context.SaveChangesAsync();

                // Kullanıcıya bilgi için TempData
                TempData["MessageStatusChanged"] = "Secilen mesajlar okundu olarak isaretlendi.";
            }
            else
            {
                TempData["MessageStatusChanged"] = "Lütfen en az bir mesaj seçin.";
            }

            return RedirectToAction("Inbox");
        }

        [HttpPost]
        public async Task<IActionResult> MoveToTrash(List<int> messageId)
        {
            var messages = _context.Messages.Where(x => messageId.Contains(x.MessageId)).ToList();

            foreach (var item in messages)
            {
                item.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            TempData["MessageStatusChanged"] = "Mesaj(lar) trashboxa gonderildi!";

            string referer = Request.Headers["Referer"].ToString();
            return Redirect(referer); // Geldiği sayfaya dön
        }

        [Authorize]
        public async Task<IActionResult> TrashBox()
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            string email = values.Email;

            var deletedMessages = await _context.Messages
                .Where(x => (x.ReceiverEmail == email || x.SenderEmail == email) && x.IsDeleted)
                .OrderByDescending(x => x.SendDate)
                .ToListAsync();

            return View(deletedMessages);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePermanently(List<int> messageIds)
        {
            var messages = _context.Messages.Where(m => messageIds.Contains(m.MessageId)).ToList();

            foreach (var msg in messages)
            {
                _context.Messages.Remove(msg);
            }

            await _context.SaveChangesAsync();
            TempData["MessageStatusChanged"] = "Mesajlar kalici olarak silindi!";
            return RedirectToAction("TrashBox");
        }

        

       
    }
}
