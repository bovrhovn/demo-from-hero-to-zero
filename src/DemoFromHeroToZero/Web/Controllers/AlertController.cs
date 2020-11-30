using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Web.Hubs;
using Web.Models;
using SubscriptionValidationEventData = Microsoft.Azure.EventGrid.Models.SubscriptionValidationEventData;

namespace Web.Controllers
{
    [ApiController]
    [Route("notification")]
    public class AlertController : ControllerBase
    {
        private readonly IHubContext<AlertHub> hubContext;

        public AlertController(IHubContext<AlertHub> hubContext) => this.hubContext = hubContext;

        [Route("check")]
        public IActionResult Health() => Ok($"I am alive {DateTime.Now}");

        [HttpPost]
        [Route("classic")]
        public async Task<IActionResult> Classic()
        {
            var bodyStream = new StreamReader(HttpContext.Request.Body);
            var receivedEvent = await bodyStream.ReadToEndAsync();
            return Ok(receivedEvent);
        }

        [HttpPost]
        [Route("alert")]
        public async Task<IActionResult> Add()
        {
            var bodyStream = new StreamReader(HttpContext.Request.Body);
            var receivedEvent = await bodyStream.ReadToEndAsync();

            if (HttpContext.Request.Headers.TryGetValue("Aeg-Event-Type", out var headerValues))
            {
                var validationHeaderValue = headerValues.FirstOrDefault();
                if (validationHeaderValue != null && validationHeaderValue.Contains("SubscriptionValidation"))
                {
                    var events = JsonConvert.DeserializeObject<EventGridEvent[]>(receivedEvent);
                    var code = events[0].Data as JObject;
                    var eventData = code?.ToObject<SubscriptionValidationEventData>();
                    var responseData =
                        new SubscriptionValidationResponseData {ValidationResponse = eventData?.ValidationCode};
                    return Ok(JsonConvert.SerializeObject(responseData));
                }
            }

            var eventGridEvents = JsonConvert.DeserializeObject<EventGridEvent[]>(receivedEvent);

            foreach (var eventGridEvent in eventGridEvents)
            {
                var sasUrl = eventGridEvent.Data as JObject;
                var imageUrl = sasUrl?["imgurl"]?.ToString();
                await hubContext.Clients.All.SendAsync("alertMessage", imageUrl);
            }

            return Ok($"Data was received at {DateTime.Now} and all clients has been notified.");
        }
    }
}