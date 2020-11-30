using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Interfaces
{
    public interface ISearchService
    {
        Task<List<SearchResult>> SearchAsync(string query);
    }
}