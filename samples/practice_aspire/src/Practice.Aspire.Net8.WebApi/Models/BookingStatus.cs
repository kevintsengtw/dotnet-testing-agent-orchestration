namespace Practice.Aspire.Net8.WebApi.Models;

/// <summary>
/// 預約狀態列舉
/// </summary>
public enum BookingStatus
{
    Pending = 0,
    Confirmed = 1,
    CheckedIn = 2,
    CheckedOut = 3,
    Cancelled = 4
}
