using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using Practice.TUnit.Net10.Core.Models;

namespace Practice.TUnit.Net10.Core.Services;

/// <summary>
/// 目錄匯出服務 — P3-5 驗證：IFileSystem 依賴、xUnit → TUnit 遷移場景
/// 使用 System.IO.Abstractions 進行檔案系統操作
/// </summary>
public class CatalogExportService
{
    private readonly IFileSystem _fileSystem;

    public CatalogExportService(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    /// <summary>
    /// 將書籍列表匯出為 CSV 檔案
    /// </summary>
    /// <param name="books">書籍列表</param>
    /// <param name="filePath">輸出檔案路徑</param>
    /// <returns>匯出的書籍數量</returns>
    public async Task<int> ExportToCsvAsync(IEnumerable<Book> books, string filePath)
    {
        if (books == null)
            throw new ArgumentNullException(nameof(books));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        var bookList = books.ToList();
        if (bookList.Count == 0)
            throw new InvalidOperationException("Cannot export empty book list");

        var sb = new StringBuilder();
        sb.AppendLine("Id,Title,Author,Isbn,Genre,PublishedDate,Price,Status,PageCount");

        foreach (var book in bookList)
        {
            sb.AppendLine(
                $"\"{book.Id}\",\"{EscapeCsvField(book.Title)}\",\"{EscapeCsvField(book.Author)}\"," +
                $"\"{book.Isbn}\",\"{book.Genre}\",\"{book.PublishedDate:yyyy-MM-dd}\"," +
                $"{book.Price},{book.Status},{book.PageCount}");
        }

        var directory = _fileSystem.Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !_fileSystem.Directory.Exists(directory))
        {
            _fileSystem.Directory.CreateDirectory(directory);
        }

        await _fileSystem.File.WriteAllTextAsync(filePath, sb.ToString());

        return bookList.Count;
    }

    /// <summary>
    /// 將書籍列表匯出為 JSON 檔案
    /// </summary>
    /// <param name="books">書籍列表</param>
    /// <param name="filePath">輸出檔案路徑</param>
    /// <returns>匯出的書籍數量</returns>
    public async Task<int> ExportToJsonAsync(IEnumerable<Book> books, string filePath)
    {
        if (books == null)
            throw new ArgumentNullException(nameof(books));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        var bookList = books.ToList();
        if (bookList.Count == 0)
            throw new InvalidOperationException("Cannot export empty book list");

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(bookList, options);

        var directory = _fileSystem.Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !_fileSystem.Directory.Exists(directory))
        {
            _fileSystem.Directory.CreateDirectory(directory);
        }

        await _fileSystem.File.WriteAllTextAsync(filePath, json);

        return bookList.Count;
    }

    /// <summary>
    /// 從 CSV 檔案匯入書籍
    /// </summary>
    /// <param name="filePath">CSV 檔案路徑</param>
    /// <returns>匯入的書籍列表</returns>
    public async Task<IReadOnlyList<Book>> ImportFromCsvAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        if (!_fileSystem.File.Exists(filePath))
            throw new FileNotFoundException("CSV file not found", filePath);

        var content = await _fileSystem.File.ReadAllTextAsync(filePath);
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
            throw new InvalidOperationException("CSV file is empty or has no data rows");

        var books = new List<Book>();

        // 跳過標題列
        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            var fields = ParseCsvLine(line);
            if (fields.Length < 9)
                continue;

            try
            {
                var book = new Book
                {
                    Id = Guid.Parse(fields[0]),
                    Title = fields[1],
                    Author = fields[2],
                    Isbn = fields[3],
                    Genre = Enum.Parse<BookGenre>(fields[4]),
                    PublishedDate = DateTime.Parse(fields[5]),
                    Price = decimal.Parse(fields[6]),
                    Status = Enum.Parse<BookStatus>(fields[7]),
                    PageCount = int.Parse(fields[8])
                };

                books.Add(book);
            }
            catch (FormatException)
            {
                // 跳過格式錯誤的行
                continue;
            }
        }

        return books.AsReadOnly();
    }

    /// <summary>
    /// 產生庫存報告
    /// </summary>
    /// <param name="books">書籍列表</param>
    /// <param name="filePath">輸出檔案路徑</param>
    /// <returns>報告摘要</returns>
    public async Task<InventoryReport> GenerateInventoryReportAsync(
        IEnumerable<Book> books, string filePath)
    {
        if (books == null)
            throw new ArgumentNullException(nameof(books));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        var bookList = books.ToList();

        var report = new InventoryReport
        {
            GeneratedAt = DateTime.UtcNow,
            TotalBooks = bookList.Count,
            AvailableBooks = bookList.Count(b => b.Status == BookStatus.Available),
            OnLoanBooks = bookList.Count(b => b.Status == BookStatus.OnLoan),
            ReservedBooks = bookList.Count(b => b.Status == BookStatus.Reserved),
            ArchivedBooks = bookList.Count(b => b.Status == BookStatus.Archived),
            GenreDistribution = bookList
                .GroupBy(b => b.Genre)
                .ToDictionary(g => g.Key.ToString(), g => g.Count()),
            TotalValue = bookList.Sum(b => b.Price),
            AveragePrice = bookList.Count > 0 ? bookList.Average(b => b.Price) : 0m
        };

        var sb = new StringBuilder();
        sb.AppendLine("=== 圖書館庫存報告 ===");
        sb.AppendLine($"產生時間: {report.GeneratedAt:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();
        sb.AppendLine("── 書籍狀態 ──");
        sb.AppendLine($"總計: {report.TotalBooks} 本");
        sb.AppendLine($"可借閱: {report.AvailableBooks} 本");
        sb.AppendLine($"已借出: {report.OnLoanBooks} 本");
        sb.AppendLine($"已預約: {report.ReservedBooks} 本");
        sb.AppendLine($"已封存: {report.ArchivedBooks} 本");
        sb.AppendLine();
        sb.AppendLine("── 類型分佈 ──");

        foreach (var (genre, count) in report.GenreDistribution.OrderByDescending(x => x.Value))
        {
            sb.AppendLine($"{genre}: {count} 本");
        }

        sb.AppendLine();
        sb.AppendLine("── 價格統計 ──");
        sb.AppendLine($"總價值: ${report.TotalValue:F2}");
        sb.AppendLine($"平均價格: ${report.AveragePrice:F2}");

        var directory = _fileSystem.Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !_fileSystem.Directory.Exists(directory))
        {
            _fileSystem.Directory.CreateDirectory(directory);
        }

        await _fileSystem.File.WriteAllTextAsync(filePath, sb.ToString());

        return report;
    }

    /// <summary>
    /// 跳脫 CSV 欄位中的雙引號
    /// </summary>
    private static string EscapeCsvField(string field)
    {
        return field.Replace("\"", "\"\"");
    }

    /// <summary>
    /// 解析 CSV 行（處理引號包圍的欄位）
    /// </summary>
    private static string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            if (line[i] == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (line[i] == ',' && !inQuotes)
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(line[i]);
            }
        }

        fields.Add(current.ToString());
        return fields.ToArray();
    }
}

/// <summary>
/// 庫存報告
/// </summary>
public class InventoryReport
{
    public DateTime GeneratedAt { get; set; }
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int OnLoanBooks { get; set; }
    public int ReservedBooks { get; set; }
    public int ArchivedBooks { get; set; }
    public Dictionary<string, int> GenreDistribution { get; set; } = new();
    public decimal TotalValue { get; set; }
    public decimal AveragePrice { get; set; }
}
