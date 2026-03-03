using Practice.TUnit.Net10.Core.Models;

namespace Practice.TUnit.Net10.Core.Interfaces;

/// <summary>
/// 書籍資料存取介面
/// </summary>
public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<Book?> GetByIsbnAsync(string isbn);
    Task<IReadOnlyList<Book>> GetAllAsync();
    Task<IReadOnlyList<Book>> GetByGenreAsync(BookGenre genre);
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Guid id);
}
