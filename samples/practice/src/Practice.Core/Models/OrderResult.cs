namespace Practice.Core.Models;

/// <summary>
/// 訂單處理結果
/// </summary>
public class OrderResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public Guid? OrderId { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 處理時間
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; set; }

    /// <summary>
    /// 建立成功結果
    /// </summary>
    /// <param name="orderId">訂單識別碼</param>
    /// <param name="processedAt">處理時間</param>
    /// <returns>成功的訂單結果</returns>
    public static OrderResult Succeeded(Guid orderId, DateTimeOffset processedAt)
        => new() { Success = true, OrderId = orderId, ProcessedAt = processedAt };

    /// <summary>
    /// 建立失敗結果
    /// </summary>
    /// <param name="errorMessage">錯誤訊息</param>
    /// <returns>失敗的訂單結果</returns>
    public static OrderResult Failed(string errorMessage)
        => new() { Success = false, ErrorMessage = errorMessage };
}
