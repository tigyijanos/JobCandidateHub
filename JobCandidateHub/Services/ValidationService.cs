using System.ComponentModel.DataAnnotations;

namespace JobCandidateHub.Services
{
    public class ValidationService : IValidationService
    {
        public bool Validate<T>(T model, out List<ValidationResult> validationResults)
        {
            var validationContext = new ValidationContext(model!, null, null);
            validationResults = [];
            return Validator.TryValidateObject(model!, validationContext, validationResults, true);
        }
    }
}
