namespace Practice.TUnit.Core.Models;

/// <summary>
/// 借閱摘要
/// </summary>
public class LoanSummary
{
    /// <summary>會員 ID</summary>
    public Guid MemberId { get; set; }

    /// <summary>會員姓名</summary>
    public string MemberName { get; set; } = string.Empty;

    /// <summary>目前借閱數量</summary>
    public int ActiveLoanCount { get; set; }

    /// <summary>最大可借閱數量</summary>
    public int MaxAllowed { get; set; }

    /// <summary>剩餘可借閱數量</summary>
    public int RemainingSlots { get; set; }

    /// <summary>逾期數量</summary>
    public int OverdueCount { get; set; }

    /// <summary>即將到期數量</summary>
    public int DueSoonCount { get; set; }
}
