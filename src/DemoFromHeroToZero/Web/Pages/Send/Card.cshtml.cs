using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Web.Models;

namespace Web.Pages.Send
{
    [Authorize]
    public class CardPageModel : PageModel
    {
        private readonly ILogger<CardPageModel> logger;

        public CardPageModel(ILogger<CardPageModel> logger) => this.logger = logger;

        public void OnGet()
        {
            logger.LogInformation("Page for sending page loaded");
        }

        [TempData]
        public string InfoText { get; set; }
        
        [BindProperty]
        public SendCardViewModel CardViewModel { get; set; } = new SendCardViewModel();
    }
}