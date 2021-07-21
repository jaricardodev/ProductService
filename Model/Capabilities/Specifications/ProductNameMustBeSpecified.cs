using Model.Capabilities.Specifications.Interfaces;
using Model.Operations;

namespace Model.Capabilities.Specifications
{
    public record ProductNameMustBeSpecified(Product Product) : ISpecification
    {
        public bool IsSatisfiedBy() => !string.IsNullOrWhiteSpace(Product.Name);

        public string ErrorMessage() => "The product name is required";
    }
}
