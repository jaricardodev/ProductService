using System.Collections.Generic;
using System.Linq;

namespace Model.Capabilities.Validation
{
    public record ValidationResult
    {
        public readonly List<ValidationError> Errors = new();

        public string Message => Errors.FirstOrDefault()?.Message;

        public bool IsValid => Errors.Count == 0;

        public void Add(ValidationError error)
        {
            Errors.Add(error);
        }
    }
}
