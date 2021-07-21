using Model.Operations;
using System.Threading.Tasks;

namespace Model.Services.Interfaces
{
    public interface IProductCatalogService
    {
        Task<Product> CreateAsync(Product product);
        Task<Product> Get(int id);
    }
}
