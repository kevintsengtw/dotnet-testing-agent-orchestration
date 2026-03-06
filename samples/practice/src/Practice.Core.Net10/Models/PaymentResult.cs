namespace Practice.Core.Net10.Models;

/// <summary>
/// 付款結果
/// </summary>
public class PaymentResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 交易識別碼
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 建立成功結果
    /// </summary>
    /// <param name="transactionId">交易識別碼</param>
    /// <returns>成功的付款結果</returns>
    public static PaymentResult Succeeded(string transactionId)
        => new() { Success = true, TransactionId = transactionId };

    /// <summary>
    /// 建立失敗結果
    /// </summary>
    /// <param name="errorMessage">錯誤訊息</param>
    /// <returns>失敗的付款結果</returns>
    public static PaymentResult Failed(string errorMessage)
        => new() { Success = false, ErrorMessage = errorMessage };
}
