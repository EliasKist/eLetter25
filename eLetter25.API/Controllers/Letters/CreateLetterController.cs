using System.Text.Json;
using eLetter25.Application.Letters.Contracts;
using eLetter25.Application.Letters.UseCases.CreateLetter;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLetter25.API.Controllers.Letters;

/// <summary>Model binding target for the multipart letter-creation request.</summary>
public sealed class CreateLetterFormInput
{
    /// <summary>JSON-serialised <see cref="CreateLetterRequest"/>.</summary>
    [FromForm(Name = "metadata")]
    public string Metadata { get; set; } = string.Empty;

    [FromForm(Name = "document")]
    public IFormFile? Document { get; set; }
}

/// <summary>
/// Controller for creating letters.
/// </summary>
[ApiController]
[Route("api/letters")]
[Produces("application/json")]
[Authorize]
public sealed class CreateLetterController(IMediator mediator) : ApiControllerBase
{
    /// <summary>Creates a new letter together with its physical document.</summary>
    /// <response code="201">Letter and document successfully created.</response>
    /// <response code="400">Invalid metadata or unsupported document format.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(CreateLetterResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequestSizeLimit(20 * 1024 * 1024)] // 20 MB
    public async Task<IActionResult> Create(
        [FromForm] CreateLetterFormInput input,
        CancellationToken cancellationToken = default)
    {
        if (input.Document is null)
        {
            return BadRequest(new { error = "A document file is required." });
        }

        if (!DocumentFormatResolver.TryResolve(input.Document.ContentType, out var format))
        {
            return BadRequest(new
            {
                error = $"Unsupported document type '{input.Document.ContentType}'. Allowed: {DocumentFormatResolver.AcceptedTypes}."
            });
        }

        CreateLetterRequest? request;

        try
        {
            request = JsonSerializer.Deserialize<CreateLetterRequest>(
                input.Metadata,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            return BadRequest(new { error = "Invalid metadata JSON." });
        }

        if (request is null)
        {
            return BadRequest(new { error = "Metadata must not be null." });
        }

        if (!TryGetOwnerId(out var ownerId))
        {
            return Unauthorized();
        }

        await using var stream = input.Document.OpenReadStream();
        var command = new CreateLetterCommand(ownerId, request, stream, format, input.Document.Length);

        var result = await mediator.Send(command, cancellationToken);
        return Created($"/api/letters/{result.LetterId}", result);
    }
}