# 使用驗證專案操作 Agent Orchestrator

本文件說明如何使用 `samples/` 目錄中的驗證專案，搭配各 Agent Orchestrator 進行實際操作。

## 前置準備

### 環境需求

| 項目                | 說明                                                   |
| ------------------- | ------------------------------------------------------ |
| **VS Code**         | 1.109 以上，已安裝 GitHub Copilot Chat                 |
| **VS Code 設定**    | `chat.customAgentInSubagent.enabled: true`             |
| **.NET SDK**        | 依據目標版本安裝 .NET 8 / 9 / 10 SDK                   |
| **Docker Desktop**  | 整合測試與 Aspire 測試需要（單元測試與 TUnit 不需要）  |
| **Aspire Workload** | 僅 Aspire 測試需要（`dotnet workload install aspire`） |

### 確認 Agent 可用

1. 開啟 Copilot Chat 面板（`Ctrl+Alt+I`）
2. 在聊天輸入框上方，將模式切換為 **Agent**（預設可能是 Ask 或 Edit）
3. 點擊 Agent 名稱旁的下拉選單，確認可以看到以下四個 Orchestrator：
   - `dotnet-testing-orchestrator`
   - `dotnet-testing-advanced-integration-orchestrator`
   - `dotnet-testing-advanced-aspire-orchestrator`
   - `dotnet-testing-advanced-tunit-orchestrator`

> 如果看不到，請參考 [啟用設定](README.md#啟用設定) 進行配置。

---

## 驗證專案與 Orchestrator 對應

| 驗證專案                        | Orchestrator                                       | 測試類型        |
| ------------------------------- | -------------------------------------------------- | --------------- |
| `samples/practice/`             | `dotnet-testing-orchestrator`                      | 單元測試        |
| `samples/practice_integration/` | `dotnet-testing-advanced-integration-orchestrator` | 整合測試        |
| `samples/practice_aspire/`      | `dotnet-testing-advanced-aspire-orchestrator`      | Aspire 整合測試 |
| `samples/practice_tunit/`       | `dotnet-testing-advanced-tunit-orchestrator`       | TUnit 測試      |

每個驗證專案提供三種 .NET 版本（net8.0 / net9.0 / net10.0），透過不同的 `.slnx` 檔案管理。建議從 net9.0（基線版本）開始操作。

---

## 1. 單元測試 Orchestrator - dotnet-testing-orchestrator

**驗證專案**：`samples/practice/`

**操作步驟**：

1. 在 Copilot Chat 切換到 **Agent** 模式
2. 從 Agent 下拉選單選擇 `dotnet-testing-orchestrator`
3. 在聊天輸入框中，使用 `#file:` 引用目標檔案，或直接描述測試目標

**輸入範例**：

**情境 1：單一目標**

```plaintext
#file:samples/practice/src/Practice.Core/Services/WeatherAlertService.cs
```

> 驗證 Orchestrator 對單一目標的基本流程：Analyzer 分析 → Writer 撰寫 → Executor 執行 → Reviewer 審查。

**情境 2：多目標（驗證平行處理）**

以下為單次對話的輸入內容。先使用 `#file:` 附加三個目標檔案，再輸入指令文字：

```plaintext
#file:samples/practice/src/Practice.Core/Services/EmployeeService.cs
#file:samples/practice/src/Practice.Core/Validators/EmployeeValidator.cs
#file:samples/practice/src/Practice.Core/Validators/OrderValidator.cs
#file:samples/practice/src/Practice.Core/Legacy/LegacyReportGenerator.cs

幫 TemperatureConverter 和 SubscriptionService 寫測試
```

> 一次涵蓋六個目標（四個 `#file:` + 兩個名稱描述），驗證 Analyzer 與 Writer 是否能同時平行處理多個不同類型的目標（Service / Validator / Legacy）。

**觀察重點**：

- **Phase 1（Analyzer）**：是否正確識別目標類型（Service / Validator / Legacy）
- **Phase 2（Writer）**：是否動態載入對應的 Skills 組合
- **Phase 3（Executor）**：`dotnet test` 是否通過
- **Phase 4（Reviewer）**：品質評分與覆蓋率建議

**預期結果**：

測試程式碼產生在 `samples/practice/tests/Practice.Core.Tests/` 目錄中。

---

## 2. 整合測試 Orchestrator

**驗證專案**：`samples/practice_integration/`

**額外需求**：Docker Desktop 必須執行中。

**操作步驟**：

1. 在 Copilot Chat 切換到 **Agent** 模式
2. 從 Agent 下拉選單選擇 `dotnet-testing-advanced-integration-orchestrator`
3. 使用 `#file:` 引用 Controller 檔案，或直接描述要測試的端點

**輸入範例**（以下每列為一次獨立的對話輸入）：

| 方式            | 輸入內容                                                          |
| --------------- | ----------------------------------------------------------------- |
| `#file:` + 指令 | 測試 `#file:OrdersController.cs` 的所有 CRUD 端點                 |
| 純文字描述      | 為 OrdersController 建立使用 SQL Server Testcontainers 的整合測試 |

> `#file:` 完整路徑：`samples/practice_integration/src/Practice.Integration.WebApi/Controllers/OrdersController.cs`

**觀察重點**：

- **Phase 1（Analyzer）**：是否正確分析 API 架構、DbContext 註冊模式、容器需求
- **Phase 2（Writer）**：是否建立 WebApiFactory、IntegrationTestBase、測試類別
- **Phase 3（Executor）**：Docker 環境檢查、容器啟動與測試執行
- **Phase 4（Reviewer）**：容器配置評估、資料隔離驗證

**預期結果**：

測試程式碼產生在 `samples/practice_integration/tests/Practice.Integration.WebApi.Tests/` 目錄中，包含 `Fixtures/`、`TestBase/`、`Controllers/` 目錄結構。

---

## 3. Aspire 整合測試 Orchestrator

**驗證專案**：`samples/practice_aspire/`

**額外需求**：Docker Desktop 必須執行中，且已安裝 Aspire Workload。

**操作步驟**：

1. 在 Copilot Chat 切換到 **Agent** 模式
2. 從 Agent 下拉選單選擇 `dotnet-testing-advanced-aspire-orchestrator`
3. 使用 `#file:` 引用 AppHost 的 `Program.cs`，或直接描述要測試的服務

**輸入範例**（以下每列為一次獨立的對話輸入）：

| 方式            | 輸入內容                                                        |
| --------------- | --------------------------------------------------------------- |
| `#file:` + 指令 | 測試 `#file:Program.cs` 中 bookingapi 服務的所有 API 端點       |
| 純文字描述      | 為 Practice.Aspire.AppHost 中的 bookingapi 建立 Aspire 整合測試 |

> `#file:` 完整路徑：`samples/practice_aspire/src/Practice.Aspire.AppHost/Program.cs`

**觀察重點**：

- **Phase 1（Analyzer）**：是否正確解析 AppHost Resource 拓撲（sql、BookingsDb、cache、bookingapi）
- **Phase 2（Writer）**：是否使用 `DistributedApplicationTestingBuilder`（非 `WebApplicationFactory`），Resource 名稱是否與 AppHost 一致
- **Phase 3（Executor）**：Docker + Aspire Workload 環境檢查，超時設定是否足夠（10 分鐘+）
- **Phase 4（Reviewer）**：是否正確使用 Collection Fixture、無 WebApplicationFactory 殘留

**預期結果**：

測試程式碼產生在 `samples/practice_aspire/tests/Practice.Aspire.AppHost.Tests/` 目錄中，包含 AspireAppFixture、CollectionDefinition、IntegrationTestBase。

---

## 4. TUnit 測試 Orchestrator

**驗證專案**：`samples/practice_tunit/`

**操作步驟**：

1. 在 Copilot Chat 切換到 **Agent** 模式
2. 從 Agent 下拉選單選擇 `dotnet-testing-advanced-tunit-orchestrator`
3. 使用 `#file:` 引用目標檔案，或直接描述測試目標

**輸入範例**（以下每列為一次獨立的對話輸入）：

| 方式          | 輸入內容                                                  |
| ------------- | --------------------------------------------------------- |
| `#file:` 引用 | 測試 `#file:BookCatalog.cs`                               |
| 純文字描述    | 幫 LoanService 和 ReservationService 寫 TUnit 測試        |
| 遷移場景      | 將 `#file:BookCatalogXunitTests.cs` 從 xUnit 遷移到 TUnit |

> `#file:` 完整路徑：
>
> - `samples/practice_tunit/src/Practice.TUnit.Core/Services/BookCatalog.cs`
> - `samples/practice_tunit/migration_source/BookCatalogXunitTests.cs`

**觀察重點**：

- **Phase 1（Analyzer）**：是否正確識別 TUnit 專案結構（OutputType=Exe）
- **Phase 2（Writer）**：是否使用 TUnit 屬性（`[Test]`、`[Arguments]`、`[MethodDataSource]`），方法是否為 `async Task`
- **Phase 3（Executor）**：是否使用 `dotnet run`（非 `dotnet test`）執行
- **Phase 4（Reviewer）**：是否符合 TUnit 規範（無 xUnit 殘留）
- **遷移場景**：`[Fact]` → `[Test]`、`[Theory]` → `[Test]` + `[Arguments]`、`[MemberData]` → `[MethodDataSource]`、`IDisposable` → `async DisposeAsync`

**預期結果**：

測試程式碼產生在 `samples/practice_tunit/tests/Practice.TUnit.Core.Tests/` 目錄中。

---

## 跨版本驗證

每個驗證專案都支援三種 .NET 版本。以 Aspire 為例：

| 版本    | .slnx                        | 輸入範例                                                                                               |
| ------- | ---------------------------- | ------------------------------------------------------------------------------------------------------ |
| net9.0  | `Practice.Aspire.slnx`       | `#file:samples/practice_aspire/src/Practice.Aspire.AppHost/Program.cs 中 bookingapi 的 API 端點`       |
| net8.0  | `Practice.Aspire.Net8.slnx`  | `#file:samples/practice_aspire/src/Practice.Aspire.Net8.AppHost/Program.cs 中 bookingapi 的 API 端點`  |
| net10.0 | `Practice.Aspire.Net10.slnx` | `#file:samples/practice_aspire/src/Practice.Aspire.Net10.AppHost/Program.cs 中 bookingapi 的 API 端點` |

跨版本驗證的重點在於確認 Orchestrator 能正確處理不同版本的套件依賴與 API 差異。

---

## 還原驗證結果

Orchestrator 會在 `tests/` 目錄下產生測試程式碼。驗證完成後，可使用以下方式還原：

```powershell
# 還原單一驗證專案的測試結果
git restore samples/practice/tests/
git restore samples/practice_integration/tests/
git restore samples/practice_aspire/tests/
git restore samples/practice_tunit/tests/

# 還原所有驗證專案
git restore samples/

# 清理 Orchestrator 新增的未追蹤檔案
git clean -fd samples/
```
