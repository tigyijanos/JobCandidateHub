using JobCandidateHub.Models;

namespace JobCandidateHub.Services
{
    /// <summary>
    /// Service interface for handling candidate operations.
    /// </summary>
    public interface ICandidateService
    {
        /// <summary>
        /// Adds or updates a candidate in the database.
        /// </summary>
        /// <param name="candidate">The candidate to be added or updated.</param>
        /// <returns>The added or updated candidate.</returns>
        Candidate UpsertCandidate(Candidate candidate);
    }
}
