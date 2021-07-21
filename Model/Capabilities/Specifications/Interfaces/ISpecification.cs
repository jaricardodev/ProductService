namespace Model.Capabilities.Specifications.Interfaces
{
    public interface ISpecification
    {
        bool IsSatisfiedBy();
        string ErrorMessage();
    }
}
