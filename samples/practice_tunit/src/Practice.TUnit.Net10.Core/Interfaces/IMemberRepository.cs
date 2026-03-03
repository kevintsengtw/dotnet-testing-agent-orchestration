using Practice.TUnit.Net10.Core.Models;

namespace Practice.TUnit.Net10.Core.Interfaces;

/// <summary>
/// 會員資料存取介面
/// </summary>
public interface IMemberRepository
{
    Task<LibraryMember?> GetByIdAsync(Guid id);
    Task<LibraryMember?> GetByEmailAsync(string email);
    Task<IReadOnlyList<LibraryMember>> GetAllAsync();
    Task AddAsync(LibraryMember member);
    Task UpdateAsync(LibraryMember member);
}
