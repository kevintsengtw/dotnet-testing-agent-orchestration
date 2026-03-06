namespace Practice.Core.Net8.Models;

/// <summary>
/// 訂單狀態
/// </summary>
public enum OrderStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled
}
