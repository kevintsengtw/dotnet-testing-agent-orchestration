namespace Practice.TUnit.Net10.Core.Models;

/// <summary>
/// 書籍狀態
/// </summary>
public enum BookStatus
{
    /// <summary>可借閱</summary>
    Available,

    /// <summary>已借出</summary>
    OnLoan,

    /// <summary>已預約</summary>
    Reserved,

    /// <summary>已下架</summary>
    Archived
}
