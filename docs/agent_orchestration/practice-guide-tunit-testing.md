# TUnit 測試 Orchestrator 操作指南

> **Orchestrator**：`dotnet-testing-advanced-tunit-orchestrator`
> **驗證專案**：`samples/practice_tunit/`（.NET 9.0 基線版本，使用 `Practice.TUnit.slnx`）

---

## 可驗證版本

本指南以 **.NET 9.0** 為基線版本。三個版本的驗證專案結構完全相同，可自由選擇：

| 版本         | .slnx                       | 來源專案路徑                                    |
| ------------ | --------------------------- | ----------------------------------------------- |
| **.NET 9.0** | `Practice.TUnit.slnx`       | `practice_tunit/src/Practice.TUnit.Core/`       |
| .NET 8.0     | `Practice.TUnit.Net8.slnx`  | `practice_tunit/src/Practice.TUnit.Net8.Core/`  |
| .NET 10.0    | `Practice.TUnit.Net10.slnx` | `practice_tunit/src/Practice.TUnit.Net10.Core/` |

> 驗證其他版本時，將情境中的檔案路徑替換為對應版本的專案路徑即可。例如：
> `#file:practice_tunit/src/Practice.TUnit.Net8.Core/Services/BookCatalog.cs`

### 還原驗證結果

```powershell
git restore samples/practice_tunit/tests/
git clean -fd samples/practice_tunit/tests/
```

---

## 前置準備

| 項目             | 說明                                       |
| ---------------- | ------------------------------------------ |
| **VS Code**      | 1.109 以上，已安裝 GitHub Copilot Chat     |
| **VS Code 設定** | `chat.customAgentInSubagent.enabled: true` |
| **.NET SDK**     | .NET 9.0 SDK                               |

> Docker Desktop **不需要**。

## 操作步驟

1. 在 Copilot Chat 切換到 **Agent** 模式
2. 從 Agent 下拉選單選擇 `dotnet-testing-advanced-tunit-orchestrator`
3. 使用 `#file:` 引用目標檔案，或直接描述測試目標

## 預期結果

測試程式碼產生在 `samples/practice_tunit/tests/Practice.TUnit.Core.Tests/` 目錄中。

---

## 驗證情境

### 情境 1：純函式 TUnit 測試

```plaintext
#file:practice_tunit/src/Practice.TUnit.Core/Services/BookCatalog.cs

為 BookCatalog 類別寫 TUnit 測試
```

> 驗證 Orchestrator 對無依賴純函式類別的 TUnit 測試撰寫能力，確認使用 TUnit 屬性而非 xUnit。

**觀察重點**：

- **Analyzer**：是否正確識別 TUnit 專案結構（`OutputType=Exe`），判斷為無依賴的純函式類別
- **Writer**：是否使用 `[Test]`（非 `[Fact]`）、`[Arguments]`（非 `[InlineData]`），方法是否為 `async Task`
- **Executor**：是否使用 `dotnet run`（**非** `dotnet test`）執行測試
- **Reviewer**：是否符合 TUnit 規範，無 xUnit 殘留（`[Fact]`、`[Theory]`、`ITestOutputHelper`）

---

### 情境 2：Mock 依賴 + 複雜驗證邏輯

```plaintext
#file:practice_tunit/src/Practice.TUnit.Core/Services/LibraryMemberService.cs

為 LibraryMemberService 寫 TUnit 測試
```

> 驗證 Writer 能否在 TUnit 框架下正確使用 NSubstitute Mock 依賴，並使用 `[MethodDataSource]` 進行多場景測試。

**觀察重點**：

- **Analyzer**：是否識別 `IMemberRepository`、`ILoanRepository`、`INotificationService` 三個依賴
- **Writer**：是否使用 `[MethodDataSource]`（非 `[MemberData]`）提供多組測試資料，Mock 設定是否正確
- **Executor**：`dotnet run` 是否通過所有測試
- **Reviewer**：會員驗證、升級、借閱檢查等場景覆蓋是否完整

---

### 情境 3：狀態轉換邏輯測試

```plaintext
#file:practice_tunit/src/Practice.TUnit.Core/Services/LoanService.cs

為 LoanService 寫 TUnit 測試
```

> 驗證 Writer 能否測試複雜的狀態機邏輯（Active → Returned / Renewed / Overdue），並正確使用 `TimeProvider` + `FakeTimeProvider`。

**觀察重點**：

- **Analyzer**：是否識別五個依賴項與 `TimeProvider` 時間依賴
- **Writer**：是否使用 `FakeTimeProvider` 控制時間推進，測試借閱 → 歸還 → 續借 → 逾期等流程
- **Executor**：`dotnet run` 是否成功執行
- **Reviewer**：狀態轉換的邊界條件（剛好到期、超過續借上限）是否涵蓋

---

### 情境 4：TimeProvider 時間敏感邏輯

```plaintext
#file:practice_tunit/src/Practice.TUnit.Core/Services/ReservationService.cs

為 ReservationService 寫 TUnit 測試
```

> 驗證 Writer 對使用 `TimeProvider` 管理預約到期、保留期限等時間敏感邏輯的測試撰寫能力。

**觀察重點**：

- **Analyzer**：是否識別 `TimeProvider` 依賴與預約保留天數常數
- **Writer**：是否使用 `FakeTimeProvider` 模擬時間流逝，測試預約過期場景
- **Reviewer**：時間邊界（保留期限內 / 剛好到期 / 過期）是否涵蓋

---

### 情境 5：IFileSystem 依賴測試

```plaintext
#file:practice_tunit/src/Practice.TUnit.Core/Services/CatalogExportService.cs

為 CatalogExportService 寫 TUnit 測試
```

> 驗證 Writer 能否在 TUnit 框架下正確使用 `MockFileSystem` 測試檔案系統操作。

**觀察重點**：

- **Analyzer**：是否識別 `IFileSystem` 依賴
- **Writer**：是否使用 `MockFileSystem` 模擬檔案系統，TUnit 的資源清理是否使用 `async DisposeAsync`（非 `IDisposable`）
- **Reviewer**：CSV / JSON 匯出、目錄建立、錯誤處理等場景是否涵蓋

---

### 情境 6：多目標平行處理

```plaintext
幫 LoanService 和 ReservationService 寫 TUnit 測試
```

> 驗證 Orchestrator 在純文字描述多個目標時，能否自行定位並平行處理。

**觀察重點**：

- **Analyzer**：是否能搜尋到兩個 Service 並分別分析依賴
- **Writer**：是否為兩個目標各自產生獨立的測試類別
- **Executor**：所有測試是否使用 `dotnet run` 一次通過
- **Reviewer**：兩個測試類別的品質是否一致

---

### 情境 7：xUnit → TUnit 遷移

```plaintext
#file:practice_tunit/migration_source/BookCatalogXunitTests.cs

將這個 xUnit 測試遷移到 TUnit
```

> 驗證 Orchestrator 能否正確偵測 xUnit 測試模式並轉換為 TUnit 框架。

**觀察重點**：

- **Analyzer**：是否識別出 xUnit 模式（`[Fact]`、`[Theory]`、`[InlineData]`、`[MemberData]`、`IDisposable`、`ITestOutputHelper`）
- **Writer**：遷移對應是否正確：
  - `[Fact]` → `[Test]`
  - `[Theory]` → `[Test]` + `[Arguments]`
  - `[InlineData]` → `[Arguments]`
  - `[MemberData]` → `[MethodDataSource]`
  - `IDisposable` → `async DisposeAsync`
  - `ITestOutputHelper` → TUnit 內建 logging
- **Executor**：遷移後的測試是否使用 `dotnet run` 通過
- **Reviewer**：是否無 xUnit 殘留（`using Xunit;`、`using Xunit.Abstractions;`）


