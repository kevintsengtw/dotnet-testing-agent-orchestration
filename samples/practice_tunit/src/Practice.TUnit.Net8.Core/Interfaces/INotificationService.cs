namespace Practice.TUnit.Net8.Core.Interfaces;

/// <summary>
/// 通知服務介面
/// </summary>
public interface INotificationService
{
    Task SendOverdueNoticeAsync(Guid memberId, string bookTitle, int overdueDays);
    Task SendReservationReadyNoticeAsync(Guid memberId, string bookTitle);
    Task SendDueSoonReminderAsync(Guid memberId, string bookTitle, int daysRemaining);
}
