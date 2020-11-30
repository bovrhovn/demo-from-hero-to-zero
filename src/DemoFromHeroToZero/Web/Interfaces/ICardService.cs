using System.Threading.Tasks;
using Models;

namespace Web.Interfaces
{
    public interface ICardService
    {
        Task<bool> SendCardAsync(string nameOfCard, SendCardModel objectVersionOfCard);
    }
}