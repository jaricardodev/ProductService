using System.Collections.Generic;
using Model.Capabilities.Specifications.Interfaces;

namespace Model.Capabilities.Validation
{
    public abstract record Validator
    {
        private readonly HashSet<Rule> _rules = new();

        public ValidationResult ValidateStopWhenError() => Validate(true);

        public ValidationResult Validate(bool stopWhenError = false)
        {
            var validationResult = new ValidationResult();
            foreach (var rule in _rules)
            {
                var specification = rule.Specification;

                if (!specification.IsSatisfiedBy())
                    validationResult.Add(new ValidationError(rule.ErrorMessage ?? specification.ErrorMessage()));

                if (!validationResult.IsValid && stopWhenError) break;
            }
            return validationResult;
        }

        protected void Add(ISpecification specification, string overrideError = null)
        {
            _rules.Add(new Rule(specification, overrideError));
        }
    }
}
