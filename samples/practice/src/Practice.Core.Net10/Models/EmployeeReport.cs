namespace Practice.Core.Net10.Models;

/// <summary>
/// 員工報告
/// </summary>
public class EmployeeReport
{
    /// <summary>
    /// 報告產生時間
    /// </summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>
    /// 員工總數
    /// </summary>
    public int TotalEmployees { get; set; }

    /// <summary>
    /// 薪資總額
    /// </summary>
    public decimal TotalSalary { get; set; }

    /// <summary>
    /// 部門人數分布
    /// </summary>
    public Dictionary<string, int> DepartmentBreakdown { get; set; } = new();

    /// <summary>
    /// 技能分布
    /// </summary>
    public Dictionary<string, int> SkillsDistribution { get; set; } = new();
}
