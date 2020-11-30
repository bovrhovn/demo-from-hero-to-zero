using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Web.Interfaces;
using Web.Models;

namespace Web.Pages.Info
{
    public class ReportPageModel : PageModel
    {
        private readonly ILogger<ReportPageModel> logger;
        private readonly ICardService cardService;

        public ReportPageModel(ILogger<ReportPageModel> logger, ICardService cardService)
        {
            this.logger = logger;
            this.cardService = cardService;
        }

        public async Task OnGetAsync()
        {
            logger.LogInformation("Getting cards");
            var cards =await cardService.GetCardsAsync();
            logger.LogInformation($"Got {cards.Count} cards");
            Reports = cards;
        }
        
        [BindProperty] public List<CardReportInfo> Reports { get; set; }
    }
}