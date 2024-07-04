using JobCandidateHub.Validators;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace JobCandidateHub.Models
{
    public class Candidate
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [SwaggerSchema(Description = "The candidate's first name.")]
        public string? FirstName { get; set; }
        
        [Required(ErrorMessage = "Last name is required")]
        [SwaggerSchema(Description = "The candidate's last name.")]
        public string? LastName { get; set; }

        [SwaggerSchema(Description = "The candidate's phone number.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [SwaggerSchema(Description = "The candidate's email address.")]
        public string? Email { get; set; }

        [TimeInterval(ErrorMessage = "Invalid time intervall format")]
        [SwaggerSchema(Description = "The best time to call the candidate, formatted as 'HH:mm-HH:mm'.")]
        public string? BestTimeToCall { get; set; }

        [Url(ErrorMessage = "Invalid LinkedIn profile URL")]
        [SwaggerSchema(Description = "The candidate's LinkedIn profile URL.")]
        public string? LinkedInProfile { get; set; }

        [Url(ErrorMessage = "Invalid GitHub profile URL")]
        [SwaggerSchema(Description = "The candidate's GitHub profile URL.")]
        public string? GitHubProfile { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        [SwaggerSchema(Description = "Additional comments about the candidate.")]
        public string? Comments { get; set; }
    }

}
