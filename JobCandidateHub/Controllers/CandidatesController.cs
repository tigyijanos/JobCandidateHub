using JobCandidateHub.Models;
using JobCandidateHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobCandidateHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController(ICandidateService candidateService) : ControllerBase
    {
        private readonly ICandidateService _candidateService = candidateService;

        [HttpPost]
        public IActionResult UpsertCandidate([FromBody] Candidate candidate)
        {
            try
            {
                var result = _candidateService.UpsertCandidate(candidate);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                foreach (var validationResult in ex.ValidationResults)
                {
                    ModelState.AddModelError(validationResult.MemberNames.FirstOrDefault() ?? string.Empty, validationResult.ErrorMessage!);
                }
                return BadRequest(ModelState);
            }
        }
    }
}