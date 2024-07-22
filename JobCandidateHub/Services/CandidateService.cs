using JobCandidateHub.Data;
using JobCandidateHub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

namespace JobCandidateHub.Services
{
    public class CandidateService(CandidateDbContext context, IMemoryCache cache, IValidationService validationService) : ICandidateService
    {
        private static readonly object _lock = new();

        public Candidate UpsertCandidate(Candidate candidate)
        {
            if (!validationService.Validate(candidate, out var validationResults))
            {
                throw new ValidationException(validationResults);
            }

            lock (_lock)
            {
                var cachedCandidate = cache.Get<Candidate>(candidate.Email!);
                if (cachedCandidate != null)
                {
                    candidate.Id = cachedCandidate.Id;
                    UpdateCandidateProperties(cachedCandidate, candidate);
                    context.Update(cachedCandidate);
                }
                else
                {
                    var existingCandidate = context.Candidates.AsNoTracking().FirstOrDefault(c => c.Email == candidate.Email);
                    if (existingCandidate != null)
                    {
                        candidate.Id = existingCandidate.Id;
                        UpdateCandidateProperties(existingCandidate, candidate);
                        context.Update(existingCandidate);
                    }
                    else
                    {
                        context.Candidates.Add(candidate);
                    }
                }

                context.SaveChanges();
                cache.Set(candidate.Email!, candidate, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
            }

            return candidate;
        }

        private static void UpdateCandidateProperties(Candidate existingCandidate, Candidate newCandidate)
        {
            existingCandidate.FirstName = newCandidate.FirstName;
            existingCandidate.LastName = newCandidate.LastName;
            existingCandidate.PhoneNumber = newCandidate.PhoneNumber;
            existingCandidate.BestTimeToCall = newCandidate.BestTimeToCall;
            existingCandidate.LinkedInProfile = newCandidate.LinkedInProfile;
            existingCandidate.GitHubProfile = newCandidate.GitHubProfile;
            existingCandidate.Comments = newCandidate.Comments;
        }
    }

    public class ValidationException(List<ValidationResult> validationResults) : Exception
    {
        public List<ValidationResult> ValidationResults { get; } = validationResults;
    }
}