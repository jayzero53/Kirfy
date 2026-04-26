using kirfy_api.Application.Abstractions;
using kirfy_api.Presentation.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace kirfy_api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ArtistsController(IArtistAlbumsProvider artistAlbumsProvider) : ControllerBase
{
    [HttpGet("{artistName}/albums")]
    [ProducesResponseType(typeof(ArtistAlbumsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArtistAlbumsResponse>> GetAlbumsByArtistName(
        [FromRoute] string artistName,
        CancellationToken cancellationToken)
    {
        var result = await artistAlbumsProvider.GetByArtistNameAsync(artistName, cancellationToken);
        if (result is null)
        {
            return NotFound(new { Message = $"Artist '{artistName}' was not found on Genius." });
        }

        var response = new ArtistAlbumsResponse(
            ArtistId: result.ArtistId,
            ArtistName: result.ArtistName,
            Albums: result.Albums.Select(album =>
                    new ArtistAlbumResponse(
                        Id: album.Id,
                        Name: album.Name,
                        Url: album.Url,
                        CoverArtUrl: album.CoverArtUrl,
                        ReleaseDate: album.ReleaseDate))
                .ToArray());

        return Ok(response);
    }
}
