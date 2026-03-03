namespace Practice.TUnit.Net8.Core.Models;

/// <summary>
/// 預約狀態
/// </summary>
public enum ReservationStatus
{
    Active,
    Fulfilled,
    Expired,
    Cancelled
}

/// <summary>
/// 書籍預約
/// </summary>
public class Reservation
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public Guid MemberId { get; set; }
    public DateTimeOffset ReservedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;
}
