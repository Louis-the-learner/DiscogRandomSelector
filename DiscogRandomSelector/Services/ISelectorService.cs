using System.Threading.Tasks;

using discogSelector.Models;

namespace discogSelector.Services
{
    
    public interface ISelectorService
    {
        Task<int> GetTotalItems();

        Task<Release> GetItem(int itemPosition);
    }
}