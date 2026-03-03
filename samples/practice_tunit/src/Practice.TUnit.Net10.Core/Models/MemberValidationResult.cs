namespace Practice.TUnit.Net10.Core.Models;

/// <summary>
/// 會員驗證結果
/// </summary>
public class MemberValidationResult
{
    /// <summary>是否通過驗證</summary>
    public bool IsValid { get; set; }

    /// <summary>驗證錯誤訊息清單</summary>
    public List<string> Errors { get; set; } = new();
}
