using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Operations;
using Model.Services.Interfaces;
using Moq;
using Persistence.Repositories;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Persistence.Tests.Repositories
{
    [TestClass]
    public class PostRepositoryTests : TestBase
    {
        private DBProductCatalogRepository _catalogRepository;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            _catalogRepository = new DBProductCatalogRepository(Context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestCleanup();
        }

 

        private Product GetTestProduct()
        {
            return new()
            {
                Name = "Test Product Name",
                Brand = "Test Product Brand",
                Expiration = DateTime.UtcNow.AddDays(10),
                ImageUrl = "url",
                IsInStock = true
            };
        }



        [TestMethod]
        public async Task AddAsync_WhenExpirationDateIsNull_PostCreatedWithNullDate()
        {
            var expectedPost = GetTestProduct();
            expectedPost.Expiration = null;

            await _catalogRepository.AddAsync(expectedPost);
            var actualPost = await Context.Products.SingleOrDefaultAsync(u => u.Name == expectedPost.Name);

            Assert.IsNull(actualPost.Expiration);
        }
    }
}
