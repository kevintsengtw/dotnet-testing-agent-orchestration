namespace Practice.Core.Legacy;

/// <summary>
/// 使用者記錄
/// </summary>
public class UserRecord
{
    /// <summary>
    /// 使用者識別碼
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;
}
