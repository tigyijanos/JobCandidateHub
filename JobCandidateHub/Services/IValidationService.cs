using System.ComponentModel.DataAnnotations;

namespace JobCandidateHub.Services
{
    /// <summary>
    /// Service interface for validating models.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Validates the specified model.
        /// </summary>
        /// <typeparam name="T">The type of the model to validate.</typeparam>
        /// <param name="model">The model to validate.</param>
        /// <param name="validationResults">The list of validation results.</param>
        /// <returns>True if the model is valid, otherwise false.</returns>
        bool Validate<T>(T model, out List<ValidationResult> validationResults);
    }
}
