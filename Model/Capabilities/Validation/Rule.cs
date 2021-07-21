using Model.Capabilities.Specifications.Interfaces;

namespace Model.Capabilities.Validation
{
    public record Rule(ISpecification Specification, string ErrorMessage)
    {
    }
}
