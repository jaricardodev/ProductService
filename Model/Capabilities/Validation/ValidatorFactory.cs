using Model.Capabilities.Validators;
using Model.Operations;
using System;

namespace Model.Capabilities.Validation
{
    public record ValidatorFactory
    {
        public Validator GetValidator(OperationEntity entity)
        {
            return entity.GetType() switch
            {
                var productType when productType == typeof(Product) => new ProductValidator((Product) entity),
                _ => throw new ArgumentOutOfRangeException(entity.GetType().Name)
            };
        }
    }
}
