using System;
using Model.Operations;
using Model.Repositories;
using Model.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Model.Capabilities.Validation;
using Model.Capabilities.Validators;
using Model.Exceptions;
using Polly;

namespace Model.Services
{
    public record ProductCatalogService(IProductCatalogRepository ProductCatalogRepository, ILogger<ProductCatalogService> Logger, ValidatorFactory ValidatorFactory) : IProductCatalogService
    {
        public async Task<Product> CreateAsync(Product product)
        {
            var validationResult = ValidatorFactory.GetValidator(product).Validate();
            if (!validationResult.IsValid)
                throw new InvalidProductException(validationResult.Message);

            product.Id = await Policy.Handle<Exception>()
                .WaitAndRetryAsync(2,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(5, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Logger.LogError(exception, "Adding Product attempt {Attempt} error.", new { Attempt = retryCount });
                    })
                .ExecuteAsync(async () => await ProductCatalogRepository.AddAsync(product));

            return product;
        }

        public Task<Product> Get(int id)
        {
            return ProductCatalogRepository.GetAsync(id);
        }
    }
}
