using System.Threading.Tasks;

using discogRandomSelector.Models;

namespace discogRandomSelector.Services
{
    
    public interface ISelectorService
    {
        Task<int> GetTotalItems();

        Task<Release> GetItem(int itemPosition);
    }
}