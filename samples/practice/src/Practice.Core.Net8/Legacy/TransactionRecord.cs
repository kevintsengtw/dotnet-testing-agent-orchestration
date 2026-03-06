namespace Practice.Core.Net8.Legacy;

/// <summary>
/// 交易記錄
/// </summary>
public class TransactionRecord
{
    /// <summary>
    /// 交易識別碼
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 使用者識別碼
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 交易金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 交易日期
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// 交易描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
