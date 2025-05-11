using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.ViewComponents
{
    public class _MessageFooterComponent: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
