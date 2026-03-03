namespace Practice.TUnit.Net8.Core.Models;

/// <summary>
/// 借閱狀態
/// </summary>
public enum LoanStatus
{
    Active,
    Returned,
    Overdue,
    Renewed
}

/// <summary>
/// 借閱紀錄
/// </summary>
public class Loan
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public Guid MemberId { get; set; }
    public DateTimeOffset LoanDate { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public DateTimeOffset? ReturnDate { get; set; }
    public LoanStatus Status { get; set; } = LoanStatus.Active;
    public int RenewalCount { get; set; }
    public int MaxRenewals { get; set; } = 2;
    public decimal OverdueFine { get; set; }
}
