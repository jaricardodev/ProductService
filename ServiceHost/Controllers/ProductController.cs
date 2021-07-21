using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Model.Operations;
using Model.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ServiceHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private IProductCatalogService ProductCatalogService { get; }

        public ProductController(IProductCatalogService productCatalogService)
        {
            ProductCatalogService = productCatalogService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var product = await ProductCatalogService.Get(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(Product product)
        {
            var createdProduct = await ProductCatalogService.CreateAsync(product);
            return CreatedAtAction("Get", new { id = createdProduct.Id }, createdProduct);
        }
    }
}
