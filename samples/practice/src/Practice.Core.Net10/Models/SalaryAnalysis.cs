namespace Practice.Core.Net10.Models;

/// <summary>
/// 薪資分析結果
/// </summary>
public class SalaryAnalysis
{
    /// <summary>
    /// 部門名稱
    /// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// 員工數量
    /// </summary>
    public int EmployeeCount { get; set; }

    /// <summary>
    /// 薪資總額
    /// </summary>
    public decimal TotalSalary { get; set; }

    /// <summary>
    /// 平均薪資
    /// </summary>
    public decimal AverageSalary { get; set; }

    /// <summary>
    /// 最低薪資
    /// </summary>
    public decimal MinSalary { get; set; }

    /// <summary>
    /// 最高薪資
    /// </summary>
    public decimal MaxSalary { get; set; }
}
