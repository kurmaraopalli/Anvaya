namespace Anvaya.Core.Models;

public class RedemptionResult
{
    public required string RedemptionId { get; set; }
    public required string Status { get; set; }
    public int PointsDeducted { get; set; }
    public int RemainingBalance { get; set; }
    public string? VoucherCode { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
}
