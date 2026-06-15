using Dukaan.Media.Application.Dtos;
using Dukaan.Media.Application.Features.GetMedia;
using Dukaan.Media.Application.Features.UploadMedia;
using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Media.Host.Controllers;

[ApiController]
[Route("api/v1/media")]
[Authorize]
public class MediaController(IMediator mediator, IStorageProvider storageProvider) : ControllerBase
{
    [HttpPost("chunk/init")]
    public async Task<IActionResult> InitiateUpload([FromBody] InitiateUploadRequest request)
    {
        var result = await mediator.Send(
            new InitiateUploadCommand(request.FileName, request.ContentType, request.TotalFileSize));

        return result.Match(
            response => Ok(response),
            errors => Problem(errors.First().Description));
    }

    [HttpPost("chunk/{mediaId}/{chunkIndex:int}")]
    public async Task<IActionResult> UploadChunk(Guid mediaId, int chunkIndex, IFormFile chunk)
    {
        var result = await mediator.Send(
            new UploadChunkCommand(mediaId, chunkIndex, chunk.OpenReadStream(), chunk.Length, chunk.ContentType));

        return result.Match(
            response => Ok(response),
            errors => Problem(errors.First().Description));
    }

    [HttpPost("chunk/{mediaId}/complete")]
    public async Task<IActionResult> CompleteUpload(Guid mediaId)
    {
        var result = await mediator.Send(new CompleteUploadCommand(mediaId));

        return result.Match(
            response => Accepted($"/api/v1/media/{mediaId}", response),
            errors => Problem(errors.First().Description));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await mediator.Send(new GetMediaQuery(id));

        return result.Match(
            response => response.Status == MediaStatus.Uploading
                ? Accepted($"/api/v1/media/{id}", response)
                : Ok(response),
            errors => Problem(errors.First().Description));
    }

    [HttpGet("{id:guid}/url")]
    public async Task<IActionResult> GetUrl(Guid id, [FromQuery] string variant = "display")
    {
        var mediaResult = await mediator.Send(new GetMediaQuery(id));
        if (mediaResult.IsError)
            return Problem(mediaResult.FirstError.Description);

        var variantData = mediaResult.Value.Variants?
            .FirstOrDefault(v => v.VariantType == variant);

        if (variantData is null)
            return NotFound();

        var storageKey = $"media/{id}/{variant}.webp";
        var urlResult = await storageProvider.GetPresignedUrlAsync(storageKey, TimeSpan.FromHours(1));

        return urlResult.Match(
            url => Ok(new MediaUrlResponse(url)),
            errors => Problem(errors.First().Description));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var mediaResult = await mediator.Send(new GetMediaQuery(id));
        if (mediaResult.IsError)
            return Problem(mediaResult.FirstError.Description);

        var deleteResult = await storageProvider.DeleteAsync($"media/{id}");
        return deleteResult.Match<IActionResult>(
            _ => NoContent(),
            errors => Problem(errors.First().Description));
    }
}
