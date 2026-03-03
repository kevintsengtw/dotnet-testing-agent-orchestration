namespace Practice.TUnit.Net8.Core.Models;

/// <summary>
/// 預約狀態
/// </summary>
public enum ReservationStatus
{
    /// <summary>等待中</summary>
    Active,

    /// <summary>已完成（取書）</summary>
    Fulfilled,

    /// <summary>已過期</summary>
    Expired,

    /// <summary>已取消</summary>
    Cancelled
}
