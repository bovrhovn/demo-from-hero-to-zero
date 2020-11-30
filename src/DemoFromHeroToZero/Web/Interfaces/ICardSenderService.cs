using System.Threading.Tasks;
using Web.Models;

namespace Web.Interfaces
{
    public interface ICardSenderService
    {
        Task<bool> SendCardAsync(string nameOfCard, SendCardModel objectVersionOfCard);
    }
}