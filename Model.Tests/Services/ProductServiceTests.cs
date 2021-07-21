using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Operations;
using Model.Repositories;
using Model.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Model.Capabilities.Validation;
using Model.Exceptions;

namespace Model.Tests.Services
{
    [TestClass]
    public class ProductServiceTests
    {
        private ProductCatalogService _catalogService;
        private Mock<IProductCatalogRepository> _postRepositoryMock;
        private Mock<ILogger<ProductCatalogService>> _productLoggerMock;

        [TestInitialize]
        public void Setup()
        {
            _postRepositoryMock = new Mock<IProductCatalogRepository>();
            _productLoggerMock = new Mock<ILogger<ProductCatalogService>>();
            _postRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Product>())).ReturnsAsync(1);
            _catalogService = new ProductCatalogService(_postRepositoryMock.Object, _productLoggerMock.Object, new ValidatorFactory());
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
        public async Task Publish_WhenValid_ReturnsAdded()
        {
            var expectedPost = GetTestProduct();

            var actualPost = await _catalogService.CreateAsync(expectedPost);

            Assert.IsNotNull(actualPost);
            Assert.IsTrue(actualPost.Id > 0);
            Assert.AreEqual(expectedPost.Name, actualPost.Name);
            Assert.AreEqual(expectedPost.Brand, actualPost.Brand);
            Assert.AreEqual(expectedPost.Expiration, actualPost.Expiration);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidProductException))]
        public async Task Publish_WhenInvalid_ThrowsException()
        {
            var expectedProduct = GetTestProduct();
            expectedProduct.Name = string.Empty;

            await _catalogService.CreateAsync(expectedProduct);
        }
    }
}
