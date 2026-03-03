namespace Practice.TUnit.Net10.Core.Services;

/// <summary>
/// 書籍目錄工具 — P3-1 驗證：純函式，無外部依賴
/// 提供 ISBN 驗證、價格計算、書籍分類等靜態工具方法
/// </summary>
public class BookCatalog
{
    /// <summary>
    /// 驗證 ISBN-13 格式與校驗碼
    /// </summary>
    /// <param name="isbn">ISBN 字串（可含連字號）</param>
    /// <returns>是否為有效的 ISBN-13</returns>
    public bool IsValidIsbn13(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            return false;

        // 移除連字號
        var digits = isbn.Replace("-", "");

        if (digits.Length != 13)
            return false;

        if (!digits.All(char.IsDigit))
            return false;

        // ISBN-13 校驗碼驗證
        var sum = 0;
        for (var i = 0; i < 12; i++)
        {
            var digit = digits[i] - '0';
            sum += i % 2 == 0 ? digit : digit * 3;
        }

        var checkDigit = (10 - sum % 10) % 10;
        return checkDigit == digits[12] - '0';
    }

    /// <summary>
    /// 根據會員年資計算折扣價格
    /// </summary>
    /// <param name="originalPrice">原價</param>
    /// <param name="membershipYears">會員年資</param>
    /// <returns>折扣後價格</returns>
    public decimal CalculateDiscountPrice(decimal originalPrice, int membershipYears)
    {
        if (originalPrice < 0)
            throw new ArgumentException("Price cannot be negative", nameof(originalPrice));

        var discountRate = membershipYears switch
        {
            >= 10 => 0.15m,
            >= 5 => 0.10m,
            >= 2 => 0.05m,
            _ => 0m
        };

        return originalPrice * (1 - discountRate);
    }

    /// <summary>
    /// 計算逾期罰款（每天 $1，上限為書價的 50%）
    /// </summary>
    /// <param name="overdueDays">逾期天數</param>
    /// <param name="bookPrice">書籍價格</param>
    /// <returns>罰款金額</returns>
    public decimal CalculateOverdueFine(int overdueDays, decimal bookPrice)
    {
        if (overdueDays <= 0)
            return 0m;

        var fine = overdueDays * 1.0m;
        var maxFine = bookPrice * 0.5m;

        return Math.Min(fine, maxFine);
    }

    /// <summary>
    /// 根據頁數分類書籍
    /// </summary>
    /// <param name="pageCount">頁數</param>
    /// <returns>分類名稱</returns>
    public string ClassifyByPageCount(int pageCount)
    {
        if (pageCount <= 0)
            throw new ArgumentException("Page count must be positive", nameof(pageCount));

        return pageCount switch
        {
            <= 50 => "小冊子",
            <= 150 => "薄書",
            <= 300 => "一般",
            <= 500 => "厚書",
            _ => "巨著"
        };
    }

    /// <summary>
    /// 產生書籍索引碼（格式：GENRE-INITIAL-YEAR）
    /// </summary>
    /// <param name="genre">書籍類型</param>
    /// <param name="author">作者</param>
    /// <param name="publishedYear">出版年份</param>
    /// <returns>索引碼</returns>
    public string GenerateIndexCode(string genre, string author, int publishedYear)
    {
        if (string.IsNullOrWhiteSpace(genre))
            throw new ArgumentException("Genre cannot be empty", nameof(genre));

        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be empty", nameof(author));

        if (publishedYear < 1450 || publishedYear > DateTime.Now.Year + 1)
            throw new ArgumentOutOfRangeException(nameof(publishedYear),
                "Published year must be between 1450 and next year");

        var genreCode = genre.Length <= 3
            ? genre.ToUpperInvariant()
            : genre[..3].ToUpperInvariant();

        var authorInitial = author[0].ToString().ToUpperInvariant();

        return $"{genreCode}-{authorInitial}-{publishedYear}";
    }

    /// <summary>
    /// 判斷書籍是否為經典（出版超過 50 年）
    /// </summary>
    /// <param name="publishedDate">出版日期</param>
    /// <returns>是否為經典</returns>
    public bool IsClassic(DateTime publishedDate)
    {
        var yearsSincePublished = DateTime.Today.Year - publishedDate.Year;
        if (DateTime.Today < publishedDate.AddYears(yearsSincePublished))
            yearsSincePublished--;

        return yearsSincePublished >= 50;
    }
}
