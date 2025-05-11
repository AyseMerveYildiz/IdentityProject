using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.ViewComponents
{
    public class _MessageHeadComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
