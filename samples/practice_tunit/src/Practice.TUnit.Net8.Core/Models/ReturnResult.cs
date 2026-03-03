namespace Practice.TUnit.Net8.Core.Models;

/// <summary>
/// 歸還結果
/// </summary>
public class ReturnResult
{
    /// <summary>借閱紀錄 ID</summary>
    public Guid LoanId { get; set; }

    /// <summary>是否逾期</summary>
    public bool IsOverdue { get; set; }

    /// <summary>逾期天數</summary>
    public int OverdueDays { get; set; }

    /// <summary>罰款金額</summary>
    public decimal Fine { get; set; }
}
