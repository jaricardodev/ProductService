using Model.Capabilities.Specifications;
using Model.Capabilities.Validation;
using Model.Operations;

namespace Model.Capabilities.Validators
{
    public record ProductValidator : Validator
    {
        public ProductValidator(Product product)
        {
            Add(new ProductNameMustBeSpecified(product));
        }
    }
}
