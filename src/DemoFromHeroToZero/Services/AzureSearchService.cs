using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Interfaces;
using Models;
using Newtonsoft.Json;

namespace Services
{
    public class AzureSearchService : ISearchService
    {
        private readonly SearchClient searchClient;

        public AzureSearchService(string searchName, string searchKey, string indexName)
        {
            var serviceEndpoint = new Uri($"https://{searchName}.search.windows.net/");
            var credential = new AzureKeyCredential(searchKey);
            searchClient = new SearchClient(serviceEndpoint, indexName, credential);
        }

        public async Task<List<SearchResult>> SearchAsync(string query)
        {
            var options = new SearchOptions
            {
                Filter = "",
                IncludeTotalCount = true
            };

            var list = new List<SearchResult>();

            var response = await searchClient.SearchAsync<SearchModel>(query + "*", options);

            foreach (var currentModel in response.Value.GetResults())
            {
                if (currentModel.Document.Content != null)
                {
                    var model = currentModel.Document.Content;
                    list.Add(new SearchResult
                    {
                        Name = $"{model.Subject} with subject {model.FullName}",
                        Description = model.DedicationText,
                        MoreInfo = model.ImageName
                    });
                }
            }

            return list;
        }
    }

    public class SearchModel
    {
        [JsonPropertyName("content")] public Content Content { get; set; }

        [JsonPropertyName("keyPhrases")] public string[] KeyPhrases { get; set; }
    }

    public class Content
    {
        [JsonProperty("FullName")] public string FullName { get; set; }

        [JsonProperty("Subject")] public string Subject { get; set; }

        [JsonProperty("DedicationText")] public string DedicationText { get; set; }

        [JsonProperty("ImageName")] public string ImageName { get; set; }
    }
}