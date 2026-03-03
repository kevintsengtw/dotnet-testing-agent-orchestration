namespace Practice.TUnit.Net10.Core.Models;

/// <summary>
/// 借閱狀態
/// </summary>
public enum LoanStatus
{
    /// <summary>借閱中</summary>
    Active,

    /// <summary>已歸還</summary>
    Returned,

    /// <summary>已逾期</summary>
    Overdue,

    /// <summary>已續借</summary>
    Renewed
}
