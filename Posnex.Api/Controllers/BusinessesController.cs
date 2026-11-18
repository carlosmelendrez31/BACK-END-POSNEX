using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posnex.Api.Contracts.Businesses;
using Posnex.BLL.Businesses;
using Posnex.Entities;

namespace Posnex.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/businesses")]
public sealed class BusinessesController(IBusinessService businessService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BusinessResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BusinessResponse>>> List(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var businesses = await businessService.ListAsync(userId.Value, cancellationToken);
        return Ok(businesses.Select(BusinessResponse.From).ToArray());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BusinessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BusinessResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var business = await businessService.GetAsync(id, userId.Value, cancellationToken);
        return business is null ? NotFound() : Ok(BusinessResponse.From(business));
    }

    [HttpPost]
    [ProducesResponseType(typeof(BusinessResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BusinessResponse>> Create(
        CreateBusinessRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        if (!Enum.TryParse<BusinessType>(request.Type, ignoreCase: true, out var type) || !Enum.IsDefined(type))
            return BadRequest(new ProblemDetails { Title = "El tipo debe ser Ecommerce o Restaurant." });

        var created = await businessService.CreateAsync(
            new CreateBusinessCommand(request.Name, type, request.PrimaryColor, request.SecondaryColor, request.LogoUrl),
            userId.Value,
            cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = created.Id }, BusinessResponse.From(created));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BusinessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BusinessResponse>> Update(
        Guid id,
        UpdateBusinessRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var updated = await businessService.UpdateAsync(
            id,
            new UpdateBusinessCommand(request.Name, request.PrimaryColor, request.SecondaryColor, request.LogoUrl),
            userId.Value,
            cancellationToken);

        return updated is null ? NotFound() : Ok(BusinessResponse.From(updated));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var deleted = await businessService.DeleteAsync(id, userId.Value, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private Guid? GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
