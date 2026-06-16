using Dukaan.Media.Application.Features.Media.Commands.DeleteMedia;
using Dukaan.Media.Application.Features.Media.Dtos;
using Dukaan.Media.Application.Features.Media.Queries.GetMedia;
using Dukaan.Media.Application.Features.Media.Queries.GetMediaUrl;
using Dukaan.Media.Application.Features.Uploads.Commands.CompleteUpload;
using Dukaan.Media.Application.Features.Uploads.Commands.InitiateUpload;
using Dukaan.Media.Application.Features.Uploads.Commands.UploadChunk;
using Dukaan.Media.Application.Features.Uploads.Dtos;
using Dukaan.Media.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Media.Host.Controllers;

[Authorize]
public class MediaController : BaseApiController
{
    [HttpPost("chunk/init")]
    public async Task<ActionResult<InitiateUploadResponse>> InitiateUpload(InitiateUploadCommand command)
        => ToActionResult(await Mediator.Send(command));

    [HttpPost("chunk/{mediaId}/{chunkIndex:int}")]
    public async Task<ActionResult<UploadChunkResponse>> UploadChunk(Guid mediaId, int chunkIndex, IFormFile chunk)
        => ToActionResult(await Mediator.Send(
            new UploadChunkCommand(mediaId, chunkIndex, chunk.OpenReadStream(), chunk.Length, chunk.ContentType)));

    [HttpPost("chunk/{mediaId}/complete")]
    public async Task<ActionResult<CompleteUploadResponse>> CompleteUpload(Guid mediaId)
        => ToActionResult(await Mediator.Send(new CompleteUploadCommand(mediaId)));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<MediaMetadataResponse>> Get(Guid id)
    {
        var result = await Mediator.Send(new GetMediaQuery(id));
        if (result.IsError)
            return ToActionResult(result);

        return result.Value.Status == MediaStatus.Uploading
            ? Accepted($"/api/media/{id}", result.Value)
            : Ok(result.Value);
    }

    [HttpGet("{id:guid}/url")]
    public async Task<ActionResult<MediaUrlResponse>> GetUrl(Guid id, [FromQuery] string variant = "display")
        => ToActionResult(await Mediator.Send(new GetMediaUrlQuery(id, variant)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => ToActionResult(await Mediator.Send(new DeleteMediaCommand(id)));
}
