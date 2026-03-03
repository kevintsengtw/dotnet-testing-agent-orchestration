namespace Practice.Integration.WebApi.Net8.Models;

/// <summary>
/// 訂單狀態列舉
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
