using System;
using Mapster;
using Product = Persistence.Context.Product;

namespace Persistence.Mappers
{
    public static class ProductMapper
    {
        public static void Configure()
        {
            TypeAdapterConfig<Model.Operations.Product, Product>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Brand, src => src.Brand)
                .Map(dest => dest.Expiration, src => src.Expiration)
                .Map(dest => dest.IsInStock, src => src.IsInStock)
                .Map(dest => dest.Created, src => DateTime.UtcNow)
                .Map(dest => dest.Modified, src => DateTime.UtcNow)
                .Ignore(dest => dest.Status)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.ModifiedBy);

            TypeAdapterConfig<Product, Model.Operations.Product>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Brand, src => src.Brand)
                .Map(dest => dest.Expiration, src => src.Expiration)
                .Map(dest => dest.IsInStock, src => src.IsInStock)
                .Ignore(dest => dest.ImageUrl);

        }
    }
}
