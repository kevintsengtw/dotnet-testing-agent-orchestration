using Practice.TUnit.Net10.Core.Interfaces;
using Practice.TUnit.Net10.Core.Models;

namespace Practice.TUnit.Net10.Core.Services;

/// <summary>
/// 會員服務 — P3-2 驗證：Mock 依賴、MethodDataSource / Matrix 進階測試
/// 管理會員註冊、驗證、升級等業務邏輯
/// </summary>
public class LibraryMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly INotificationService _notificationService;

    public LibraryMemberService(
        IMemberRepository memberRepository,
        ILoanRepository loanRepository,
        INotificationService notificationService)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    /// <summary>
    /// 註冊新會員
    /// </summary>
    /// <param name="member">會員資料</param>
    /// <returns>註冊後的會員</returns>
    public async Task<LibraryMember> RegisterMemberAsync(LibraryMember member)
    {
        if (member == null)
            throw new ArgumentNullException(nameof(member));

        if (string.IsNullOrWhiteSpace(member.Email))
            throw new ArgumentException("Email is required", nameof(member));

        if (!member.Email.Contains('@'))
            throw new ArgumentException("Invalid email format", nameof(member));

        var existing = await _memberRepository.GetByEmailAsync(member.Email);
        if (existing != null)
            throw new InvalidOperationException($"Member with email '{member.Email}' already exists");

        member.Id = Guid.NewGuid();
        member.JoinDate = DateTime.UtcNow;
        member.IsActive = true;
        member.MembershipType = MembershipType.Basic;

        await _memberRepository.AddAsync(member);

        return member;
    }

    /// <summary>
    /// 驗證會員資料
    /// </summary>
    /// <param name="member">會員</param>
    /// <returns>驗證結果</returns>
    public MemberValidationResult ValidateMember(LibraryMember member)
    {
        if (member == null)
            throw new ArgumentNullException(nameof(member));

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(member.Name))
            errors.Add("Name is required");

        if (member.Name?.Length > 100)
            errors.Add("Name cannot exceed 100 characters");

        if (string.IsNullOrWhiteSpace(member.Email))
            errors.Add("Email is required");
        else if (!member.Email.Contains('@'))
            errors.Add("Invalid email format");

        if (member.JoinDate > DateTime.UtcNow)
            errors.Add("Join date cannot be in the future");

        if (member.JoinDate < new DateTime(2000, 1, 1))
            errors.Add("Join date is too far in the past");

        if (!string.IsNullOrEmpty(member.PhoneNumber))
        {
            var digitsOnly = new string(member.PhoneNumber.Where(char.IsDigit).ToArray());
            if (digitsOnly.Length < 8 || digitsOnly.Length > 15)
                errors.Add("Phone number must be between 8 and 15 digits");
        }

        return new MemberValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors.AsReadOnly()
        };
    }

    /// <summary>
    /// 升級會員等級
    /// </summary>
    /// <param name="memberId">會員 ID</param>
    /// <param name="newType">新等級</param>
    /// <returns>升級後的會員</returns>
    public async Task<LibraryMember> UpgradeMembershipAsync(Guid memberId, MembershipType newType)
    {
        var member = await _memberRepository.GetByIdAsync(memberId)
                     ?? throw new KeyNotFoundException($"Member '{memberId}' not found");

        if (!member.IsActive)
            throw new InvalidOperationException("Cannot upgrade inactive member");

        if (newType <= member.MembershipType)
            throw new InvalidOperationException(
                $"New membership type ({newType}) must be higher than current ({member.MembershipType})");

        member.MembershipType = newType;
        await _memberRepository.UpdateAsync(member);

        return member;
    }

    /// <summary>
    /// 檢查會員是否可以借書
    /// </summary>
    /// <param name="memberId">會員 ID</param>
    /// <returns>是否可借書</returns>
    public async Task<bool> CanBorrowAsync(Guid memberId)
    {
        var member = await _memberRepository.GetByIdAsync(memberId)
                     ?? throw new KeyNotFoundException($"Member '{memberId}' not found");

        if (!member.IsActive)
            return false;

        var activeLoans = await _loanRepository.GetActiveLoansByMemberAsync(memberId);
        if (activeLoans.Count >= member.MaxBooksAllowed)
            return false;

        // 檢查是否有逾期未還的書
        if (activeLoans.Any(l => l.Status == LoanStatus.Overdue))
            return false;

        return true;
    }

    /// <summary>
    /// 計算年費
    /// </summary>
    /// <param name="membershipType">會員等級</param>
    /// <param name="isRenewal">是否為續約</param>
    /// <returns>年費金額</returns>
    public decimal CalculateAnnualFee(MembershipType membershipType, bool isRenewal = false)
    {
        var baseFee = membershipType switch
        {
            MembershipType.Basic => 0m,
            MembershipType.Premium => 500m,
            MembershipType.Vip => 1200m,
            _ => throw new ArgumentOutOfRangeException(nameof(membershipType))
        };

        // 續約享 10% 折扣
        if (isRenewal && baseFee > 0)
            baseFee *= 0.9m;

        return baseFee;
    }
}

/// <summary>
/// 會員驗證結果
/// </summary>
public class MemberValidationResult
{
    public bool IsValid { get; set; }
    public IReadOnlyList<string> Errors { get; set; } = Array.Empty<string>();
}
