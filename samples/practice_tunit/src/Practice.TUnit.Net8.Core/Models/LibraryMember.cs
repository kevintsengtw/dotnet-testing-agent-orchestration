namespace Practice.TUnit.Net8.Core.Models;

/// <summary>
/// 會員等級
/// </summary>
public enum MembershipType
{
    Basic,
    Premium,
    Vip
}

/// <summary>
/// 圖書館會員
/// </summary>
public class LibraryMember
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public MembershipType MembershipType { get; set; } = MembershipType.Basic;
    public DateTime JoinDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 根據會員等級計算最大借閱數
    /// </summary>
    public int MaxBooksAllowed => MembershipType switch
    {
        MembershipType.Basic => 3,
        MembershipType.Premium => 7,
        MembershipType.Vip => 15,
        _ => 3
    };

    /// <summary>
    /// 根據會員等級計算借閱期限（天數）
    /// </summary>
    public int LoanPeriodDays => MembershipType switch
    {
        MembershipType.Basic => 14,
        MembershipType.Premium => 21,
        MembershipType.Vip => 30,
        _ => 14
    };
}
