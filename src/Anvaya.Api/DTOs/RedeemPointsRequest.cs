using System.ComponentModel.DataAnnotations;

namespace Anvaya.Api.DTOs;

public class RedeemPointsRequest
{
    [Required]
    public string TenantId { get; set; } = string.Empty;

    [Required]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    public string RewardId { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int PointsToRedeem { get; set; }

    [Required]
    public string ReferenceOrderId { get; set; } = string.Empty;
}
