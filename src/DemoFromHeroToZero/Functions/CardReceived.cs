using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Models;
using Newtonsoft.Json;

namespace Functions
{
    public class CardReceived
    {
        private readonly IStorageWorker storageWorker;
        private static readonly HttpClient httpClient = new HttpClient();
        public CardReceived(IStorageWorker storageWorker) => this.storageWorker = storageWorker;

        [FunctionName("CardReceived")]
        public async Task RunAsync([BlobTrigger("cards/{name}", Connection = "AzureWebJobsStorage")]
            Stream cardBlob, string name, ILogger log, [Queue("emails")] CloudQueue emails)
        {
            log.LogInformation($"Card in process mode\n Name:{name} \n Size: {cardBlob.Length} Bytes");
            cardBlob.Position = 0;
            using var reader = new StreamReader(cardBlob, Encoding.UTF8);

            var card = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(card))
            {
                log.LogError("Card was empty. Check, if data is ok.");
                return;
            }

            var model = JsonConvert.DeserializeObject<SendCardModel>(card);
            log.LogInformation($"Prepare card and save it to another blob storage for download");
            var html = await storageWorker.DownloadAsStringAsync("card.html");
            //replace items ##
            var replaced = html.Replace("##FULLNAME", model.FullName);
            replaced = replaced.Replace("##SUBJECT", model.Subject);

            var imageUrl = await storageWorker.GetFileUrl(model.ImageName, false);

            replaced = replaced.Replace("##IMAGE", imageUrl);
            replaced = replaced.Replace("##DEDICATION", model.DedicationText);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(replaced));
            await storageWorker.UploadFileAsync(name, stream, "sent");

            var cardPublicUrl = await storageWorker.GetFileUrl(name, "cards", false);

            var emailMessage = new EmailModel
            {
                From = "santa.claus@northpole.org",
                To = "bojan@vrhovnik.net",
                Subject = "Christmas card sent",
                Content = "Christmas card is available on this link (copy and open in browser): " + cardPublicUrl
            };
            var emailToBeSent = JsonConvert.SerializeObject(emailMessage);
            await emails.AddMessageAsync(new CloudQueueMessage(emailToBeSent));
            log.LogInformation("Email sent, processing event grid");
            
            //SENT info to all subscribers to EventGrid
            string publisherMessage = $"We generated card and send email with this link {cardPublicUrl}";
            await SentInfoToEventGrid(publisherMessage, log);
            log.LogInformation("Finished sending");
        }

        private async Task SentInfoToEventGrid(string message, ILogger log)
        {
            string topicEndpoint = Environment.GetEnvironmentVariable("EventGridTopicEndpoint",
                EnvironmentVariableTarget.Process);
            string key = Environment.GetEnvironmentVariable("EventGridTopicKey",
                EnvironmentVariableTarget.Process);

            var list = new List<Event>();
            var currentData = new Event {EventTypes = EventTypes.GENERAL_INFORMATION, Subject = message};

            list.Add(currentData);
            var report = JsonConvert.SerializeObject(list);

            log.LogInformation($"Sending message to {topicEndpoint}");

            var content = new StringContent(report);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(topicEndpoint ?? "", UriKind.Absolute),
                Method = HttpMethod.Post,
                Content = content
            };

            request.Headers.Add("aeg-sas-key", key);

            var response = await httpClient.SendAsync(request);
            log.LogInformation($"Message has been sent. Awaiting result - status code: {response.IsSuccessStatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                log.LogInformation(error);
            }
        }
    }
}