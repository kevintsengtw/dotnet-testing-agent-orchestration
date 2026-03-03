namespace Practice.Core.Models;

/// <summary>
/// 訂單模型 - Phase 5 練習：跨技能整合
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 客戶識別碼
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 客戶電子郵件
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// 訂單項目
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// 訂單總金額
    /// </summary>
    public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);

    /// <summary>
    /// 訂單建立時間
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 訂單處理時間
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; set; }

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
