using Practice.TUnit.Net10.Core.Models;

namespace Practice.TUnit.Net10.Core.Interfaces;

/// <summary>
/// 借閱紀錄資料存取介面
/// </summary>
public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Loan>> GetActiveLoansByMemberAsync(Guid memberId);
    Task<Loan?> GetActiveLoanByBookAsync(Guid bookId);
    Task<IReadOnlyList<Loan>> GetOverdueLoansAsync();
    Task AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
}
