using Mapster;
using Model.Operations;
using Model.Repositories;
using Persistence.Context;
using System.Threading.Tasks;
using Product = Persistence.Context.Product;

namespace Persistence.Repositories
{
    public class DBProductCatalogRepository : BaseRepository, IProductCatalogRepository
    {

        public DBProductCatalogRepository(FeedContext context) : base(context)
        {
            
        }

        public async Task<int> AddAsync(Model.Operations.Product product)
        {
            var dbProduct = product.Adapt<Product>();
            await base.AddAsync(dbProduct);
            return dbProduct.Id;
        }

        public async Task<Model.Operations.Product> GetAsync(int id)
        {
            var dbProduct = await base.FindOneAsync<Product>(x => x.Id == id);

            return dbProduct.Adapt<Model.Operations.Product>();
        }
    }
}
