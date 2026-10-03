using Anvaya.Api.DTOs;
using Anvaya.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Anvaya.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RedemptionsController : ControllerBase
{
    private readonly RedemptionService _redemptionService;

    public RedemptionsController(RedemptionService redemptionService)
    {
        _redemptionService = redemptionService;
    }

    [HttpPost]
    public async Task<IActionResult> RedeemPoints([FromBody] RedeemPointsRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _redemptionService.RedeemPointsAsync(
            request.TenantId,
            request.CustomerId,
            request.RewardId,
            request.PointsToRedeem,
            request.ReferenceOrderId);

        if (result.Status == "Failed")
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
