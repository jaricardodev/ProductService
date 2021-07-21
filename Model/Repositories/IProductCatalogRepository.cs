using Model.Operations;
using System.Threading.Tasks;

namespace Model.Repositories
{
    public interface IProductCatalogRepository 
    {
        Task<int> AddAsync(Product product);
        Task<Product> GetAsync(int id);
    }
}
