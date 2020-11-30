using System;
using System.IO;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Models;
using Web.Helpers;
using Web.Interfaces;
using Web.Models;

namespace Web.Pages.Send
{
    [Authorize]
    public class CardPageModel : PageModel
    {
        private readonly ILogger<CardPageModel> logger;
        private readonly ICardService cardService;
        private readonly IStorageWorker storageWorker;

        public CardPageModel(ILogger<CardPageModel> logger,
            ICardService cardService,
            IStorageWorker storageWorker)
        {
            this.logger = logger;
            this.cardService = cardService;
            this.storageWorker = storageWorker;
        }

        public void OnGet()
        {
            logger.LogInformation("Page for sending page loaded");
        }

        public async Task<RedirectToPageResult> OnPostAsync()
        {
            logger.LogInformation("Call service for sending card");

            logger.LogInformation($"Uploading image");
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var cardImageName = $"{StringHelpers.RandomString(5)}-{file.FileName}";
                var filePath = Path.GetTempFileName();

                await using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                    await storageWorker.UploadFileAsync(cardImageName, stream);
                }

                CardModel.ImageName = cardImageName;
            }

            var name = StringHelpers.RandomString(8);
            await cardService.SendCardAsync(name, CardModel);

            logger.LogInformation("Card sent");
            InfoText = $"Card with name {name} has been sent at {DateTime.Now}";
            return RedirectToPage("/Send/Card");
        }

        [TempData] public string InfoText { get; set; }

        [BindProperty] public SendCardModel CardModel { get; set; } = new SendCardModel();
    }
}