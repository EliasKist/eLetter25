using eLetter25.Application.Letters.UseCases.GetLetters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLetter25.API.Controllers.Letters;

[ApiController]
[Route("api/letters")]
[Produces("application/json")]
[Authorize]
public sealed class GetLettersController(IMediator mediator) : ApiControllerBase
{
    /// <summary>Returns all letters belonging to the authenticated user, ordered by creation date descending.</summary>
    /// <response code="200">List of letter summaries.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GetLettersResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        if (!TryGetOwnerId(out var ownerId))
        {
            return Unauthorized();
        }

        var result = await mediator.Send(new GetLettersQuery(ownerId), cancellationToken);
        return Ok(result);
    }
}