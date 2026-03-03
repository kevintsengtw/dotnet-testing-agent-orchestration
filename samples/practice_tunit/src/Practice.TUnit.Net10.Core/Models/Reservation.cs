namespace Practice.TUnit.Net10.Core.Models;

/// <summary>
/// 書籍預約模型
/// </summary>
public class Reservation
{
    /// <summary>預約唯一識別碼</summary>
    public Guid Id { get; set; }

    /// <summary>書籍 ID</summary>
    public Guid BookId { get; set; }

    /// <summary>會員 ID</summary>
    public Guid MemberId { get; set; }

    /// <summary>預約時間</summary>
    public DateTimeOffset ReservedAt { get; set; }

    /// <summary>預約到期時間</summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>預約狀態</summary>
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;
}
