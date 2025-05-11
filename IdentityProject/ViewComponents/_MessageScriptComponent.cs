using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.ViewComponents
{
    public class _MessageScriptComponent: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
