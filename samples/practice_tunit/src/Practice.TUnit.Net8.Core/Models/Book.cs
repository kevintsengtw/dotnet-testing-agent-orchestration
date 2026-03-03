namespace Practice.TUnit.Net8.Core.Models;

/// <summary>
/// 書籍模型
/// </summary>
public class Book
{
    /// <summary>書籍唯一識別碼</summary>
    public Guid Id { get; set; }

    /// <summary>書名</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>作者</summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>ISBN（國際標準書號）</summary>
    public string Isbn { get; set; } = string.Empty;

    /// <summary>書籍類型</summary>
    public BookGenre Genre { get; set; }

    /// <summary>出版日期</summary>
    public DateTime PublishedDate { get; set; }

    /// <summary>定價</summary>
    public decimal Price { get; set; }

    /// <summary>書籍狀態</summary>
    public BookStatus Status { get; set; } = BookStatus.Available;

    /// <summary>頁數</summary>
    public int PageCount { get; set; }
}
