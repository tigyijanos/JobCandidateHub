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
        [MaxLength(255)]
        public string? FirstName { get; set; }
        
        [Required(ErrorMessage = "Last name is required")]
        [SwaggerSchema(Description = "The candidate's last name.")]
        [MaxLength(255)]
        public string? LastName { get; set; }

        [SwaggerSchema(Description = "The candidate's phone number.")]
        [MaxLength(255)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [SwaggerSchema(Description = "The candidate's email address.")]
        [MaxLength(255)]
        public string? Email { get; set; }

        [TimeInterval(ErrorMessage = "Invalid time interval format")]
        [SwaggerSchema(Description = "The best time to call the candidate, formatted as 'HH:mm-HH:mm'.")]
        [MaxLength(11)]
        public string? BestTimeToCall { get; set; }

        [Url(ErrorMessage = "Invalid LinkedIn profile URL")]
        [SwaggerSchema(Description = "The candidate's LinkedIn profile URL.")]
        [MaxLength(500)]
        public string? LinkedInProfile { get; set; }

        [Url(ErrorMessage = "Invalid GitHub profile URL")]
        [SwaggerSchema(Description = "The candidate's GitHub profile URL.")]
        [MaxLength(1000)]
        public string? GitHubProfile { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        [SwaggerSchema(Description = "Additional comments about the candidate.")]
        [MaxLength(2000)]
        public string? Comments { get; set; }
    }

}
