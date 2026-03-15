using eLetter25.Application.Letters.UseCases.UploadDocument;
using eLetter25.Domain.Letters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLetter25.API.Controllers.Letters;

/// <summary>
/// Controller for uploading physical document files to an existing letter.
/// </summary>
[ApiController]
[Route("api/letters")]
[Produces("application/json")]
[Authorize]
public sealed class UploadDocumentController(IMediator mediator) : ControllerBase
{
    private static readonly Dictionary<string, DocumentFormat> SupportedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["application/pdf"] = DocumentFormat.Pdf,
            ["image/png"] = DocumentFormat.Png,
            ["image/jpeg"] = DocumentFormat.Jpeg
        };

    /// <summary>
    /// Uploads a document file and registers it against the specified letter.
    /// The document is created in <c>Registered</c> status.
    /// </summary>
    /// <param name="letterId">The unique identifier of the letter to attach the document to.</param>
    /// <param name="document">The file to upload (PDF, PNG, or JPEG, max 20 MB).</param>
    /// <param name="cancellationToken"></param>
    /// <response code="201">Document successfully registered and file stored.</response>
    /// <response code="400">Missing file or unsupported document format.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Letter not found.</response>
    [HttpPost("{letterId:guid}/documents")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UploadDocumentResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequestSizeLimit(20 * 1024 * 1024)] // 20 MB
    public async Task<IActionResult> Upload(
        [FromRoute] Guid letterId,
        IFormFile? document,
        CancellationToken cancellationToken = default)
    {
        if (document is null)
        {
            return BadRequest(new { error = "A document file is required." });
        }

        if (!SupportedContentTypes.TryGetValue(document.ContentType, out var format))
        {
            return BadRequest(new
            {
                error = $"Unsupported document type '{document.ContentType}'. Allowed: PDF, PNG, JPEG."
            });
        }

        await using var stream = document.OpenReadStream();
        var command = new UploadDocumentCommand(letterId, format, stream);

        var result = await mediator.Send(command, cancellationToken);

        return Created($"/api/letters/{result.LetterId}/documents/{result.DocumentId}", result);
    }
}

