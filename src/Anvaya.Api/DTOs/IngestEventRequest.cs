using System.ComponentModel.DataAnnotations;

namespace Anvaya.Api.DTOs;

public class IngestEventRequest
{
    [Required]
    public string TenantId { get; set; } = string.Empty;

    [Required]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    public string EventName { get; set; } = string.Empty;

    public decimal Value { get; set; }

    public string Currency { get; set; } = "USD";

    public Dictionary<string, string> Attributes { get; set; } = new();
}
