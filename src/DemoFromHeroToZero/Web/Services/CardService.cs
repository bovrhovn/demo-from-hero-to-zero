using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using Web.Interfaces;
using Web.Models;
using Web.Options;

namespace Web.Services
{
    public class CardService : ICardService
    {
        private readonly ILogger<CardService> logger;
        private readonly IStorageWorker storageWorker;
        private readonly StorageOptions options;

        public CardService(ILogger<CardService> logger, IStorageWorker storageWorker,
            IOptions<StorageOptions> optionsValue)
        {
            this.logger = logger;
            options = optionsValue.Value;
            this.storageWorker = storageWorker;
        }

        public async Task<bool> SendCardAsync(string nameOfCard, SendCardModel objectVersionOfCard)
        {
            string generatedJsonObject = JsonConvert.SerializeObject(objectVersionOfCard);
            var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(generatedJsonObject));

            try
            {
                await storageWorker.UploadFileAsync(nameOfCard, ms, options.CardsContainer);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return false;
            }

            return true;
        }

        public async Task<List<CardReportInfo>> GetCardsAsync()
        {
            var list = await storageWorker.GetListInContainerAsync(options.CardsContainer);
            var cards = new List<CardReportInfo>();

            foreach (var currentCard in list)
            {
                string url = await storageWorker.GetFileUrl(currentCard, options.CardsContainer, false);
                cards.Add(new CardReportInfo
                {
                    Name = currentCard,
                    URL = url
                });
            }

            return cards;
        }
    }
}