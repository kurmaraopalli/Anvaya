using Anvaya.Api.DTOs;
using Anvaya.Core.Models;
using Anvaya.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Anvaya.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EventsController : ControllerBase
{
    private readonly LoyaltyEngineService _loyaltyEngineService;

    public EventsController(LoyaltyEngineService loyaltyEngineService)
    {
        _loyaltyEngineService = loyaltyEngineService;
    }

    [HttpPost]
    public async Task<IActionResult> IngestEvent(
        [FromBody] IngestEventRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var loyaltyEvent = new LoyaltyEvent
        {
            EventId = $"evt_{Guid.NewGuid():N}",
            TenantId = request.TenantId,
            CustomerId = request.CustomerId,
            EventName = request.EventName,
            Value = request.Value,
            Currency = request.Currency,
            Attributes = request.Attributes,
            IdempotencyKey = idempotencyKey,
            ProcessedUtc = DateTime.UtcNow
        };

        var result = await _loyaltyEngineService.ProcessEventAsync(loyaltyEvent);
        return Ok(result);
    }
}
