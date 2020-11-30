using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Newtonsoft.Json;
using Web.Helpers;
using Web.Interfaces;
using Web.Models;

namespace Web.Pages.Send
{
    [Authorize]
    public class CardPageModel : PageModel
    {
        private readonly ILogger<CardPageModel> logger;
        private readonly ICardSenderService cardSenderService;

        public CardPageModel(ILogger<CardPageModel> logger, ICardSenderService cardSenderService)
        {
            this.logger = logger;
            this.cardSenderService = cardSenderService;
        }

        public void OnGet()
        {
            logger.LogInformation("Page for sending page loaded");
        }

        public async Task<RedirectToPageResult> OnPostAsync()
        {
            logger.LogInformation("Call service for sending card");
            
            var name = StringHelpers.RandomString(8);
            await cardSenderService.SendCardAsync(name, CardModel);
            
            logger.LogInformation("Card sent");
            InfoText = $"Card with name {name} has been sent at {DateTime.Now}";
            return RedirectToPage("/Send/Card");
        }

        [TempData]
        public string InfoText { get; set; }
        
        [BindProperty]
        public SendCardModel CardModel { get; set; } = new SendCardModel();
    }
}