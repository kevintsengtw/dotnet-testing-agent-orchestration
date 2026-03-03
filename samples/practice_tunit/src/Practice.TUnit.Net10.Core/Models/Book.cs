namespace Practice.TUnit.Net10.Core.Models;

/// <summary>
/// 書籍狀態
/// </summary>
public enum BookStatus
{
    Available,
    OnLoan,
    Reserved,
    Archived
}

/// <summary>
/// 書籍類型
/// </summary>
public enum BookGenre
{
    Fiction,
    NonFiction,
    Science,
    History,
    Biography,
    Children,
    Mystery,
    Romance,
    Technology
}

/// <summary>
/// 書籍 — 圖書館管理系統的核心領域模型
/// </summary>
public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public BookGenre Genre { get; set; }
    public DateTime PublishedDate { get; set; }
    public decimal Price { get; set; }
    public BookStatus Status { get; set; } = BookStatus.Available;
    public int PageCount { get; set; }
}
