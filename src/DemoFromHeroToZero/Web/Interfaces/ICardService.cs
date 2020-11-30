using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Web.Models;

namespace Web.Interfaces
{
    public interface ICardService
    {
        Task<bool> SendCardAsync(string nameOfCard, SendCardModel objectVersionOfCard);
        Task<List<CardReportInfo>> GetCardsAsync();
    }
}