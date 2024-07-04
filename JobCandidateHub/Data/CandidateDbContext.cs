using JobCandidateHub.Models;
using Microsoft.EntityFrameworkCore;

namespace JobCandidateHub.Data
{
    public class CandidateDbContext(DbContextOptions<CandidateDbContext> options) : DbContext(options)
    {
        public DbSet<Candidate> Candidates { get; set; }
    }
}
