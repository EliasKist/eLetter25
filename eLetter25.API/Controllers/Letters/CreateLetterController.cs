using eLetter25.Application.Letters.Contracts;
using eLetter25.Application.Letters.UseCases.CreateLetter;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLetter25.API.Controllers.Letters;

/// <summary>
/// Controller for creating letters.
/// </summary>
[ApiController]
[Route("api/letters")]
[Produces("application/json")]
[Authorize]
public sealed class CreateLetterController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Creates a new letter.
    /// </summary>
    /// <param name="request">The letter creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created letter.</returns>
    /// <response code="201">Letter successfully created.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateLetterResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody] CreateLetterRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new CreateLetterCommand(request), cancellationToken);
        return Created($"/api/letters/{result.LetterId}", result);
    }
}