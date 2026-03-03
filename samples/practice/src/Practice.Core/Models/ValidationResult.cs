namespace Practice.Core.Models;

/// <summary>
/// 驗證結果
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// 是否通過驗證
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 錯誤訊息列表
    /// </summary>
    public List<string> Errors { get; set; } = new();
}
