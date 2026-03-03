# Practice TUnit — TUnit 測試驗證專案

本目錄為 `dotnet-testing-advanced-tunit-orchestrator` 的驗證專案，用於驗證 TUnit Testing Orchestrator 及其 4 個 Subagent 的泛化能力。

包含三種 .NET 版本，透過獨立的 `.slnx` 檔案管理：

| .slnx                       | .NET    | TUnit   | TimeProvider.Testing | 說明               |
| --------------------------- | ------- | ------- | -------------------- | ------------------ |
| `Practice.TUnit.slnx`       | net9.0  | 0.6.123 | 9.0.0                | 基線版本           |
| `Practice.TUnit.Net8.slnx`  | net8.0  | 0.6.123 | 8.10.0               | net8.0 跨版本驗證  |
| `Practice.TUnit.Net10.slnx` | net10.0 | 0.6.123 | 10.0.0               | net10.0 跨版本驗證 |

## 專案結構

```plaintext
practice_tunit/
├── Practice.TUnit.slnx                    # net9.0
├── Practice.TUnit.Net8.slnx               # net8.0
├── Practice.TUnit.Net10.slnx              # net10.0
├── README.md
├── migration_source/                      # xUnit → TUnit 遷移來源（三版本共用）
│   └── BookCatalogXunitTests.cs
├── src/
│   ├── Practice.TUnit.Core/              # 被測專案（net9.0）
│   ├── Practice.TUnit.Net8.Core/         # 被測專案（net8.0）
│   └── Practice.TUnit.Net10.Core/        # 被測專案（net10.0）
└── tests/
    ├── Practice.TUnit.Core.Tests/        # TUnit 測試專案（由 Orchestrator 產生）
    ├── Practice.TUnit.Net8.Core.Tests/
    └── Practice.TUnit.Net10.Core.Tests/
```

## 領域說明

以「圖書館管理系統」為主題，涵蓋書籍管理、會員管理、借閱管理、預約管理、報表匯出等功能。三個版本使用相同的領域邏輯，僅 Namespace 前綴不同。

### 被測類別與驗證場景對應

| 類別                   | 驗證場景 | 測試技術需求                               |
| ---------------------- | -------- | ------------------------------------------ |
| `BookCatalog`          | P3-1     | 純函式、`[Test]` + `[Arguments]` 參數化    |
| `LibraryMemberService` | P3-2     | Mock、`[MethodDataSource]` / `[Matrix]`    |
| `LoanService`          | P3-3     | Mock、狀態轉換、Executor `dotnet run` 驗證 |
| `ReservationService`   | P3-4     | TimeProvider、Reviewer 合規性審查          |
| `CatalogExportService` | P3-5     | IFileSystem、xUnit → TUnit 遷移場景        |

## migration_source

`migration_source/BookCatalogXunitTests.cs` 是 xUnit → TUnit 遷移驗證用的來源檔案，包含 `[Fact]`、`[Theory]`、`[MemberData]`、`IDisposable` 等 xUnit 特性，用於驗證 Orchestrator 能否正確將其轉換為 TUnit 對應的 `[Test]`、`[Arguments]`、`[MethodDataSource]`、`async DisposeAsync` 等。

## 版本差異

| 面向                 | net9.0                | net8.0                     | net10.0                     |
| -------------------- | --------------------- | -------------------------- | --------------------------- |
| TUnit                | 0.6.123               | 0.6.123                    | 0.6.123                     |
| TimeProvider.Testing | 9.0.0                 | 8.10.0                     | 10.0.0                      |
| Namespace 前綴       | `Practice.TUnit.Core` | `Practice.TUnit.Net8.Core` | `Practice.TUnit.Net10.Core` |
| 驗證重點             | 基線迴歸              | net8.0 相容性              | net10.0 向後相容性          |

## 建置與測試

```powershell
# 建置（以 net9.0 為例）
dotnet build samples/practice_tunit/Practice.TUnit.slnx -p:WarningLevel=0 /clp:ErrorsOnly --verbosity minimal

# 執行 TUnit 測試（使用 dotnet run）
dotnet run --project samples/practice_tunit/tests/Practice.TUnit.Core.Tests/Practice.TUnit.Core.Tests.csproj
```

## 注意事項

- TUnit 測試專案使用 `OutputType=Exe`，不使用 `Microsoft.NET.Test.Sdk`
- 測試執行優先使用 `dotnet run`，而非 `dotnet test`
- 所有測試方法必須為 `async Task`
