using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Web.Options;

namespace Web.Pages.Info
{
    public class SearchPageModel : PageModel
    {
        private readonly ILogger<SearchPageModel> logger;
        private readonly ISearchService searchService;
        private readonly IStorageWorker storageWorker;
        private readonly StorageOptions options;

        public SearchPageModel(ILogger<SearchPageModel> logger, 
            ISearchService searchService,
            IStorageWorker storageWorker,
            IOptions<StorageOptions> optionsValue)
        {
            this.logger = logger;
            options = optionsValue.Value;
            this.searchService = searchService;
            this.storageWorker = storageWorker;
        }

        public async Task OnGetAsync()
        {
            logger.LogInformation("Loading page for searching");
            if (!string.IsNullOrEmpty(Query))
            {
                logger.LogInformation($"starting to search based on {Query}");
                var results = await searchService.SearchAsync(Query);
                foreach (var searchResult in results)
                {
                    searchResult.MoreInfo = "https://cards.vrhovnik.net"; //TODO: change to redirector
                    Results.Add(searchResult);
                }

                logger.LogInformation($"Received back {results.Count} results");
            }
        }

        [BindProperty(SupportsGet = true)] public string Query { get; set; }
        [BindProperty] public List<SearchResult> Results { get; set; } = new List<SearchResult>();
    }
}